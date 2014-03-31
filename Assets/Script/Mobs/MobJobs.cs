using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MobJobs : MonoBehaviour {
	// This is for jobs an indiviual MUST complete, rather than can be done by anyone
	private LinkedList<JobClass> PersonalJobList;

	public JobClass myJob;
	JobClass idleJob;
	//string currentJob = "Idle";

	GameManager manager;
	MobBehaviour mobBehave;
	MobMovement mobMove;
	MobItems mobItem;
	MobCondition mobCon;

	private int MAX_BUILD = 50; //HARDCODE? Can fix later
	private int build;
	private int buildPatience;

	private int MAX_SLEEP;
	private int SLEEP;
	private GameObject SleepZZZ;

	void Awake(){
		manager = GameObject.FindGameObjectWithTag ("Manager").GetComponent<GameManager> ();
		idleJob = ScriptableObject.CreateInstance<JobClass> ();
		idleJob.Initialise("Idle", Vector2.zero, false);
		myJob = idleJob;
	}
	// Use this for initialization
	void Start () {
		mobMove = GetComponent<MobMovement>();
		mobItem = GetComponent<MobItems>();
		mobBehave = GetComponent<MobBehaviour>();
		mobCon = GetComponent<MobCondition>();
		PersonalJobList = new LinkedList<JobClass>();
		BuildMax ();
		BuildPatienceMax();
	}

	private void BuildMax(){
		build = MAX_BUILD;
	}
	private void BuildPatienceMax(){
		buildPatience = MAX_BUILD;
	}
	
	// Update is called once per frame
	void Update () {
		LinkedList<JobClass> removeList;
		removeList = new LinkedList<JobClass>();
		foreach (JobClass job in PersonalJobList) {
			if(job.complete){
				removeList.AddLast(job);
			} else if (job.tries < 1) {
				removeList.AddLast(job);
			}
		}
		foreach (JobClass job in removeList) {
			RemovePersonalJob(job);
		}
		removeList.Clear ();
	}

	public void AddPersonalJob(JobClass job){
		job.assigned = false;
		job.personal = true;
		PersonalJobList.AddLast(job);
	}
	public void RemovePersonalJob(JobClass job){
		Destroy(job.GetSelectionRing());
		PersonalJobList.Remove (job);
	}

	// Job is finished
	void FinishJob(){
		myJob.complete = true;
		myJob = idleJob;
	}

	// Person gives up on job, but it's still available for taking
	public void AbandonJob(){
		//Debug.Log ("Abandoned Job");
		myJob.ReturnJob();
		myJob = idleJob;
	}

	// Abandons job, doesn't unassign it. Places copy on personal job list
	public void HoldJob(){
		Debug.Log ("Holding Job");
		AddPersonalJob(myJob);
		myJob = idleJob;
	}

	// Sets job as complete without completing it
	public void CancelJob(){
		// Currently Functionally identical to finish job
		// Different name for legibility 
		myJob.complete = true;
		myJob = idleJob;
	}

	//---------------------------------------------------------------------------------------------------------------------
	//--------------------------------------- PERSONAL JOB CREATION -------------------------------------------------------
	//---------------------------------------------------------------------------------------------------------------------


	public void CreateWalkJob(){
		//First check if there is a gathering area
		bool found = false;
		GameObject[] objs = GameObject.FindGameObjectsWithTag("GatheringZone");
		int[] newGridPos = {-1,-1};
		foreach(GameObject obj in objs){
			newGridPos = obj.GetComponent<ZoneDetails>().GetRandomSpace();
			found = true;
		}

		// BoredStep here? Get grid value, convert to world, jam in new job here
		if(!found){
			newGridPos = mobMove.BoredStep();
		}

		if(newGridPos[0] == -1 || newGridPos[1] == -1){
			Debug.Log ("Borked walk job");
		}else{
			Vector2 newPos;
			newPos.x = GameManager.GridToWorld(newGridPos[0]);
			newPos.y = GameManager.GridToWorld(newGridPos[1]);
			JobClass newJob = ScriptableObject.CreateInstance<JobClass>();
			newJob.Initialise("Walk", newPos, false);

			AddPersonalJob(newJob);
		}
	}
	public void CreatePickUpJob(){
		//Find item on groud
		GameObject[] obj = GameObject.FindGameObjectsWithTag("Resource");
		bool found = false;
		foreach(GameObject item in obj){
			if(!item.GetComponent<BuildingResourceDetails>().assigned && !found){
				Vector3 res = item.GetComponent<BuildingResourceDetails>().GetResources();
				Vector3 wantedRes = mobItem.GetWantedRes();
				if(res.x >= wantedRes.x && res.y >= wantedRes.y && res.z >= wantedRes.z){
					JobClass newJob = ScriptableObject.CreateInstance<JobClass>();
					newJob.Initialise("PickUp", item.transform.position, false);
					found = true;
					item.GetComponent<BuildingResourceDetails>().assigned = true;
					AddPersonalJob(newJob);
					break;
				}
			}
		}
		if(!found){
			AbandonJob();
		}
	}
	public void CreateDropJob(){
		//Find a spot on a stockpile
		bool found = false;
		foreach(GameObject pile in GameObject.FindGameObjectsWithTag("Stockpile")){
			int[] gridPos = pile.GetComponent<ZoneDetails>().GetFreeSpace();
			// Get free space returns a -1 if there is no space
			if(gridPos[0] == -1 || gridPos[1] == -1){
				found = false;
				continue;
			}
			found = true;
			JobClass newJob = ScriptableObject.CreateInstance<JobClass>();
			Vector2 pos = new Vector2(GameManager.GridToWorld(gridPos[0]), GameManager.GridToWorld(gridPos[1]));
			newJob.Initialise("Drop", pos, false);
			AddPersonalJob(newJob);
			break;
		}
		if(!found){
			int[] newGridPos = mobMove.BoredStep();
			if(newGridPos[0] != -1 && newGridPos[1] != -1){
				JobClass newJob = ScriptableObject.CreateInstance<JobClass>();
				
				Vector2 pos = new Vector2(GameManager.GridToWorld(newGridPos[0]), GameManager.GridToWorld(newGridPos[1]));
				newJob.Initialise("Drop", pos, false);
				AddPersonalJob(newJob);
			}
		}
	}

	public void CreateEatDrinkJob(string actionType){
		//Find item on groud
		GameObject[] obj = GameObject.FindGameObjectsWithTag("Item");
		bool found = false;
		foreach(GameObject item in obj){
			if(!item.GetComponent<ItemDetails>().assigned && !found){
				if((actionType == "Eat" && item.GetComponent<ItemDetails>().IsEdible() && mobBehave.hungry) || 
				   (actionType == "Drink" && item.GetComponent<ItemDetails>().IsDrinkable() && mobBehave.thirsty)){
					JobClass newJob = ScriptableObject.CreateInstance<JobClass>();
					newJob.Initialise("EatDrink", item.transform.position, false);
					newJob.JobDetails = actionType;
					found = true;
					item.GetComponent<ItemDetails>().assigned = true;
					AddPersonalJob(newJob);
					break;
				}
			}
		}
		if(!found){
			AbandonJob();
			if(actionType == "Eat"){
				StartCoroutine("HungryJobReset");
			}else if(actionType == "Drink"){
				StartCoroutine("ThirstyJobReset");
			}
		}
	}
	public void CreateSleepJob(float maxSleep){
		MAX_SLEEP = (int)maxSleep *30;
		//This would be the searching for bed part, but for now have them crash wherever
		int[] newGridPos = mobMove.BoredStep();
		if(newGridPos[0] != -1 && newGridPos[1] != -1){
			JobClass newJob = ScriptableObject.CreateInstance<JobClass>();
			
			Vector2 pos = new Vector2(GameManager.GridToWorld(newGridPos[0]), GameManager.GridToWorld(newGridPos[1]));
			newJob.Initialise("Sleep", pos, false);
			AddPersonalJob(newJob);
		}
	}
	IEnumerator HungryJobReset(){
		yield return new WaitForSeconds(10.0f);
		mobBehave.hungryJob = false;
	}
	IEnumerator ThirstyJobReset(){
		yield return new WaitForSeconds(10.0f);
		mobBehave.thirstyJob = false;
	}

	//---------------------------------------------------------------------------------------------------------------------
	//--------------------------------------------- MISC JOB CHECKS -------------------------------------------------------
	//---------------------------------------------------------------------------------------------------------------------

	public bool HaveJob(){
		return !myJob.IsIdle();
	}
	public bool CurrentJobIsPersonalWalkJob(){
		return (myJob.JobName == "Walk" && myJob.personal);
	}

	public bool FindJob(){
		bool haveJob = false;
		//Grab job from list
		foreach(JobClass job in PersonalJobList){
			if(myJob.IsIdle() && !(job.assigned)){
				job.TakeJob();
				myJob = job;
				haveJob = true;
			}
		}
		if(!haveJob){ // If no personal jobs, grab a community job
			foreach (JobClass job in manager.JobList) {
				if(myJob.IsIdle() && !(job.assigned)){
					job.TakeJob();
					myJob = job;
					haveJob = true;
				}
			}
		}
		// If there is no job but holding resources, drop
		if(!haveJob && mobItem.HasResources()){
			CreateDropJob();
		}
		return haveJob;
	}

	//---------------------------------------------------------------------------------------------------------------------
	//-------------------------------------------- JOB WORKING CODE -------------------------------------------------------
	//---------------------------------------------------------------------------------------------------------------------
	
	public bool DoJob(){
		bool atJobSite = false;
		if(!mobMove.walking){
			switch(myJob.JobName){
				//jobby shit here, if job can be done from current position, set atJobSite to true
			case "Mine":
				//Check the pathfinding 
				if(mobMove.StepsAwayFromGoal() == 1){
					atJobSite = true;

					//Do the job code here?
					Mine ();
				}else{
					mobMove.GetGoal();
				}
				break;
			case "Build":
				Vector3 neededRes = NeededResources();

				if((mobItem.GetResources().x >= neededRes.x) && (mobItem.GetResources().y >= neededRes.y) && (mobItem.GetResources().z >= neededRes.z)){
					//Debug.Log ("Has enough resources at " + item.GetResources().ToString());
					if(mobMove.OneStepAway()){
						atJobSite = true;
						Build ();
					}else{
						mobMove.GetGoal();
					}
				}else{
					//Wants resources
					mobItem.SetWantedRes(neededRes);
					mobBehave.PickUp();


					if(!myJob.personal){
						AbandonJob();
					}

				}
				break;
			case "Walk":
				if(mobMove.StepsAwayFromGoal() == 0){
					atJobSite = true;
					Walk ();
				}else{
					mobMove.GetGoal();
				}
				break;
			case "PickUp":
				if(mobMove.StepsAwayFromGoal() == 0){
					atJobSite = true;
					PickUp ();
				}else{
					mobMove.GetGoal();
				}
				break;
			case "Haul":
				if(mobMove.StepsAwayFromGoal() == 0){
					atJobSite = true;
					PickUp ();
					CreateDropJob();
				}else{
					mobMove.GetGoal();
				}
				break;

			case "Drop":
				if(mobMove.StepsAwayFromGoal() == 0){
					atJobSite = true;
					Drop ();
				}else{
					mobMove.GetGoal ();
				}
				break;
			case "EatDrink":
				if(mobMove.StepsAwayFromGoal() == 0){
					atJobSite = true;
					EatDrink ();
				}else{
					mobMove.GetGoal ();
				}
				break;
			case "Sleep":
				if(mobMove.StepsAwayFromGoal() == 0){
					atJobSite = true;
					Sleep();
				}else{
					mobMove.GetGoal();
				}
				break;
			}
		}

		return atJobSite;
	}
	//---------------------------------------------------------------------------------------------------------------------
	//------------------------------------------- SPECIFIC JOB CODE -------------------------------------------------------
	//---------------------------------------------------------------------------------------------------------------------
	void Mine(){
		if (manager.TerrainAtLocation(mobMove.GetXGoal(), mobMove.GetYGoal()) == -1) {
			//select and damage rock
			Vector2 rockPos = new Vector2 ();
			rockPos.x = GameManager.GridToWorld (mobMove.GetXGoal());
			rockPos.y = GameManager.GridToWorld (mobMove.GetYGoal());
			Collider2D[] rock = Physics2D.OverlapPointAll (rockPos);
			int index = 0;
			bool minedRock = false;
			while ((index < rock.Length) && !(minedRock)) {
				if (rock [index].transform.CompareTag ("Rock") || 
				    rock [index].transform.CompareTag ("Wood") || 
				    rock [index].transform.CompareTag ("Metal") ||
				    rock [index].transform.CompareTag ("Structure")  ) 
				{
					rock [index].gameObject.GetComponent<TerrainDetails> ().DamageTerrain (1);
					minedRock = true;
				}
				index++;
			}
		}else{
			FinishJob();
		}
	}

	void Build(){
		if(manager.TerrainAtLocation(mobMove.GetXGoal(), mobMove.GetYGoal()) == 0){
			// obNum = obstruction number
			int obNum = NoObstruction(mobMove.GetXGoal(), mobMove.GetYGoal());
			if(obNum == 0 || myJob.JobDetails == "Roof"){
				Vector3 res = mobItem.GetResources();
				Vector3 neededRes = NeededResources();

				if(build <= 0 && (res.x >= neededRes.x)&& (res.y >= neededRes.y)&& (res.z >= neededRes.z)){
					Vector3 pos = new Vector3 (GameManager.GridToWorld(mobMove.GetXGoal()), GameManager.GridToWorld(mobMove.GetYGoal()), 0);
					GameObject newTerrain;
					newTerrain = Instantiate (BuildingType(), pos, new Quaternion ()) as GameObject;
					newTerrain.GetComponent<TerrainDetails> ().Initialise (neededRes);
					
					manager.AddTerrain (newTerrain, mobMove.GetXGoal(), mobMove.GetYGoal());
					BuildMax();
					BuildPatienceMax();
					mobItem.SpendResources(neededRes);

					FinishJob();
				}else{
					build--;
				}
			}else if(obNum == 1){
				BuildMax();
				if(buildPatience-- <= 0){
					BuildPatienceMax();
					AbandonJob();
				}

			}else if(obNum == 2){
				CancelJob();
			}
		}else{
			FinishJob();
		}
	}

	int NoObstruction(int x, int y){
		//Convert grid to worldspace
		Vector2 pos = new Vector2 (GameManager.GridToWorld(mobMove.GetXGoal()), GameManager.GridToWorld(mobMove.GetYGoal()));
		
		Collider2D[] colliders = Physics2D.OverlapPointAll(pos);
		int noObstruction = 0; //No obstruction
		foreach(Collider2D col in colliders){
			//Debug.Log (col.ToString());
			if (col.CompareTag("Human")){
				if(!col.gameObject.GetComponent<MobJobs>().HaveJob()){
					//Debug.Log ("Told to take a step");
					col.gameObject.GetComponent<MobBehaviour>().TakeStep();
				}
				if(noObstruction <= 1){
					noObstruction = 1;
				}
			}
			else if(col.CompareTag("Resource") || col.CompareTag("Item")){
				if(noObstruction <= 2){
					noObstruction = 2;
				}
			}
		}
		return noObstruction;
		
	}

	void Walk(){
		FinishJob();
	}

	void PickUp(){
		//Pick up item
		//	Reduce amount of resource on item
		//Vector2 pos = new Vector2 (GameManager.GridToWorld(move.GetXGoal()), GameManager.GridToWorld(move.GetYGoal()));
		Vector2 pos = transform.position;
		Collider2D[] colliders = Physics2D.OverlapPointAll(pos);

		foreach(Collider2D col in colliders){
			if(col.CompareTag("Resource")){
				// Take what you want from item
				Vector3 taken = col.gameObject.GetComponent<BuildingResourceDetails>().TakeAllResource(); 
				/*HARDCODE - should be some switch for picking up what you want for building and everything for hauling
				 */
				// Hold what you take
				mobItem.PickUpResources(taken);
				// Change how much you want
				mobItem.AddWantedRes(-taken);

				break;
			}else if(col.CompareTag("Item")){
				//Pick up the item
				//We need a proper inventory for that
				//SHit - maybe later

				mobItem.PickUpItem(col.GetComponent<ItemDetails>().GetItemNumber());
				Destroy(col.gameObject);
			}
		}
		//	Increase amount of resource on person
		FinishJob ();
	}

	void Drop(){
		//Check for rocks in the way
		Vector2 pos = transform.position;
		Collider2D[] colliders = Physics2D.OverlapPointAll(pos);
		bool problem = false;
		foreach(Collider2D hit in colliders){
			if(hit.CompareTag("Resource") || hit.CompareTag("Item")){
				//needs to look elsewhere
				problem = true;
				break;
			}
		}
		if(problem){
			//Remake drop job
			FinishJob ();
			CreateDropJob();
		}else{
			if(!mobItem.GetResources().Equals(Vector3.zero)){
				//******************************************************     HARD CODE v
				GameObject itemType = DroppedResourceType();
				GameObject newItem = Instantiate(itemType, transform.position, transform.rotation) as GameObject;
				Vector3 Dropped = mobItem.ForceDropResources(mobItem.GetResources());
				newItem.GetComponent<BuildingResourceDetails>().Initialise(Dropped);

			}else if(mobItem.InventoryHasItem()){
				GameObject itemType = manager.TerrainTypes[mobItem.DropItemAtIndex(0)];
				//GameObject newItem = Instantiate(itemType, transform.position, transform.rotation) as GameObject;
				Instantiate(itemType, transform.position, transform.rotation);
			}
			FinishJob();
		}

	}

	void EatDrink(){
		Debug.Log ("EatDrink() Method entry");
		if(myJob.JobDetails == "Eat"){
			mobBehave.hungryJob = false;
		}else if(myJob.JobDetails == "Drink"){
			mobBehave.thirstyJob = false;
		}
		Vector2 pos = transform.position;
		Collider2D[] colliders = Physics2D.OverlapPointAll(pos);

		foreach(Collider2D col in colliders){
			if(col.CompareTag("Item")){
				// Take what you want from item
				if(myJob.JobDetails == "Eat" && mobBehave.hungry && col.gameObject.GetComponent<ItemDetails>().IsEdible()){
					Debug.Log ("EatDrink() \"Eat\" section");
					//Eat the food
					mobCon.EatFood(col.gameObject.GetComponent<ItemDetails>().GetEatPower());
					mobCon.Drink(col.gameObject.GetComponent<ItemDetails>().GetDrinkPower());
					
					//consume item
					col.gameObject.GetComponent<ItemDetails>().ConsumeItem();
					break;
				}
				if(myJob.JobDetails == "Drink" && mobBehave.thirsty && col.gameObject.GetComponent<ItemDetails>().IsDrinkable()){
					Debug.Log ("EatDrink() \"Drink\" section");
					//Eat the food
					mobCon.EatFood(col.gameObject.GetComponent<ItemDetails>().GetEatPower());
					mobCon.Drink(col.gameObject.GetComponent<ItemDetails>().GetDrinkPower());
					
					//consume item
					col.gameObject.GetComponent<ItemDetails>().ConsumeItem();
					break;
				}

			}
		}
		//	Increase amount of resource on person
		FinishJob ();

	}

	void Sleep(){

		if(SLEEP == 0){//Start new sleep
			SLEEP = MAX_SLEEP;
			SleepZZZ = Instantiate(manager.TerrainTypes[(int)GameManager.TerrainIndex.sleepZZZ], transform.position + new Vector3(0.15f, 0.15f, 0.0f),transform.rotation) as GameObject;
		}else if(--SLEEP == 0){
			mobBehave.sleepJob = false;
			mobCon.Sleep();
			Destroy(SleepZZZ);
			FinishJob();
		}
	}


	//---------------------------------------------------------------------------------------------------------------------
	//--------------------------------------- HELPER FUNCTIONS FOR JOB CODE -----------------------------------------------
	//---------------------------------------------------------------------------------------------------------------------

	Vector3 NeededResources(){
		if(myJob.JobDetails == "Rock"){
			return new Vector3(0,50,0);
		}else if(myJob.JobDetails == "Wood"){
			return new Vector3(50,0,0);
		}else if(myJob.JobDetails == "Metal"){
			return new Vector3(0,0,50);
		}else if(myJob.JobDetails == "WoodDoor"){
			return new Vector3(50,0,0);
		}else if(myJob.JobDetails == "Roof"){
			return new Vector3(50,0,0);
		}
		Debug.LogError("Method NeededResources() didn't find appropriate job detail.");
		return new Vector3();
	}

	GameObject BuildingType(){
		if(myJob.JobDetails == "Rock"){
			return manager.TerrainTypes [(int)GameManager.TerrainIndex.stoneWall];

		}else if(myJob.JobDetails == "Wood"){
			return manager.TerrainTypes [(int)GameManager.TerrainIndex.woodWall];

		}else if(myJob.JobDetails == "Metal"){
			return manager.TerrainTypes [(int)GameManager.TerrainIndex.metalWall];
		}else if(myJob.JobDetails == "WoodDoor"){
			return manager.TerrainTypes [(int)GameManager.TerrainIndex.woodDoor];
		}else if(myJob.JobDetails == "Roof"){
			return manager.TerrainTypes[(int)GameManager.TerrainIndex.roof];
		}
		Debug.LogError("Method BuildingType() didn't find appropriate job detail.");
		return null;
	}

	GameObject DroppedResourceType(){
		Vector3 res = mobItem.GetResources();
		if((res.x > res.y) && (res.x > res.z)){
			return manager.TerrainTypes[(int)GameManager.TerrainIndex.woodLog];
		}else if((res.y > res.x) && (res.y > res.z)){
			return manager.TerrainTypes[(int)GameManager.TerrainIndex.rockPiece];
		}else{
			return manager.TerrainTypes[(int)GameManager.TerrainIndex.metalPiece];
		}
	}
}
