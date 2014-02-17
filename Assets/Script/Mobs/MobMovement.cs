using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MobMovement : MonoBehaviour {
	Vector3 goalPos;
	public bool walking;
	float speed = 0.02f;
	GameManager manager;
	MobBehaviour behaviour;
	private int STEP_COST = 1;
	public int[,] PathGrid;
	
	private int x_grid;
	private int y_grid;
	
	private int x_goal;
	private int y_goal;
	
	private int lastXNav;
	private int lastYNav;

	private int numOfCellsChecked = 0;
	private int numOfCellsChanged = 0;

	void Start () {
		behaviour = GetComponent<MobBehaviour>();
		manager = GameObject.FindGameObjectWithTag ("Manager").GetComponent<GameManager> ();
		PathGrid = new int[GameManager.MaxGrid(),GameManager.MaxGrid()];
		goalPos = transform.position;
		walking = false;
	}

	// Update is called once per frame
	void Update () {
	}

	public void SetGoal(int x, int y){
		x_goal = x;
		y_goal = y;
		if(!BuildPathTo(x, y)){
			Debug.Log ("Can't set goal properly");
			//behaviour.TakeStep();
		}
	}

	public void Step(){
		Vector3 path = goalPos - transform.position;
		if (path.magnitude < speed) {
			transform.position = goalPos;
		} else {
			path.Normalize ();
			transform.position += path * speed;
		}
		if (transform.position == goalPos) {
			walking = false;
		}
	}
	public int[] BoredStep(){
		//Find a direction that is pathable
		// Needs to return the space being moved into
		// Returns an array of 2 int, one for x-position and y position
		MyGridPos();

		int n = 8;
		int[] num = {0,1,2,3,4,5,6,7};

		int[] newPos = new int[2];

		while (n > 1){
			n--;
			int k = Random.Range(0,n+1);
			int value = num[k];
			num[k] = num[n];
			num[n] = value;
		}

		bool pathFound = false;
		foreach(int i in num){
			if(!pathFound){
				switch(i){
				case 0:
					if(x_grid > 0){
						if(manager.TerrainAtLocation(x_grid-1, y_grid) == 0){
							pathFound = true;
							//walking = true;

							newPos[0] = x_grid-1;
							newPos[1] = y_grid;
						}
					}
					break;
				case 1:
					if (y_grid > 0) {
						if(manager.TerrainAtLocation(x_grid, y_grid-1) == 0){
							pathFound = true;
							//walking = true;
							newPos[0] = x_grid;
							newPos[1] = y_grid-1;
						}
					}
					break;
				case 2:
					if (x_grid+ 1 < (GameManager.MaxGrid())) {
						if(manager.TerrainAtLocation(x_grid+1, y_grid) == 0){
							pathFound = true;
							//walking = true;
							newPos[0] = x_grid+1;
							newPos[1] = y_grid;
						}
					}
					break;
				case 3:
					if (y_grid + 1 < (GameManager.MaxGrid())) {
						if(manager.TerrainAtLocation(x_grid, y_grid+1) == 0){
							pathFound = true;
							//walking = true;
							newPos[0] = x_grid;
							newPos[1] = y_grid+1;
						}
					}
					break;
				case 4:
					if (x_grid > 0 && y_grid > 0) {
						if(manager.TerrainAtLocation(x_grid-1, y_grid-1) == 0){
							pathFound = true;
							//walking = true;
							//SetGoal(x_grid-1, y_grid-1);
							newPos[0] = x_grid-1;
							newPos[1] = y_grid-1;
						}
					}
					break;
				case 5:
					if (x_grid > 0 && y_grid + 1 < (GameManager.MaxGrid())) {
						if(manager.TerrainAtLocation(x_grid-1, y_grid+1) == 0){
							pathFound = true;
							//walking = true;
							//SetGoal(x_grid-1, y_grid+1);
							newPos[0] = x_grid-1;
							newPos[1] = y_grid+1;
						}
					}
					break;
				case 6:
					if (x_grid+ 1 < (GameManager.MaxGrid())&& y_grid > 0) {
						if(manager.TerrainAtLocation(x_grid+1, y_grid-1) == 0){
							pathFound = true;
							//walking = true;
							//SetGoal(x_grid+1, y_grid-1);
							newPos[0] = x_grid+1;
							newPos[1] = y_grid-1;
						}
					}
					break;
				case 7:
					if (x_grid+ 1 < (GameManager.MaxGrid())&& y_grid + 1 < (GameManager.MaxGrid())) {
						if(manager.TerrainAtLocation(x_grid+1, y_grid+1) == 0){
							pathFound = true;
							//walking = true;
							//SetGoal(x_grid+1, y_grid+1);
							newPos[0] = x_grid+1;
							newPos[1] = y_grid+1;
						}
					}
					break;
				}
			}
		}
		if(!pathFound){
			newPos[0] = -1;
			newPos[1] = -1;
		}

		return newPos;

	}
	public int StepsAwayFromGoal(){
		// On goal is 0 steps
		// Only counts Cardinal directions
		return (PathGrid [x_grid, y_grid] - 1);
	}
	public bool OnDiagonal(){
		return((x_grid -1 == x_goal || x_grid +1 == x_goal) && (y_grid -1 == y_goal || y_grid +1 == y_goal));
	}
	public bool OneStepAway(){
		MyGridPos();
		return((x_grid == x_goal || x_grid -1 == x_goal || x_grid+1 == x_goal) && (y_grid == y_goal || y_grid -1 == y_goal || y_grid +1 == y_goal));
	}


	public int GetXGoal(){
		return x_goal;
	}
	public int GetYGoal(){
		return y_goal;
	}
	
	void MyGridPos(){
		//find my position on grid
		x_grid = GameManager.WorldToGrid(transform.position.x);
		y_grid = GameManager.WorldToGrid(transform.position.y);
	}
	
	void RetrieveTerrainGrid(){ // Gets the Terrain grid from the manager and assigns the value to the mob grid
		for (int i = 0; i < GameManager.MaxGrid(); i++) {
			for( int j=0; j < GameManager.MaxGrid(); j++){
				PathGrid[i,j] = manager.TerrainGrid[i,j];
			}
		}
	}

	public bool GetGoal(){
		walking = true;
		bool foundGoal = true;
		MyGridPos ();
		int x_nav = x_grid, y_nav = y_grid;

		
		//---------------------------- Normal ----------------------------------------------------------------------
		
		// if x-1 is on grid
		if (x_grid > 0) {
			// if x-1 y-1 tile has lower cost and is pathable
			if((PathGrid[x_grid-1,y_grid] < PathGrid[x_grid,y_grid]) && (PathGrid[x_grid-1,y_grid] > 0)){
				//set x-1 as target tile
				if((manager.TerrainAtLocation(x_grid-1,y_grid) >= 0)){
					x_nav = x_grid - 1;
					y_nav = y_grid;
				}
			}
		}
		
		// ditto x+1
		if (x_grid+ 1 < (GameManager.MaxGrid())) {
			if((PathGrid[x_grid+1,y_grid] < PathGrid[x_grid,y_grid]) && (PathGrid[x_grid+1,y_grid] > 0)){
				if((manager.TerrainAtLocation(x_grid+1,y_grid) >= 0)){
					x_nav = x_grid + 1;
					y_nav = y_grid;
				}
			}
		}
		
		// ditto y-1
		if (y_grid > 0) {
			if((PathGrid[x_grid,y_grid-1] < PathGrid[x_grid,y_grid]) && (PathGrid[x_grid,y_grid-1] > 0)){
				if((manager.TerrainAtLocation(x_grid,y_grid-1) >= 0)){
					x_nav = x_grid;
					y_nav = y_grid - 1;
				}
			}
		}
		// ditto y+1
		if (y_grid + 1 < (GameManager.MaxGrid())) {
			if((PathGrid[x_grid,y_grid+1] < PathGrid[x_grid,y_grid]) && (PathGrid[x_grid,y_grid+1] > 0)){
				if((manager.TerrainAtLocation(x_grid,y_grid+1) >= 0)){
					x_nav = x_grid;
					y_nav = y_grid + 1;
				}
			}
		}
		
		/*------------------------ DIAGONALS --------------------------------------------*/
		
		// ditto x-1 y-1
		if (x_grid > 0 && y_grid > 0) {
			// if x-1 tile has lower cost and is pathable
			if((PathGrid[x_grid-1,y_grid-1] < PathGrid[x_grid,y_grid]) &&
			   (PathGrid[x_grid-1,y_grid-1] > 0) &&
			   (PathGrid[x_grid,y_grid-1] >= 0) &&
			   (PathGrid[x_grid-1,y_grid] >= 0)){
				if((manager.TerrainAtLocation(x_grid-1,y_grid-1) >= 0)/* || (PathGrid[x_grid-1,y_grid-1] == 1)*/){
					x_nav = x_grid - 1;
					y_nav = y_grid - 1;
				}
			}
		}
		// ditto x+1 y-1
		if (x_grid+ 1 < (GameManager.MaxGrid())&& y_grid > 0) {
			// if x+1 y-1 tile has lower cost and is pathable
			if((PathGrid[x_grid+1,y_grid-1] < PathGrid[x_grid,y_grid]) && 
			   (PathGrid[x_grid+1,y_grid-1] > 0) &&
			   (PathGrid[x_grid,y_grid-1] >= 0) &&
			   (PathGrid[x_grid+1,y_grid] >= 0)){
				if((manager.TerrainAtLocation(x_grid+1,y_grid-1) >= 0)/* || (PathGrid[x_grid+1,y_grid-1] == 1)*/){
					x_nav = x_grid + 1;
					y_nav = y_grid - 1;
				}
			}
		}
		// ditto x-1 y+1
		if (x_grid > 0 && y_grid + 1 < (GameManager.MaxGrid())) {
			// if x+1 y-1 tile has lower cost and is pathable
			if((PathGrid[x_grid-1,y_grid+1] < PathGrid[x_grid,y_grid]) && 
			   (PathGrid[x_grid-1,y_grid+1] > 0) &&
			   (PathGrid[x_grid,y_grid+1] >= 0) &&
			   (PathGrid[x_grid-1,y_grid] >= 0)){
				if((manager.TerrainAtLocation(x_grid-1,y_grid+1) >= 0)/* || (PathGrid[x_grid-1,y_grid+1] == 1)*/){
					x_nav = x_grid - 1;
					y_nav = y_grid + 1;
				}
			}
		}
		// ditto x+1 y-1
		if (x_grid+ 1 < (GameManager.MaxGrid())&& y_grid + 1 < (GameManager.MaxGrid())) {
			// if x+1 y-1 tile has lower cost and is pathable
			if((PathGrid[x_grid+1,y_grid+1] < PathGrid[x_grid,y_grid]) && 
			   (PathGrid[x_grid+1,y_grid+1] > 0) &&
			   (PathGrid[x_grid,y_grid+1] >= 0) &&
			   (PathGrid[x_grid+1,y_grid] >= 0)){
				if((manager.TerrainAtLocation(x_grid+1,y_grid+1) >= 0)/* || (PathGrid[x_grid+1,y_grid+1] == 1)*/){
					x_nav = x_grid + 1;
					y_nav = y_grid + 1;
				}
			}
		}
		
		if ((!(x_goal == x_grid) || !(y_goal == y_grid)) && ((lastXNav == x_nav) && (lastYNav == y_nav))) {
			foundGoal = BuildPathTo(x_goal, y_goal);
		} else {
			lastXNav = x_nav; lastYNav = y_nav;
			goalPos.x = GameManager.GridToWorld(x_nav);
			goalPos.y = GameManager.GridToWorld(y_nav);
			walking = true;
		}
		return foundGoal;
	}
	
	public bool BuildPathTo (int x, int y){
		numOfCellsChanged = 1;
		numOfCellsChecked = 0;
		
		LinkedList<int[]> openCells = new LinkedList<int[]>();
		RetrieveTerrainGrid ();
		PathGrid [x, y] = STEP_COST;
		
		if (x > 0) {
			if(PathGrid[x-1,y] == 0){
				openCells.AddLast(new int[] {(x-1),y});
			}
		}
		if (x + 1 < (GameManager.MaxGrid())) {
			if(PathGrid[x+1,y] == 0){
				openCells.AddLast(new int[] {(x+1),y});
			}
		}
		if (y > 0) {
			if(PathGrid[x,y-1] == 0){
				openCells.AddLast(new int[] {x,(y-1)});
			}
		}
		if (y + 1 < (GameManager.MaxGrid())) {
			if(PathGrid[x,y+1] == 0){
				openCells.AddLast(new int[] {x,(y+1)});
			}
		}
		
		bool foundMe = false;
		
		while ((openCells.Count > 0) && (!foundMe)) {
			int[] checkCell = openCells.First.Value;
			openCells.RemoveFirst();
			
			numOfCellsChecked++;
			if(PathGrid[checkCell[0], checkCell[1]] == 0){
				numOfCellsChanged++;
				PathGrid[checkCell[0], checkCell[1]] = CostCell(ref openCells, checkCell[0], checkCell[1]);
			}
			if((checkCell[0] == x_grid) && (checkCell[1] == y_grid)){
				foundMe = true;
			}
		}
		return foundMe;
	}
	
	int CostCell(ref LinkedList<int[]> openCells, int x, int y){
		int bestcost = int.MaxValue;
		// if x-1 is on grid
		if (x > 0) {
			// if x-1 has pathcost
			if(PathGrid[x-1,y] > 0){
				// if x-1 pathcost is better than current cost
				if(PathGrid[x-1,y] < bestcost){
					// make bestcost x-1 cost
					bestcost = PathGrid[x-1,y];
				}
			} else {
				// if x-1 has no pathcost
				openCells.AddLast(new int[] {(x-1),y});
			}
		}
		if (x + 1 < (GameManager.MaxGrid())) {
			if(PathGrid[x+1,y] > 0){
				if(PathGrid[x+1,y] < bestcost){
					bestcost = PathGrid[x+1,y];
				}
			} else {
				// if x+1 has no pathcost
				openCells.AddLast(new int[] {(x+1),y});
			}
		}
		if (y > 0) {
			if(PathGrid[x,y-1] > 0){
				if(PathGrid[x,y-1] < bestcost){
					bestcost = PathGrid[x,y-1];
				}
			} else {
				// if y-1 has no pathcost
				openCells.AddLast(new int[] {x,(y-1)});
			}
		}
		if (y + 1 < (GameManager.MaxGrid())) {
			if(PathGrid[x,y+1] > 0){
				if(PathGrid[x,y+1] < bestcost){
					bestcost = PathGrid[x,y+1];
				}
			} else {
				// if y+1 has no pathcost
				openCells.AddLast(new int[] {x,(y+1)});
			}
		}
		
		
		// give this cell bestcost + stepcost
		return (bestcost + STEP_COST);
	}
}
