using UnityEngine;
using System.Collections;

public class GUIScript : MonoBehaviour {
	/* 
		This script should hold everything to do with the GUI. I don't want little bits of it everywhere, it'd become such a mess
	 */
	private SpawnRocks spawnRocks;
	private GameManager manager;
	private DragSelect dragSelect;
	void Start(){
		spawnRocks = GetComponent<SpawnRocks>();
		manager = GetComponent<GameManager>();
		dragSelect= GetComponent<DragSelect>();
	}
	void OnGUI(){
		// A lot of this is magic numbers right now, should probably change that soon -Matt
		// Have to make it so stuff moves around with resizing/ different resolutions

		int buttonYpos = 100;
		int diffY = 40;
		int buttonNum = 0;

		GUI.Box (new Rect(10,10,200, 560), "Debug Menu");
		//Make a background Box
		GUI.Label(new Rect(30,40,160,60),"For your own safety, don't overfill space. Will ∞ loop");
		//Make the first button, if it is pressed, shit 10 rocks
		if(GUI.Button(new Rect(30,buttonYpos+diffY*buttonNum++,160,20), "Shit Rocks(10)")){
			spawnRocks.ShitTerrain(10, manager.TerrainTypes[ (int)GameManager.TerrainIndex.rock ]);
		}
		if(GUI.Button(new Rect(30,buttonYpos+diffY*buttonNum++,160,20), "Shit Rocks(100)")){
			spawnRocks.ShitTerrain(100, manager.TerrainTypes[ (int)GameManager.TerrainIndex.rock]);
		}
		if(GUI.Button(new Rect(30,buttonYpos+diffY*buttonNum++,160,20), "Shit Trees(10)")){
			spawnRocks.ShitTerrain(10, manager.TerrainTypes[ (int)GameManager.TerrainIndex.tree]);
		}
		if(GUI.Button(new Rect(30,buttonYpos+diffY*buttonNum++,160,20), "Shit Dude(1)")){
			spawnRocks.ShitDude(1);
		}
		if(GUI.Button(new Rect(30,buttonYpos+diffY*buttonNum++,160,20), "Shit FoodDrink(10)")){
			spawnRocks.ShitFoodDrink(10);
		}
		if(GUI.Button(new Rect(30, buttonYpos+diffY*buttonNum++, 160, 20), "Mine")){
			// Change drag select to mine
			dragSelect.SetSelectionType("Mine");
		}
		if(GUI.Button(new Rect(30, buttonYpos+diffY*buttonNum++, 160, 20), "Chop")){
			// Change drag select to mine
			dragSelect.SetSelectionType("Chop");
			
		}
		if(GUI.Button (new Rect(30, buttonYpos+diffY*buttonNum++, 160, 20), "Build Stone Wall")){
			// Change draf select to build
			dragSelect.SetSelectionType("BuildRock");
		}
		if(GUI.Button (new Rect(30, buttonYpos+diffY*buttonNum++, 160, 20), "Build Wooden Wall")){
			// Change draf select to build
			dragSelect.SetSelectionType("BuildWood");
		}
		if(GUI.Button (new Rect(30, buttonYpos+diffY*buttonNum++, 160, 20), "Build Metal Wall")){
			// Change draf select to build
			dragSelect.SetSelectionType("BuildMetal");
		}
		if(GUI.Button (new Rect(30, buttonYpos+diffY*buttonNum++, 160, 20), "Stockpile")){
			dragSelect.SetSelectionType("Stockpile");
		}
		if(GUI.Button (new Rect(30, buttonYpos+diffY*buttonNum++, 160, 20), "Gathering Zone")){
			dragSelect.SetSelectionType("GatheringZone");
		}



	}
}
