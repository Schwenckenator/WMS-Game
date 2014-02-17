using UnityEngine;
using System.Collections;

public class GatheringZoneDetails : MonoBehaviour {
	int xLowGrid;
	int xHighGrid;
	int yLowGrid;
	int yHighGrid;

	// Use this for initialization
	void Start () {
	
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
