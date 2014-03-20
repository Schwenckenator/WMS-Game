using UnityEngine;
using System.Collections;

public class SelectionRingDetails : MonoBehaviour {
	public JobClass job;

	public void SetJob(JobClass newJob){
		job = newJob;
	}
	public JobClass GetJob(){
		return job;
	}
}
