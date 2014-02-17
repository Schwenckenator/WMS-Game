using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MobItems : MonoBehaviour {
	private Vector3 resources;
	private Vector3 wantedRes; //Wanted resources
	private List<int> inventory;
	// Use this for initialization
	void Start () {
		inventory = new List<int>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public bool WantsResources(){
		return wantedRes.sqrMagnitude > 0;
	}

	public Vector3 GetWantedRes(){
		return wantedRes;
	}
	public void SetWantedRes(Vector3 res){
		wantedRes = res;
		CheckWantedResource();
	}
	public void AddWantedRes(Vector3 res){
		wantedRes += res;
		CheckWantedResource();
	}

	public void PickUpResources(Vector3 res){
		resources += res;
	}

	public Vector3 GetResources(){
		return resources;
	}

	public bool HasResources(){
		return resources.x > 0 ||resources.y > 0 ||resources.z > 0;
	}

	void CheckWantedResource(){
		if(wantedRes.x < 0){
			wantedRes.x = 0;
		}
		if(wantedRes.y < 0){
			wantedRes.y = 0;
		}
		if(wantedRes.z < 0){
			wantedRes.z = 0;
		}
	}

	// This will fail to drop anything if there is not enough
	// resource on mob
	public bool DropResources(Vector3 res){
		if(resources.x >= res.x && resources.y >= res.y && resources.z >= res.z){
			Drop (res);
			return true;
		}else{
			return false;
		}
	}

	public bool SpendResources(Vector3 res){
		if(resources.x >= res.x && resources.y >= res.y && resources.z >= res.z){
			resources -= res;
			return true;
		}else{
			return false;
		}
	}

	//This will drop all available resource if there is not enough
	// Returns the value of resources removed
	public Vector3 ForceDropResources(Vector3 res){
		if(resources.x >= res.x && resources.y >= res.y && resources.z >= res.z){
			Drop (res);
			return res;
		}else{
			float x = Mathf.Min(resources.x, res.x);
			float y = Mathf.Min(resources.y, res.y);
			float z = Mathf.Min(resources.z, res.z);

			Vector3 newRes = new Vector3 (x,y,z);

			Drop (newRes);
			return newRes;
		}
	}

	private void Drop(Vector3 res){
		//Create object here? Seems like a good idea
		// nope nope nope
		resources -= res;
	}

	//Inventory
	public void PickUpItem(int item){
		inventory.Add(item);
	}
	public void DropItemType(int item){
		inventory.Remove(item);
	}
	public int DropItemAtIndex(int index){
		int obj = inventory[index];
		inventory.RemoveAt(index);
		return obj;
	}
	public bool InventoryHasItem(){
		return inventory.Count > 0;
	}

}
