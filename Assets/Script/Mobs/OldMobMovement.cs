//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;
//
//public class OldMobMovement : MonoBehaviour {
//	Vector3 goalPos;
//	JobClass myJob;
//	float speed = 0.02f;
//	string currentJob = "Idle";
//	GameManager manager;
//	private int STEP_COST = 1;
//	public int[,] PathGrid;
//
//	private int MAX_RELAX = 100;
//	private int relax;
//	private int MAX_STUCK = 200;
//	private int stuck;
//	private int MAX_BORED = 500;
//	private int bored;
//
//	private int x_grid;
//	private int y_grid;
//	
//	private int x_goal;
//	private int y_goal;
//
//	private int lastXNav;
//	private int lastYNav;
//
//	private int numOfCellsChecked = 0;
//	private int numOfCellsChanged = 0;
//
//	void Start () {
//		manager = GameObject.FindGameObjectWithTag ("Manager").GetComponent<GameManager> ();
//		PathGrid = new int[manager.MAX_GRID,manager.MAX_GRID];
//		myJob = ScriptableObject.CreateInstance<JobClass> ();
//		myJob.Initialise("Idle", Vector2.zero);
//		Destroy(myJob.GetSelectionRing());
//		goalPos = transform.position;
//
//		stuck = MAX_STUCK;
//		bored = Random.Range(0, MAX_BORED);
//	}
//	
//	// Update is called once per frame
//	void Update () {
//		//Look for a job
//		if (currentJob == "Idle") {
//			if (relax <= 0) {
//				FindJob ();
//			} else {
//				relax--;
//			}
//
//		} else {
//		//
//			if (transform.position != goalPos) {
//				Vector3 path = goalPos - transform.position;
//				if (path.magnitude < speed) {
//					transform.position = goalPos;
//				} else {
//					path.Normalize ();
//					transform.position += path * speed;
//				}
//			} else {
//				MyGridPos ();
//				switch(currentJob){
//				case "Mine":
//					if((PathGrid [x_grid, y_grid] < 3) && (PathGrid[x_grid,y_grid] >=0)){
//						if (manager.TerrainAtLocation (x_goal, y_goal) == -1) {
//							//select and damage rock
//							Vector2 rockPos = new Vector2 ();
//							rockPos.x = (x_goal - manager.MAX_GRID / 2) * .25f;
//							rockPos.y = (y_goal - manager.MAX_GRID / 2) * .25f;
//							Collider2D[] rock = Physics2D.OverlapCircleAll (rockPos, 0.05f);
//							int index = 0;
//							bool minedRock = false;
//							while ((index < rock.Length) && !(minedRock)) {
//								if (rock [index].transform.CompareTag ("Rock")) {
//									rock [index].gameObject.GetComponent<TerrainDetails> ().DamageTerrain (Mathf.RoundToInt (1));
//									minedRock = true;
//								}
//								index++;
//							}
//						}else{
//							FinishJob();
//						}
//					}else{
//						GetGoal ();
//					}
//					break;
//				case "Build":
//					if((PathGrid[x_grid,y_grid] >=0) && 
//					   ((PathGrid [x_grid, y_grid] < 3) || 
//					   ((PathGrid [x_grid, y_grid] < 4) && (x_grid-1 == x_goal || x_grid+1 == x_goal) && (y_grid-1 == y_goal || y_grid+1 == y_goal)))){
//
//						if ((manager.TerrainAtLocation (x_goal, y_goal) == 0)) {
//							if(NoPeople(x_goal, y_goal)){
//								Vector3 pos = new Vector3 (((x_goal - manager.MAX_GRID / 2) * .25f), ((y_goal - manager.MAX_GRID / 2) * .25f), 0);
//								GameObject newTerrain;
//								newTerrain = Instantiate (manager.TerrainTypes [(int)GameManager.TerrainIndex.rock], pos, new Quaternion ()) as GameObject;
//								newTerrain.GetComponent<TerrainDetails> ().Initialise (new Vector3 (0.0f, 50.0f, 0.0f));
//								
//								manager.AddTerrain (newTerrain, x_goal, y_goal);
//								FinishJob();
//							}else{
//								AbandonJob();
//							}
//						}else{
//							// Shit's in the way, so fuck it, it's done already
//							FinishJob();
//						}
//					}else{
//						GetGoal ();
//					}
//					break;
//				case "Walk":
//					if ((x_goal == x_grid) && (y_goal == y_grid)) {
//						FinishJob();
//					}else{
//						GetGoal();
//					}
//					break;
//				}
//
//				stuck--;
//				if(stuck <= 0){
//					AbandonJob();
//				}
//			}
//		}
//	}
//	bool NoPeople(int x, int y){
//		//Convert grid to worldspace
//		Vector2 pos = new Vector2 (((x_goal - manager.MAX_GRID / 2) * .25f), ((y_goal - manager.MAX_GRID / 2) * .25f));
//
//		Collider2D[] colliders = Physics2D.OverlapCircleAll(pos, 0.1f);
//		bool noPerson = true;
//		foreach(Collider2D col in colliders){
//			Debug.Log (col.ToString());
//			if (col.CompareTag("Human")){
//				noPerson = false;
//			}
//		}
//		return noPerson;
//
//	}
//
//	void FinishJob(){
//		currentJob = "Idle"; 
//		myJob.complete = true;
//		stuck = MAX_STUCK;
//	}
//	void AbandonJob(){
//
//		myJob.ReturnJob();
//
//		currentJob = "Idle";
//		relax = MAX_RELAX;
//		stuck = MAX_STUCK;
//	}
//	void FindJob(){
//		//Grab job from list
//		foreach (JobClass job in manager.JobList) {
//			if((currentJob == "Idle") && !(job.assigned)){
//				currentJob = job.JobName;
//				job.TakeJob();
//				myJob = job;
//				//find goal position on grid
//				x_goal = Mathf.RoundToInt( job.JobLocation.x*4 + manager.MAX_GRID/2);
//				y_goal = Mathf.RoundToInt( job.JobLocation.y*4 + manager.MAX_GRID/2);
//
//				MyGridPos();
//				BuildPathTo(x_goal, y_goal);
//			}
//		}
//		if(currentJob == "Idle"){
//			if (bored <= 0){
//				RandomWalkJob(10, 30, 10, 30);
//				bored = Random.Range(0, MAX_BORED);
//			}else{
//				bored--;
//			}
//		}
//	}
//
//	// Creates a random Walk job
//	void WalkJob(int x, int y){
//		if(manager.TerrainAtLocation(x,y) == 0){
//			currentJob = "Walk";
//			x_goal = x;
//			y_goal = y;
//			MyGridPos();
//			BuildPathTo(x_goal, y_goal);
//		}
//	}
//	void RandomWalkJob(int xLow, int xHigh, int yLow, int yHigh){
//		int x, y;
//		do{
//			x  = Random.Range(xLow, xHigh);
//			y = Random.Range(yLow, yHigh);
//		}while (manager.TerrainAtLocation(x, y) != 0);
//		WalkJob(x,y);
//	}
//
//	void MyGridPos(){
//		//find my position on grid
//		x_grid = Mathf.RoundToInt(transform.position.x*4 + manager.MAX_GRID/2);
//		y_grid = Mathf.RoundToInt(transform.position.y*4 + manager.MAX_GRID/2);
//	}
//
//	void RetrieveTerrainGrid(){ // Gets the Terrain grid from the manager and assigns the value to the mob grid
//		for (int i = 0; i < manager.MAX_GRID; i++) {
//			for( int j=0; j < manager.MAX_GRID; j++){
//				PathGrid[i,j] = manager.TerrainGrid[i,j];
//			}
//		}
//	}
//
//	void GetGoal(){
//		MyGridPos ();
//		int x_nav = x_grid, y_nav = y_grid;
//
//
//		//---------------------------- Normal ----------------------------------------------------------------------
//
//		// if x-1 is on grid
//		if (x_grid > 0) {
//			// if x-1 y-1 tile has lower cost and is pathable
//			if((PathGrid[x_grid-1,y_grid] < PathGrid[x_grid,y_grid]) && (PathGrid[x_grid-1,y_grid] > 0)){
//				//set x-1 as target tile
//				if((manager.TerrainAtLocation(x_grid-1,y_grid) >= 0) || (PathGrid[x_grid-1,y_grid] == 1)){
//					x_nav = x_grid - 1;
//					y_nav = y_grid;
//				}
//			}
//		}
//
//		// ditto x+1
//		if (x_grid+ 1 < (manager.MAX_GRID)) {
//			if((PathGrid[x_grid+1,y_grid] < PathGrid[x_grid,y_grid]) && (PathGrid[x_grid+1,y_grid] > 0)){
//				if((manager.TerrainAtLocation(x_grid+1,y_grid) >= 0) || (PathGrid[x_grid+1,y_grid] == 1)){
//					x_nav = x_grid + 1;
//					y_nav = y_grid;
//				}
//			}
//		}
//
//		// ditto y-1
//		if (y_grid > 0) {
//			if((PathGrid[x_grid,y_grid-1] < PathGrid[x_grid,y_grid]) && (PathGrid[x_grid,y_grid-1] > 0)){
//				if((manager.TerrainAtLocation(x_grid,y_grid-1) >= 0) || (PathGrid[x_grid,y_grid-1] == 1)){
//					x_nav = x_grid;
//					y_nav = y_grid - 1;
//				}
//			}
//		}
//		// ditto y+1
//		if (y_grid + 1 < (manager.MAX_GRID)) {
//			if((PathGrid[x_grid,y_grid+1] < PathGrid[x_grid,y_grid]) && (PathGrid[x_grid,y_grid+1] > 0)){
//				if((manager.TerrainAtLocation(x_grid,y_grid+1) >= 0) || (PathGrid[x_grid,y_grid+1] == 1)){
//					x_nav = x_grid;
//					y_nav = y_grid + 1;
//				}
//			}
//		}
//
//		/*------------------------ DIAGONALS --------------------------------------------*/
//		
//		// ditto x-1 y-1
//		if (x_grid > 0 && y_grid > 0) {
//			// if x-1 tile has lower cost and is pathable
//			if((PathGrid[x_grid-1,y_grid-1] < PathGrid[x_grid,y_grid]) &&
//			   (PathGrid[x_grid-1,y_grid-1] > 0) &&
//			   (PathGrid[x_grid,y_grid-1] >= 0) &&
//			   (PathGrid[x_grid-1,y_grid] >= 0)){
//				if((manager.TerrainAtLocation(x_grid-1,y_grid-1) >= 0)/* || (PathGrid[x_grid-1,y_grid-1] == 1)*/){
//					x_nav = x_grid - 1;
//					y_nav = y_grid - 1;
//				}
//			}
//		}
//		// ditto x+1 y-1
//		if (x_grid+ 1 < (manager.MAX_GRID)&& y_grid > 0) {
//			// if x+1 y-1 tile has lower cost and is pathable
//			if((PathGrid[x_grid+1,y_grid-1] < PathGrid[x_grid,y_grid]) && 
//			   (PathGrid[x_grid+1,y_grid-1] > 0) &&
//			   (PathGrid[x_grid,y_grid-1] >= 0) &&
//			   (PathGrid[x_grid+1,y_grid] >= 0)){
//				if((manager.TerrainAtLocation(x_grid+1,y_grid-1) >= 0)/* || (PathGrid[x_grid+1,y_grid-1] == 1)*/){
//					x_nav = x_grid + 1;
//					y_nav = y_grid - 1;
//				}
//			}
//		}
//		// ditto x-1 y+1
//		if (x_grid > 0 && y_grid + 1 < (manager.MAX_GRID)) {
//			// if x+1 y-1 tile has lower cost and is pathable
//			if((PathGrid[x_grid-1,y_grid+1] < PathGrid[x_grid,y_grid]) && 
//			   (PathGrid[x_grid-1,y_grid+1] > 0) &&
//			   (PathGrid[x_grid,y_grid+1] >= 0) &&
//			   (PathGrid[x_grid-1,y_grid] >= 0)){
//				if((manager.TerrainAtLocation(x_grid-1,y_grid+1) >= 0)/* || (PathGrid[x_grid-1,y_grid+1] == 1)*/){
//					x_nav = x_grid - 1;
//					y_nav = y_grid + 1;
//				}
//			}
//		}
//		// ditto x+1 y-1
//		if (x_grid+ 1 < (manager.MAX_GRID)&& y_grid + 1 < (manager.MAX_GRID)) {
//			// if x+1 y-1 tile has lower cost and is pathable
//			if((PathGrid[x_grid+1,y_grid+1] < PathGrid[x_grid,y_grid]) && 
//			   (PathGrid[x_grid+1,y_grid+1] > 0) &&
//			   (PathGrid[x_grid,y_grid+1] >= 0) &&
//			   (PathGrid[x_grid+1,y_grid] >= 0)){
//				if((manager.TerrainAtLocation(x_grid+1,y_grid+1) >= 0)/* || (PathGrid[x_grid+1,y_grid+1] == 1)*/){
//					x_nav = x_grid + 1;
//					y_nav = y_grid + 1;
//				}
//			}
//		}
//		
//		if ((!(x_goal == x_grid) || !(y_goal == y_grid)) && ((lastXNav == x_nav) && (lastYNav == y_nav))) {
//			BuildPathTo (x_goal, y_goal);
//		} else {
//			lastXNav = x_nav; lastYNav = y_nav;
//			goalPos.x = (x_nav - manager.MAX_GRID / 2) * .25f;
//			goalPos.y = (y_nav - manager.MAX_GRID / 2) * .25f;
//		}
//	}
//
//	void BuildPathTo (int x, int y){
//		numOfCellsChanged = 1;
//		numOfCellsChecked = 0;
//
//		LinkedList<int[]> openCells = new LinkedList<int[]>();
//		RetrieveTerrainGrid ();
//		PathGrid [x, y] = STEP_COST;
//
//		if (x > 0) {
//			if(PathGrid[x-1,y] == 0){
//				openCells.AddLast(new int[] {(x-1),y});
//			}
//		}
//		if (x + 1 < (manager.MAX_GRID)) {
//			if(PathGrid[x+1,y] == 0){
//				openCells.AddLast(new int[] {(x+1),y});
//			}
//		}
//		if (y > 0) {
//			if(PathGrid[x,y-1] == 0){
//				openCells.AddLast(new int[] {x,(y-1)});
//			}
//		}
//		if (y + 1 < (manager.MAX_GRID)) {
//			if(PathGrid[x,y+1] == 0){
//				openCells.AddLast(new int[] {x,(y+1)});
//			}
//		}
//
//		bool foundMe = false;
//
//		while ((openCells.Count > 0) && (!foundMe)) {
//			int[] checkCell = openCells.First.Value;
//			openCells.RemoveFirst();
//
//			numOfCellsChecked++;
//			if(PathGrid[checkCell[0], checkCell[1]] == 0){
//				numOfCellsChanged++;
//				PathGrid[checkCell[0], checkCell[1]] = CostCell(ref openCells, checkCell[0], checkCell[1]);
//			}
//			if((checkCell[0] == x_grid) && (checkCell[1] == y_grid)){
//				foundMe = true;
//			}
//		}
//		if (!foundMe) {
//			AbandonJob();
//		}
//	}
//
//	int CostCell(ref LinkedList<int[]> openCells, int x, int y){
//		int bestcost = int.MaxValue;
//		// if x-1 is on grid
//		if (x > 0) {
//			// if x-1 has pathcost
//			if(PathGrid[x-1,y] > 0){
//				// if x-1 pathcost is better than current cost
//				if(PathGrid[x-1,y] < bestcost){
//					// make bestcost x-1 cost
//					bestcost = PathGrid[x-1,y];
//				}
//			} else {
//				// if x-1 has no pathcost
//				openCells.AddLast(new int[] {(x-1),y});
//			}
//		}
//		if (x + 1 < (manager.MAX_GRID)) {
//			if(PathGrid[x+1,y] > 0){
//				if(PathGrid[x+1,y] < bestcost){
//					bestcost = PathGrid[x+1,y];
//				}
//			} else {
//				// if x+1 has no pathcost
//				openCells.AddLast(new int[] {(x+1),y});
//			}
//		}
//		if (y > 0) {
//			if(PathGrid[x,y-1] > 0){
//				if(PathGrid[x,y-1] < bestcost){
//					bestcost = PathGrid[x,y-1];
//				}
//			} else {
//				// if y-1 has no pathcost
//				openCells.AddLast(new int[] {x,(y-1)});
//			}
//		}
//		if (y + 1 < (manager.MAX_GRID)) {
//			if(PathGrid[x,y+1] > 0){
//				if(PathGrid[x,y+1] < bestcost){
//					bestcost = PathGrid[x,y+1];
//				}
//			} else {
//				// if y+1 has no pathcost
//				openCells.AddLast(new int[] {x,(y+1)});
//			}
//		}
//
//
//		// give this cell bestcost + stepcost
//		return (bestcost + STEP_COST);
//	}
//}
