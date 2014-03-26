using UnityEngine;
using System.Collections;

public class GUIScript : MonoBehaviour {
	/* 
		This script should hold everything to do with the GUI. I don't want little bits of it everywhere, it'd become such a mess
	 */
	private SpawnRocks spawnRocks;
	private GameManager manager;
	private DragSelect dragSelect;

	private int sWidth;
	private int sHeight;

	private int selectedWindow = 0;

	Rect ToolbarWindow;
	Rect JobsWindow;
	Rect ZonesWindow;
	Rect DebugWindow;

	void Start(){
		spawnRocks = GetComponent<SpawnRocks>();
		manager = GetComponent<GameManager>();
		dragSelect= GetComponent<DragSelect>();

		sWidth = Screen.width;
		sHeight = Screen.height;

		ToolbarWindow = new Rect (10, 10, sWidth - 230, 50);
		JobsWindow = new Rect (10, 10, 200, sHeight-20);
		ZonesWindow = new Rect (10, 10, 200, sHeight-20);
		DebugWindow = new Rect (10, 10, 200, sHeight-20);
	}
	void OnGUI(){

		if(selectedWindow == 0 ){
			GUI.Window(0,ToolbarWindow, Toolbar, "");
		}else if(selectedWindow == 1){
			GUI.Window(1,JobsWindow, JobsGUI, "");
		}else if(selectedWindow == 2){
			GUI.Window (2,ZonesWindow, ZonesGUI, "");
		}else if(selectedWindow == 3){
			GUI.Window (3,DebugWindow, DebugGUI, "");
		}


		//New job list
		GUI.Box (new Rect(sWidth - 210,10,200, sHeight-20), "Debug\nJobs List");
		int basePos = 40;
		int posDelta = 20;
		int index = 0;
		int extraJobs = 0;
		foreach(JobClass job in manager.JobList){


			if(basePos+posDelta*index < sHeight-60){
				GUI.Label (new Rect(sWidth - 200, basePos + posDelta * index, 180, 20), job.JobName + ", " + job.JobDetails + ", " + job.JobLocation.ToString());
				index++;
			}else{
				extraJobs++;
			}
		}
		if(extraJobs > 0){
			GUI.Label (new Rect(sWidth - 200, sHeight-40, 180, 20), extraJobs.ToString() + " undisplayed jobs.");
		}


	}
	//************************************************************************************************
	//**************************************** Window Options ****************************************
	//************************************************************************************************
	void Toolbar(int windowId){
		int selected = 0;
		string[] content = {"Jobs", "Zones", "Debug"};
		selected = GUI.Toolbar(new Rect(10,10,300, 30),selected, content);
		if(GUI.changed){
			selectedWindow = ++selected;
		}
	}

	void JobsGUI(int windowId){
		int buttonYpos = 40;
		int diffY = 40;
		int buttonNum = 0;

		if(GUI.Button(new Rect(20, buttonYpos+diffY*buttonNum++, 160, 20), "Mine")){
			// Change drag select to mine
			dragSelect.SetSelectionType("Mine");
		}
		if(GUI.Button(new Rect(20, buttonYpos+diffY*buttonNum++, 160, 20), "Chop")){
			// Change drag select to mine
			dragSelect.SetSelectionType("Chop");
			
		}
		if(GUI.Button (new Rect(20, buttonYpos+diffY*buttonNum++, 160, 20), "Build Stone Wall")){
			// Change draf select to build
			dragSelect.SetSelectionType("BuildRock");
		}
		if(GUI.Button (new Rect(20, buttonYpos+diffY*buttonNum++, 160, 20), "Build Wooden Wall")){
			// Change draf select to build
			dragSelect.SetSelectionType("BuildWood");
		}
		if(GUI.Button (new Rect(20, buttonYpos+diffY*buttonNum++, 160, 20), "Build Metal Wall")){
			// Change draf select to build
			dragSelect.SetSelectionType("BuildMetal");
		}

		if(GUI.Button (new Rect(20, buttonYpos+diffY*buttonNum++, 160, 20), "Cancel Job")){
			dragSelect.SetSelectionType("CancelJob");
		}

		//Back button goes last always
		if(GUI.Button(new Rect(20,buttonYpos+diffY*buttonNum++,160,20), "Back")){
			selectedWindow = 0;
		}
	}
	void ZonesGUI(int windowId){
		int buttonYpos = 40;
		int diffY = 40;
		int buttonNum = 0;

		if(GUI.Button (new Rect(20, buttonYpos+diffY*buttonNum++, 160, 20), "Stockpile")){
			dragSelect.SetSelectionType("Stockpile");
		}
		if(GUI.Button (new Rect(20, buttonYpos+diffY*buttonNum++, 160, 20), "Gathering Zone")){
			dragSelect.SetSelectionType("GatheringZone");
		}
		
		if(GUI.Button (new Rect(20, buttonYpos+diffY*buttonNum++, 160, 20), "Delete Zone")){
			dragSelect.SetSelectionType("DeleteZone");
		}

		//Back button goes last always
		if(GUI.Button(new Rect(20,buttonYpos+diffY*buttonNum++,160,20), "Back")){
			selectedWindow = 0;
		}
	}
	void DebugGUI(int windowId){
		int buttonYpos = 40;
		int diffY = 40;
		int buttonNum = 0;

		if(GUI.Button(new Rect(20,buttonYpos+diffY*buttonNum++,160,20), "Shit Rocks(10)")){
			spawnRocks.ShitTerrain(10, manager.TerrainTypes[ (int)GameManager.TerrainIndex.rock ]);
		}
		if(GUI.Button(new Rect(20,buttonYpos+diffY*buttonNum++,160,20), "Shit Rocks(100)")){
			spawnRocks.ShitTerrain(100, manager.TerrainTypes[ (int)GameManager.TerrainIndex.rock]);
		}
		if(GUI.Button(new Rect(20,buttonYpos+diffY*buttonNum++,160,20), "Shit Trees(10)")){
			spawnRocks.ShitTerrain(10, manager.TerrainTypes[ (int)GameManager.TerrainIndex.tree]);
		}
		if(GUI.Button(new Rect(20,buttonYpos+diffY*buttonNum++,160,20), "Shit Dude(1)")){
			spawnRocks.ShitDude(1);
		}
		if(GUI.Button(new Rect(20,buttonYpos+diffY*buttonNum++,160,20), "Shit FoodDrink(10)")){
			spawnRocks.ShitFoodDrink(10);
		}

		//Back button goes last always
		if(GUI.Button(new Rect(20,buttonYpos+diffY*buttonNum++,160,20), "Back")){
			selectedWindow = 0;
		}
	}

}
