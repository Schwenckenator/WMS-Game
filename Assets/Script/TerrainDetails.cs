using UnityEngine;
using System.Collections;

public class TerrainDetails : MonoBehaviour {

	private GameManager manager;
	private int x_grid;
	private int y_grid;

	private int maxHealth;
	private int health;

	private Vector3 resources;

	public int pathingValue;
	public string materialType;

	// Use this for initialization
	void Start () {
		ConnectionInitialise();
	}


	public void Initialise(Vector3 mats){
		resources = mats;
		ConnectionInitialise();
		HealthInitialise ();
		TypeInitialise();
	}
	public void Initialise(float woodMat, float stoneMat, float metalMat){
		resources = new Vector3 (woodMat, stoneMat, metalMat);
		ConnectionInitialise();
		HealthInitialise ();
		TypeInitialise();
	}


	void ConnectionInitialise(){
		manager = GameObject.FindGameObjectWithTag ("Manager").GetComponent<GameManager> ();
	}
	void HealthInitialise(){
		maxHealth = Mathf.RoundToInt(resources.x * manager.materialHealth.x + resources.y * manager.materialHealth.y + resources.z * manager.materialHealth.z);
		health = maxHealth;
	}
	void TypeInitialise(){
		if((resources.x > resources.y) && (resources.x > resources.z)){
			materialType = "Wood";
		}else if((resources.y > resources.x) && (resources.y > resources.z)){
			materialType = "Rock";
		}else{
			materialType = "Metal";
		}
	}

	public void DestroyTerrain(){
		//Tell grid you an hero
		manager.RemoveTerrain (x_grid, y_grid);

		//Make an item with resources
		GameObject newItem = new GameObject(); // Will crash if something goes wrong lol
		if(materialType == "Rock"){
			newItem = Instantiate(manager.TerrainTypes[(int)GameManager.TerrainIndex.rockPiece], transform.position, transform.rotation) as GameObject;
		}else if(materialType == "Wood"){
			newItem = Instantiate(manager.TerrainTypes[(int)GameManager.TerrainIndex.woodLog], transform.position, transform.rotation) as GameObject;
		}else if(materialType == "Metal"){
			newItem = Instantiate(manager.TerrainTypes[(int)GameManager.TerrainIndex.metalPiece], transform.position, transform.rotation) as GameObject;
		}
		newItem.GetComponent<BuildingResourceDetails>().Initialise(resources);

		JobClass haulJob = ScriptableObject.CreateInstance<JobClass>();
		haulJob.Initialise("Haul", transform.position, false);
		manager.AddJob(haulJob);
		// An hero
		Destroy (gameObject);
	}

	public void AnHero(){
		//Tell grid you an hero
		manager.RemoveTerrain (x_grid, y_grid);
		// An hero
		Destroy (gameObject);
	}

	public void DamageTerrain(int dam){
		health -= dam;
		HealthCheck();

	}
	public void RepairTerrain(int repair){
		health += repair;
		HealthCheck();
	}

	private void HealthCheck(){
		//Check for death
		if (health <= 0) {
			DestroyTerrain ();
		} else if (health > maxHealth) {
			health = maxHealth;
		}
	}

	public int PathingValue {
		get {
			return pathingValue;
		}
		set {
			pathingValue = value;
		}
	}
	public void SetGridPosition(int x, int y){
		x_grid = x;
		y_grid = y;
	}
	public int[] GetGridPosition(){
		int[] temp = {x_grid,y_grid};
		return temp;
	}

	public string Material {
		get {
			return materialType;
		}
		set {
			materialType = value;
		}
	}
}
