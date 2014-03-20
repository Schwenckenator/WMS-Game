using UnityEngine;
using System.Collections; 

public class SpawnRocks : MonoBehaviour {

	private GameManager manager;
	// Use this for initialization
	void Start () {
		manager = GetComponent<GameManager> ();
		SpawnStartingTerrain();
	}

	void SpawnStartingTerrain(){
		ShitTerrain(500, manager.TerrainTypes[ (int)GameManager.TerrainIndex.rock]);
		ShitTerrain(100, manager.TerrainTypes[ (int)GameManager.TerrainIndex.tree]);
		ShitTerrain(100, manager.TerrainTypes[ (int)GameManager.TerrainIndex.scrapMetal]);
		//ShitFoodDrink(15);
	}

	public void ShitTerrain(int num, GameObject terrain){
		for(int i=0; i<num; i++){
			bool NoPosition = true;
			int x;
			int y;
			do{
				x = Random.Range(0, GameManager.MaxGrid());
				y = Random.Range(0, GameManager.MaxGrid());
				if(manager.TerrainAtLocation(x,y) == 0){
					NoPosition = false;
				}
			}while(NoPosition);


			Vector3 pos = new Vector3( GameManager.GridToWorld(x), GameManager.GridToWorld(y), 0);
			GameObject newTerrain;
			newTerrain = Instantiate(terrain, pos, new Quaternion()) as GameObject;
			if(newTerrain.CompareTag("Rock")){
				newTerrain.GetComponent<TerrainDetails>().Initialise(new Vector3(0.0f, 50.0f, 0.0f));
			}else if(newTerrain.CompareTag("Wood")){
				newTerrain.GetComponent<TerrainDetails>().Initialise(new Vector3(50.0f, 0.0f, 0.0f));
			}else if(newTerrain.CompareTag("Metal")){
				newTerrain.GetComponent<TerrainDetails>().Initialise(new Vector3(0.0f, 0.0f, 50.0f));
			}


			manager.AddTerrain(newTerrain, x, y);
		}
	}
	public void ShitDude(int num){ // 		<- Might need to refactor this method into ShitTerrain
		for(int i=0; i<num; i++){
			bool NoPosition = true;
			int x;
			int y;
			do{
				x = Random.Range(0, GameManager.MaxGrid());
				y = Random.Range(0, GameManager.MaxGrid());
				if(manager.TerrainAtLocation(x,y) == 0){
					NoPosition = false;
				}
			}while(NoPosition);
			
			
			Vector3 pos = new Vector3( GameManager.GridToWorld(x), GameManager.GridToWorld(y), 0);
			Instantiate(manager.TerrainTypes[(int) GameManager.TerrainIndex.guy ], pos, new Quaternion());

		}
	}
	public void ShitFoodDrink(int num){
		for(int i=0; i<num; i++){
			bool NoPosition = true;
			int x;
			int y;
			do{
				x = Random.Range(0, GameManager.MaxGrid());
				y = Random.Range(0, GameManager.MaxGrid());
				if(manager.TerrainAtLocation(x,y) == 0){
					NoPosition = false;
				}
			}while(NoPosition);
			
			
			Vector3 pos = new Vector3( GameManager.GridToWorld(x), GameManager.GridToWorld(y), 0);
			//GameObject newFoodDrink;
			if(i%3 == 0){
				//newFoodDrink = Instantiate(manager.TerrainTypes[(int) GameManager.TerrainIndex.food ], pos, new Quaternion()) as GameObject;
				//newFoodDrink.GetComponent<ItemDetails>().Initialise(true, 30, false, 0);

				Instantiate(manager.TerrainTypes[(int) GameManager.TerrainIndex.food ], pos, new Quaternion());
			}else{
				//newFoodDrink = Instantiate(manager.TerrainTypes[(int) GameManager.TerrainIndex.drink ], pos, new Quaternion()) as GameObject;
				//newFoodDrink.GetComponent<ItemDetails>().Initialise(false, 0, true,30);

				Instantiate(manager.TerrainTypes[(int) GameManager.TerrainIndex.drink ], pos, new Quaternion());
			}


			JobClass haulJob = ScriptableObject.CreateInstance<JobClass>();
			haulJob.Initialise("Haul", pos, false);
			manager.AddJob(haulJob);
		}
	}
}
