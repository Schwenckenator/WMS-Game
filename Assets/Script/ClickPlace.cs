using UnityEngine;
using System.Collections;

public class ClickPlace : MonoBehaviour {

	private GameManager manager;
	private string placeType;
	Vector2 corner;

	// Use this for initialization
	void Start () {
		manager = GetComponent<GameManager>();
		Reset ();
	}
	
	// Update is called once per frame
	void Update () {
		if(placeType != "None"){
			if(Input.GetMouseButtonUp(0)){
				if(!manager.clickFlag){
					Vector3 camPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

					corner.x = GameManager.SnapToGrid(camPos.x);
					corner.y = GameManager.SnapToGrid(camPos.y);

					if(placeType == "WoodenDoor"){
						PlaceDoor();
					}
					if(!Input.GetKey(KeyCode.LeftShift)){
						Reset ();
					}
				}else{
					manager.clickFlag = false;
				}
			}
		}

	}

	void Reset(){
		placeType = "None";
	}
	public void SetPlaceType(string input){
		placeType = input;
		manager.clickFlag = true;
	}

	void PlaceDoor(){
		Debug.Log("PlaceDoor()");
		int xGridCorner, yGridCorner;
		xGridCorner = GameManager.WorldToGrid(corner.x);
		yGridCorner = GameManager.WorldToGrid(corner.y);
		if(manager.TerrainAtLocation(xGridCorner,yGridCorner) == 0){
			Vector3 pos = new Vector3(GameManager.GridToWorld(xGridCorner),GameManager.GridToWorld(yGridCorner), 0);
			
			JobClass job = ScriptableObject.CreateInstance<JobClass> ();
			job.Initialise("Build", pos, true);
			if(placeType == "WoodenDoor"){
				job.JobDetails = "WoodDoor";
			}else if(placeType == "StoneDoor"){
				job.JobDetails = "StoneDoor";
			}else if(placeType == "MetalDoor"){
				job.JobDetails = "MetalDoor";
			}
			manager.AddJob(job);
		}
	}
}
