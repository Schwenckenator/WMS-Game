using UnityEngine;
using System.Collections;

public class MobBehaviour : MonoBehaviour {
	//Connect to MobJobs and Mob Movement
	private MobJobs jobs;
	private MobMovement move;
	//GameManager manager;

	private int MAX_BORED = 100;
	private int bored;
	private int MAX_RELAX = 10;
	private int MAX_RELAX_EXTRA = 300;
	private int relax;

	private bool needSuperRelax = false;

	public bool hungry = false;
	public bool hungryJob = false;
	public bool thirsty = false;
	public bool thirstyJob = false;
	public bool sleepy = false;
	public bool sleepJob = false;

	// Use this for initialization
	void Start () {
	//	manager = GameObject.FindGameObjectWithTag ("Manager").GetComponent<GameManager> ();
		jobs = GetComponent<MobJobs>();
		move = GetComponent<MobMovement>();
		RefreshBoredom ();
		Relax ();
	}

	public void Relax(){
		relax = MAX_RELAX;
	}
	public void SuperRelax(){
		relax = MAX_RELAX_EXTRA;
	}
	public void RefreshBoredom(){
		bored = Random.Range(MAX_BORED/5, MAX_BORED);
	}
	
	// Update is called once per frame
	void Update () {
		if(move.walking){
			move.Step();
		}
		if(jobs.HaveJob()){
			move.walking = !jobs.DoJob();
		} else {
			if(relax-- <= 0){
				if(needSuperRelax){
					SuperRelax();
				}else{
					Relax ();
				}
				bool haveJob = jobs.FindJob();
				if (haveJob){
					//find goal position on grid
					int x_goal = GameManager.WorldToGrid(jobs.myJob.JobLocation.x);
					int y_goal = GameManager.WorldToGrid(jobs.myJob.JobLocation.y);

					move.SetGoal(x_goal, y_goal);
				}else{
					if(--bored <= 0){
						TakeStep ();
					}
				}
			}
		}
	}


	public void TakeStep(){
		//Debug.Log ("Mob behaviour Taking a step");
		RefreshBoredom();
		if(jobs.HaveJob()){
			if(jobs.CurrentJobIsPersonalWalkJob()){
				jobs.CancelJob();
			}else{
				jobs.AbandonJob();
			}
		}
		jobs.CreateWalkJob();
		//needSuperRelax = true;
	}

	public void PickUp(){
		RefreshBoredom ();
		jobs.CreatePickUpJob();
	}
	public void Eat(){
		if(!hungryJob){
			Debug.Log ("Mob Behaviour Eat!");
			RefreshBoredom();
			jobs.CreateEatDrinkJob("Eat");
			hungryJob = true;
		}
	}
	public void Drink(){
		if(!thirstyJob){
			Debug.Log ("Mob Behaviour Drink!");
			RefreshBoredom();
			jobs.CreateEatDrinkJob("Drink");
			thirstyJob = true;
		}
	}

	public void Sleep(float maxSleep){
		if(!sleepJob){
			Debug.Log("Mob behaviour Sleep!");
			RefreshBoredom();
			jobs.CreateSleepJob(maxSleep);
			sleepJob = true;
		}
	}
}
