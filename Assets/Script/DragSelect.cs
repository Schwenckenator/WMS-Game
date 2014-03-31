using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DragSelect : MonoBehaviour {

	public GameObject selector;

	public Material[] selectorMaterials;
	public enum colour {red, blue, white, black};
	

	private GameManager manager;

	private GameObject selectorInstance;
	private Vector2 corner;
	private Vector2 selectorCorner;

	private string selectionType;

	// Use this for initialization
	void Start () {
		// This works because this script is on the game manager
		manager = GetComponent<GameManager>();
		Reset ();
	}

	public string GetSeletionType(){
		return selectionType;
	}
	public void SetSelectionType(string newType){
		selectionType = newType;
		manager.clickFlag = true;
	}
	
	// Update is called once per frame
	void Update () {
		/* ******************************************************************************************
		 * 							LEFT CLICK
		 * ******************************************************************************************/

		// ********** Mouse press **********
		if(selectionType != "None" && !manager.clickFlag){
			if(Input.GetMouseButtonDown(0)){
				Vector3 camPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				corner.x = GameManager.SnapToGrid(camPos.x);
				corner.y = GameManager.SnapToGrid(camPos.y);
				//
				
				selectorInstance = Instantiate(selector, corner, new Quaternion()) as GameObject;
				selectorInstance.transform.localScale = new Vector3();

				Material selectorColour = selectorMaterials[(int)colour.white];
				if(selectionType == "Mine" || selectionType == "Chop" || selectionType == "Demolish"){
					selectorColour = selectorMaterials[(int)colour.blue];
				}else if(selectionType == "BuildRockWall" || selectionType == "BuildWoodWall" || selectionType == "BuildMetalWall" ){
					selectorColour = selectorMaterials[(int)colour.red];
				}else if(selectionType == "Stockpile" || selectionType == "GatheringZone"){
					selectorColour = selectorMaterials[(int)colour.white];
				}else if(selectionType == "CancelJob"){
					selectorColour = selectorMaterials[(int)colour.black];
				}

				selectorInstance.transform.GetChild(0).renderer.material = selectorColour;
			}
			// ********** Mouse Drag **********
			else if(Input.GetMouseButton(0)){
				Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

				Vector2 dir = mousePos - corner; // Non rounded direction

				mousePos.x = GameManager.SnapToGrid(mousePos.x);
				mousePos.y = GameManager.SnapToGrid(mousePos.y);
				
				Vector2 resizeVector = mousePos - corner; // Rounded direction


				if(dir.x < 0){
					resizeVector.x -= 0.25f;
					selectorCorner.x = corner.x + 0.125f;
				}else{
					resizeVector.x += 0.25f;
					selectorCorner.x = corner.x - 0.125f;
				}
				if(dir.y < 0){
					resizeVector.y -= 0.25f;
					selectorCorner.y = corner.y + 0.125f;
				}else{
					resizeVector.y += 0.25f;
					selectorCorner.y = corner.y - 0.125f;
				}


				Vector2 newScale = selectorInstance.transform.localScale;
				newScale.x = resizeVector.x;
				newScale.y = -resizeVector.y;
				
				selectorInstance.transform.localScale = newScale;
				selectorInstance.transform.position = selectorCorner;
			}
			// ********** Mouse Raise **********
			else if(Input.GetMouseButtonUp(0)){
				// If the selector exists
				if(selectorInstance){
					if(selectionType == "Mine" || selectionType == "Chop" || selectionType == "Demolish"){
						SelectObjects();
					}else 
					if(selectionType == "BuildRockWall" || selectionType == "BuildWoodWall" || selectionType == "BuildMetalWall"){
						PlaceObjects();
					}else 
					if(selectionType == "Stockpile" || selectionType == "GatheringZone"){
						PlaceZone ();
					}else 					
					if(selectionType == "CancelJob"){
						CancelSelectedJobs ();
					}else 					
					if(selectionType == "DeleteZone"){
						DeleteZone();
					}else 
					if(selectionType == "BuildRoof"){
						PlaceRoof();
					}

					if(!Input.GetKey(KeyCode.LeftShift)){
						Reset ();
					}
					Destroy(selectorInstance);
				}
			}
		}else{
			manager.clickFlag = false;
		}
	}
	void Reset(){
		selectionType = "None";
	}
	void CancelSelectedJobs(){
		Debug.Log("CancelSelectedJobs()");
		Vector2 temp = selectorInstance.transform.localScale;
		temp.y *= -1;
		Vector2 point = selectorCorner + temp;

		Collider2D[] obj;
		obj = Physics2D.OverlapAreaAll(selectorCorner, point);

		//Assemble a list of jobs to cancel
		List<JobClass> cancelList = new List<JobClass>();
		for(int i=0; i< obj.Length; i++){
			if(obj[i].CompareTag("SelectionRing")){
				cancelList.Add(obj[i].GetComponent<SelectionRingDetails>().GetJob());
			}
		}
		//Mark the jobs as complete
		//Just loop through the cancel list? Will this work?
		foreach (JobClass job in cancelList){
			job.complete = true;
		}


	}
	void SelectObjects(){
		Vector2 temp = selectorInstance.transform.localScale;
		temp.y *= -1;
		Vector2 point = selectorCorner + temp;

		//For every terrain object in screen, are you in here?
		// --- Will need optimising later, I'm sure of it. That's if it works.
		// ^ scratch that, overlap area ftw!
		Collider2D[] obj;
		obj = Physics2D.OverlapAreaAll(selectorCorner, point);

		for(int i=0; i< obj.Length; i++){
			//Don't an hero, ask to be mined
			if(selectionType == "Mine"){
				if(obj[i].CompareTag("Rock") || obj[i].CompareTag("Metal")){
					JobClass job = ScriptableObject.CreateInstance<JobClass> ();
					job.Initialise("Mine", obj[i].transform.position, true);
					manager.AddJob(job);
				}
			}else if(selectionType == "Chop"){
				if(obj[i].CompareTag("Wood")){
					JobClass job = ScriptableObject.CreateInstance<JobClass> ();
					job.Initialise("Mine", obj[i].transform.position, true);
					manager.AddJob(job);
				}
			}else if(selectionType == "Demolish"){
				if(obj[i].CompareTag("Structure")){
					JobClass job = ScriptableObject.CreateInstance<JobClass> ();
					job.Initialise("Mine", obj[i].transform.position, true);
					manager.AddJob(job);
				}
			}

		}
	}
	void PlaceObjects(){
		//Find which grid spaces the selector is over
		//Populate with rocks

		//Find grid space one corner is in
		//Find grid space opposite corner is in
			// Every space between those two points is covered
		// Loop through each axis, place rock there
		Vector2 temp = selectorInstance.transform.localScale;
		temp.y *= -1;
		Vector2 dir = temp;
		//Undo selector movement


		Vector2 point = corner + temp;

		if(dir.x < 0){
			point.x += 0.25f;
			corner.x += 0.25f;
		}
		if(dir.y < 0){
			point.y += 0.25f;
			corner.y += 0.25f;
		}

		int xGridCorner, yGridCorner;
		int xGridPoint, yGridPoint;

		// Corner to grid space
		xGridCorner = GameManager.WorldToGrid(corner.x);
		yGridCorner = GameManager.WorldToGrid(corner.y);
		// Point to grid spcae
		xGridPoint = GameManager.WorldToGrid(point.x);
		yGridPoint = GameManager.WorldToGrid(point.y);

		//Need to make it loop from lowest value to highest value,
		// time for tricky variable fun

		int xLow = Mathf.Min(xGridCorner, xGridPoint);
		int xHigh = Mathf.Max(xGridCorner, xGridPoint);

		int yLow = Mathf.Min(yGridCorner, yGridPoint);
		int yHigh = Mathf.Max(yGridCorner, yGridPoint);

		for(int i = xLow; i< xHigh; i++){ 
			// Is the x point on the grid?
			if(i < GameManager.MaxGrid() && i >= 0){
				for(int j = yLow; j< yHigh; j++){
					// Is the y point on the grid?
					if(j < GameManager.MaxGrid() && j >= 0){
						//If on the edges
						if(i == xLow || i == xHigh-1 || j == yLow || j == yHigh-1){
							// Is the location clear?
							if(manager.TerrainAtLocation(i,j) == 0){
								// Convert Grid space back into world space
								Vector3 pos = new Vector3(GameManager.GridToWorld(i),GameManager.GridToWorld(j), 0);

								JobClass job = ScriptableObject.CreateInstance<JobClass> ();
								job.Initialise("Build", pos, true);
								if(selectionType == "BuildRockWall"){
									job.JobDetails = "Rock";
								}else if(selectionType == "BuildWoodWall"){
									job.JobDetails = "Wood";
								}else if(selectionType == "BuildMetalWall"){
									job.JobDetails = "Metal";
								}

								manager.AddJob(job);
							}
						}
					}
				}
			}
		}

	}
	void PlaceRoof(){
		//Find which grid spaces the selector is over
		//Populate with rocks
		
		//Find grid space one corner is in
		//Find grid space opposite corner is in
		// Every space between those two points is covered
		// Loop through each axis, place rock there
		Vector2 temp = selectorInstance.transform.localScale;
		temp.y *= -1;
		Vector2 dir = temp;
		//Undo selector movement
		
		
		Vector2 point = corner + temp;
		
		if(dir.x < 0){
			point.x += 0.25f;
			corner.x += 0.25f;
		}
		if(dir.y < 0){
			point.y += 0.25f;
			corner.y += 0.25f;
		}
		
		int xGridCorner, yGridCorner;
		int xGridPoint, yGridPoint;
		
		// Corner to grid space
		xGridCorner = GameManager.WorldToGrid(corner.x);
		yGridCorner = GameManager.WorldToGrid(corner.y);
		// Point to grid spcae
		xGridPoint = GameManager.WorldToGrid(point.x);
		yGridPoint = GameManager.WorldToGrid(point.y);
		
		//Need to make it loop from lowest value to highest value,
		// time for tricky variable fun
		
		int xLow = Mathf.Min(xGridCorner, xGridPoint);
		int xHigh = Mathf.Max(xGridCorner, xGridPoint);
		
		int yLow = Mathf.Min(yGridCorner, yGridPoint);
		int yHigh = Mathf.Max(yGridCorner, yGridPoint);

		// If it's a corner, you need a support // If on a corner but there is support
		//Check for corner support first.
		//If absent, abandon
		Debug.Log ("Terrain At "+ xLow.ToString()+", "+yLow.ToString()+" : "+manager.TerrainAtLocation(xLow, yLow));
		Debug.Log ("Terrain At "+ xLow.ToString()+", "+ (yHigh-1).ToString()+" : "+manager.TerrainAtLocation(xLow, yHigh-1));
		Debug.Log ("Terrain At "+ (yHigh-1).ToString()+", "+ yLow.ToString()+" : "+manager.TerrainAtLocation(xHigh-1, yLow));
		Debug.Log ("Terrain At "+ (xHigh-1).ToString()+", "+ (yHigh-1).ToString()+" : "+manager.TerrainAtLocation(xHigh-1, yHigh-1));


		if(manager.TerrainAtLocation(xLow, yLow) == -1 || 
		   manager.TerrainAtLocation(xLow, yHigh-1) == -1 ||
		   manager.TerrainAtLocation(xHigh-1, yLow) == -1 || 
		   manager.TerrainAtLocation(xHigh-1, yHigh-1) == -1)
		{
			for(int i = xLow; i< xHigh; i++){ 
				// Is the x point on the grid?
				if(i < GameManager.MaxGrid() && i >= 0){
					for(int j = yLow; j< yHigh; j++){
						// Is the y point on the grid?
						if(j < GameManager.MaxGrid() && j >= 0){

							// Convert Grid space back into world space
							Vector3 pos = new Vector3(GameManager.GridToWorld(i),GameManager.GridToWorld(j), 0);
							
							JobClass job = ScriptableObject.CreateInstance<JobClass> ();
							job.Initialise("Build", pos, true);
							job.JobDetails = "Roof";
							
							manager.AddJob(job);
						}
					}
				}
			}
		}


	}

	void PlaceZone(){
		//Get scale
		//Check for min size, 3x3
		float xScale = selectorInstance.transform.localScale.x;
		float yScale = selectorInstance.transform.localScale.y;

		//Make scale positive
		if(xScale < 0) xScale *= -1;
		if(yScale < 0) yScale *= -1;

		if(xScale >= 0.75f && yScale >= 0.75f){
			GameObject newZone = null;
			//Instatiate stockpile object undernearth with same scale and position as selector instance
			//Stockpile exists, give it info about its grid points
			if(selectionType == "GatheringZone"){
				newZone = Instantiate(manager.TerrainTypes[(int)GameManager.TerrainIndex.gatheringZone], 
				                                 selectorInstance.transform.position, new Quaternion()) as GameObject;
			}else if(selectionType == "Stockpile"){
				newZone = Instantiate(manager.TerrainTypes[(int)GameManager.TerrainIndex.stockpile], 
				                                      selectorInstance.transform.position, new Quaternion()) as GameObject;
			}

			newZone.transform.localScale = selectorInstance.transform.localScale;
			//First, find grid point corners
			
			Vector2 temp = selectorInstance.transform.localScale;
			temp.y *= -1;
			Vector2 dir = temp;
			//Undo selector movement
			
			
			Vector2 point = corner + temp;
			
			if(dir.x < 0){
				point.x += 0.25f;
				corner.x += 0.25f;
			}
			if(dir.y < 0){
				point.y += 0.25f;
				corner.y += 0.25f;
			}
			
			int xGridCorner, yGridCorner;
			int xGridPoint, yGridPoint;
			
			// Corner to grid space
			xGridCorner = GameManager.WorldToGrid(corner.x);
			yGridCorner = GameManager.WorldToGrid(corner.y);
			// Point to grid spcae
			xGridPoint = GameManager.WorldToGrid(point.x);
			yGridPoint = GameManager.WorldToGrid(point.y);
			
			//Need to make it loop from lowest value to highest value,
			// time for tricky variable fun
			
			int xLow = Mathf.Min(xGridCorner, xGridPoint);
			int xHigh = Mathf.Max(xGridCorner, xGridPoint);
			
			int yLow = Mathf.Min(yGridCorner, yGridPoint);
			int yHigh = Mathf.Max(yGridCorner, yGridPoint);
			
			//Stockpile exists, give it info about its grid points
			newZone.GetComponentInChildren<ZoneDetails>().Initialise(xLow,xHigh, yLow, yHigh);
		}

	}
	void DeleteZone(){
		Debug.Log ("Deleting Zone!");
		Vector2 temp = selectorInstance.transform.localScale;
		temp.y *= -1;
		Vector2 point = selectorCorner + temp;
		
		//For every terrain object in screen, are you in here?
		// --- Will need optimising later, I'm sure of it. That's if it works.
		// ^ scratch that, overlap area ftw!
		Collider2D[] obj;
		obj = Physics2D.OverlapAreaAll(selectorCorner, point);
		
		for(int i=0; i< obj.Length; i++){
			Debug.Log (obj[i].tag);
			if(obj[i].CompareTag("GatheringZone") || obj[i].CompareTag("Stockpile")){
				Destroy (obj[i].gameObject);
			}
		}
	}

}
