using UnityEngine;
using System.Collections;

public class BuildingResourceDetails : MonoBehaviour {
	private Vector3 resources;
	private int x_grid;
	private int y_grid;
	public bool assigned;


	void Start () {
		assigned = false;
	}
	public void Initialise(float wood, float stone, float metal){
		resources.x = wood;
		resources.y = stone;
		resources.z = metal;
		PrivateInit();
	}
	public void Initialise(Vector3 resIn){
		resources = resIn;
		PrivateInit();
	}

	void PrivateInit(){
		x_grid = GameManager.WorldToGrid(transform.position.x);
		y_grid = GameManager.WorldToGrid(transform.position.y);

	}    
	// Update is called once per frame
	void Update () {
	
	}
	public Vector3 GetResources(){
		return resources;
	}

	//Remove resources from item
	// returns true if possible, changes resource amount
	// returns false if not possible and no change
	public bool TakeResource(float wood, float stone, float metal){
		if(wood <= resources.x && stone <= resources.y && metal <= resources.z){
			Vector3 res = new Vector3(wood, stone, metal);
			RemoveResource(res);
			return true;
		}else{
			return false;
		}
	}
	public bool TakeResource(Vector3 resOut){
		if(resOut.x <= resources.x && resOut.y <= resources.y && resOut.z <= resources.z){
			RemoveResource(resOut);
			return true;
		}else{
			return false;
		}
	}
	// I think this method is better anyway
	public Vector3 ForceTakeResource(Vector3 resOut){
		if(resOut.x <= resources.x && resOut.y <= resources.y && resOut.z <= resources.z){
			RemoveResource(resOut);

			return resOut;
		}else{
			float x = Mathf.Min(resources.x, resOut.x);
			float y = Mathf.Min(resources.y, resOut.y);
			float z = Mathf.Min(resources.z, resOut.z);
			
			Vector3 newRes = new Vector3 (x,y,z);
			
			RemoveResource (newRes);
			Debug.Log (resources.ToString());
			return newRes;
		}
	}

	public Vector3 TakeAllResource(){
		Vector3 oldRes = resources;
		RemoveResource (resources);
		return oldRes;
	}

	void RemoveResource(Vector3 res){
		resources -= res;
		CheckForRemoval();
	}

	public int[] GetGridPosition(){
		int[] temp = {x_grid, y_grid};
		return temp;
	}

	void CheckForRemoval(){
		// If there are no resources here, delete object
		if(resources.sqrMagnitude == 0.0f){
			Destroy (gameObject);
		}
	}

}
