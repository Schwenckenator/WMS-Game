using UnityEngine;
using System.Collections;

public class ZoneDetails : MonoBehaviour {
	//GameManager manager;
	
	int xLowGrid;
	int xHighGrid;
	int yLowGrid;
	int yHighGrid;
	//int xDiff; // X difference
	//int yDiff; // Y difference
	
	// Use this for initialization
	void Start () {
		//	manager = GameObject.FindGameObjectWithTag ("Manager").GetComponent<GameManager> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	private void OutofBounds(){
		//If one of the edges is off the grid, kill self
		bool anHero = false;
		if(xLowGrid < 0 || xLowGrid >= GameManager.MaxGrid() ||
		   yLowGrid < 0 || yLowGrid >= GameManager.MaxGrid() ||
		   xHighGrid < 0 || xHighGrid >= GameManager.MaxGrid() ||
		   yHighGrid < 0 || yHighGrid >= GameManager.MaxGrid())
		{
			anHero = true;
		}
		if(anHero){
			Destroy(gameObject);
		}
	}
	
	public void Initialise(int xLow, int xHigh, int yLow, int yHigh){
		xLowGrid = xLow;
		xHighGrid = xHigh;
		yLowGrid = yLow;
		yHighGrid = yHigh;
		
		OutofBounds();
	}
	
	public int[] GetLowestGridPosition(){
		int[] temp = new int[2];
		temp[0] = xLowGrid;
		temp[1] = yLowGrid;
		return temp;
	}
	
	public int[] GetFreeSpace(){
		// Set up loops to loop through grid space
		bool found = false;
		int freeX = -1, freeY = -1;
		
		for(int i=xLowGrid; i< xHighGrid; i++){
			if(found) break; // Once the spot is found you don't need to keep looping
			for(int j=yLowGrid; j< yHighGrid; j++){
				if(found) break; // See above
				// overlap circle finding item?
				Collider2D[] hits = Physics2D.OverlapPointAll(new Vector2(GameManager.GridToWorld(i), GameManager.GridToWorld(j)));
				bool hitItem = false;
				foreach(Collider2D hit in hits){
					//Debug.Log (hit.ToString());
					if(hit.CompareTag("Item") || 
					   hit.CompareTag("Rock") ||  
					   hit.CompareTag("Wood") ||  
					   hit.CompareTag("Metal") || 
					   hit.CompareTag("Resource")){
						hitItem = true;
					}
				}
				if(!hitItem){
					found = true;
					freeX = i;
					freeY = j;
				}
			}
		}
		int[] temp = new int[2];
		temp[0] = freeX;
		temp[1] = freeY;
		return temp;
	}
	public int[] GetRandomSpace(){
		
		int randX, randY;
		
		randX = Random.Range(xLowGrid, xHighGrid);
		randY = Random.Range(yLowGrid, yHighGrid);
		
		int[] temp = new int[2];
		temp[0] = randX;
		temp[1] = randY;
		return temp;
	}
}
