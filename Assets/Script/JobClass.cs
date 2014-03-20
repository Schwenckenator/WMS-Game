using UnityEngine;
using System.Collections;

public class JobClass : ScriptableObject{

	private GameManager manager;
	private int MAX_TRIES = 1;
	private int MAX_RELIST = 10;

	private string jobName;
	private Vector2 jobLocation;
	private GameObject selectionRing;
	private string jobDetails; // Additional Details about the job
	public int tries;
	public int listed;
	public bool assigned;
	public bool complete;
	public bool reorder;
	public bool personal;
	
	public void Initialise(string name, Vector2 loc, bool ring){
		jobName = name;
		jobLocation = loc;
		tries = MAX_TRIES;
		listed = MAX_RELIST;
		assigned = false;
		complete = false;
		manager = GameObject.FindGameObjectWithTag ("Manager").GetComponent<GameManager> ();
		if(ring){
			MakeRing();
		}
	}

	public string JobName {
		get {
			return jobName;
		}
		set {
			jobName = value;
		}
	}

	public Vector2 JobLocation {
		get {
			return jobLocation;
		}
		set {
			jobLocation = value;
		}
	}

	public string JobDetails {
		get {
			return jobDetails;
		}
		set {
			jobDetails = value;
		}
	}

	public void TakeJob(){
		assigned = true;
	}

	public void ReturnJob(){
		assigned = false;
		tries--;
		//Debug.Log ("Job Returned, " +tries.ToString() + " attempts remaining");
	}

	public void ReList(){
		tries = MAX_TRIES;
		listed--;
		//Debug.Log ("Job Relisted, " +listed.ToString() + " attempts remaining");
	}

	public override string ToString ()
	{
		return string.Format ("[JobClass: JobName={0}, JobLocation={1}]", JobName, JobLocation);
	}

	public GameObject GetSelectionRing(){
		return selectionRing;
	}

	public bool Equals(JobClass job){
		if(this.jobName == job.JobName && this.jobLocation == job.JobLocation){
			return true;
		}else{
			return false;
		}
	}
	public bool IsIdle(){
		return jobName == "Idle";
	}

	public void MakeRing(){
		selectionRing = Instantiate(manager.TerrainTypes[(int)GameManager.TerrainIndex.ring], jobLocation, new Quaternion()) as GameObject;
		//Tell the ring what job it's connected to
		selectionRing.GetComponent<SelectionRingDetails>().SetJob(this);
	}
}
