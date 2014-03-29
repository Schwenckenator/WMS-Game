using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
	public bool clickFlag = false;
	public LinkedList<JobClass> JobList;
	public int[,] TerrainGrid;
	//public int MAX_GRID = 38;
	const int MAX_GRID = 100;

	public Vector3 materialHealth;
	public GameObject[] TerrainTypes;
	public enum TerrainIndex{guy, rock, rockPiece, tree, ring, stockpile, stoneWall, gatheringZone, woodLog, woodWall, humanCorpse, food, drink,
		scrapMetal, metalPiece, metalWall, sleepZZZ, woodDoor
	};
	

	//Use these for all conversions and rounding pl0x

	//Takes a world point and returns the grid value
	public static int WorldToGrid(float input){
		return Mathf.RoundToInt(input*4 + MAX_GRID/2);
	}
	//Takes a grid number and returns the world position
	public static float GridToWorld(int input){
		return (input - MAX_GRID / 2) * .25f;
	}
	//Takes a world point and rounds it to a grid point
	public static float SnapToGrid(float input){
		return 0.25f * Mathf.RoundToInt(input*4);
	}
	public static int MaxGrid(){
		return MAX_GRID;
	}

	public static int IdentifyItem(string input){
		int output;
		switch(input){
		case "Food":
			output = (int)GameManager.TerrainIndex.food;
			break;
		case "Drink":
			output = (int)GameManager.TerrainIndex.drink;
			break;
		case "HumanCorpse":
			output = (int)GameManager.TerrainIndex.humanCorpse;
			break;
		default:
			Debug.LogError("ItemDetails() couldn't identify itself");
			output = -1;
			break;
		}
		return output;
	}
	//******************************************************************************************************
	//***************************** CODE STARTS HERE *******************************************************
	//******************************************************************************************************


	// Use this for initialization
	void Awake () {
		JobList = new LinkedList<JobClass>();
		TerrainGrid = new int[MAX_GRID,MAX_GRID];
		InitialiseTerrainGrid ();
		materialHealth = new Vector3 (1, 2, 5);
	}
	
	// Update is called once per frame
	void Update () {
		// Reorganise Job List
		LinkedList<JobClass> removeList, reAddList;
		removeList = new LinkedList<JobClass>();
		reAddList = new LinkedList<JobClass>();
		foreach (JobClass job in JobList) {
			if(job.complete){
				//Debug.Log ("Job complete");
				removeList.AddLast(job);
			} else if (job.tries < 1) {
				if(job.listed < 1){
					//Debug.Log ("Job Relisted too many times");
					removeList.AddLast(job);
				} else {
					reAddList.AddLast(job);
				}
			}
		}
		foreach (JobClass job in removeList) {
			RemoveJob(job);
		}
		foreach (JobClass job in reAddList) {
			//Debug.Log ("Job Re-added");
			RemoveJob(job);
			AddJob(job);
			job.MakeRing();
			job.ReList();
		}
		removeList.Clear ();
		reAddList.Clear ();

		//Middle Click for terrain information
		if (Input.GetMouseButtonDown (2)) {
			Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			pos.z = 0;
			int x_grid = GameManager.WorldToGrid(pos.x);
			int y_grid = GameManager.WorldToGrid(pos.y);
			Debug.Log ("Terrain Grid value at "+x_grid.ToString()+", "+y_grid.ToString()+" is: "+TerrainGrid[x_grid,y_grid].ToString());
		}
		if(Input.GetMouseButtonDown(1)){
			//Check for people
			Collider2D[] colliders = Physics2D.OverlapPointAll(Camera.main.ScreenToWorldPoint(Input.mousePosition));
			foreach(Collider2D hit in colliders){
				if(hit.CompareTag("Human")){
					Debug.Log ("Held Resources: "+hit.GetComponent<MobItems>().GetResources().ToString());
				}
			}
		}
		if(Input.GetKeyDown(KeyCode.F2)){
			foreach(JobClass job in JobList){
				Debug.Log (job.ToString());
			}
			Debug.Log ("Number of Jobs: "+JobList.Count.ToString());
		}
	}
	public void AddTerrain(GameObject obj, int x_grid, int y_grid){
		obj.GetComponent<TerrainDetails> ().SetGridPosition (x_grid, y_grid);
		TerrainGrid [x_grid, y_grid] = obj.GetComponent<TerrainDetails> ().PathingValue;
	}
	public void RemoveTerrain(int x, int y){
		TerrainGrid [x, y] = 0;
	}

	public int TerrainAtLocation(int x, int y){
		return TerrainGrid [x, y];
	}

	private void InitialiseTerrainGrid(){
		for (int x=0; x<MAX_GRID; x++) {
			for (int y=0; y<MAX_GRID; y++) {
				TerrainGrid[x,y] = 0;
			}
		}
	}
	public void AddJob(JobClass job){
		//Check for duplicate location on jobs
		bool isDuplicate = false;
		foreach(JobClass j in JobList){
			if(job.Equals(j)){
				isDuplicate = true;
			}
		}
		if(!isDuplicate){
			JobList.AddLast (job);
		}else{
			Destroy(job.GetSelectionRing());
		}
	}
	public void RemoveJob(JobClass job){
		//Debug.Log ("job removed!");
		Destroy(job.GetSelectionRing());
		JobList.Remove (job);
	}

}
