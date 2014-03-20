using UnityEngine;
using System.Collections;

public class MobCondition : MonoBehaviour {
	//use big ints for stats that tick down
	private GameManager manager;
	private MobBehaviour mobBehave;

	private float hunger;
	private float thirst;
	private float health;
	private float happiness;
	private float sleep;

	// ~ -> default
	// If the number are different, it's for testing purposes
	private float maxHunger = 1000; 	//~ 1000
	private float maxThirst = 500; 		//~ 500
	private float maxHealth = 100; 		//~ 100
	private float maxHappiness = 100; 	//~ 100
	private float maxSleep = 3000; 		//~ 3000

	private int strength;		// NOT CURRENTLY USED
	private int charisma;		// NOT CURRENTLY USED
	private int intelligence;	// NOT CURRENTLY USED
	private int perception;	// NOT CURRENTLY USED
	private int defence;		// NOT CURRENTLY USED
	private int accuracy;		// NOT CURRENTLY USED

	// Use this for initialization
	void Start () {
		manager = GameObject.FindGameObjectWithTag ("Manager").GetComponent<GameManager> ();
		mobBehave = GetComponent<MobBehaviour>();

		InitialiseStats();
		StartCoroutine("HungerTick");
		StartCoroutine("ThirstTick");
		StartCoroutine("SleepTick");
		StartCoroutine("HappinessCheck");

	}
	void InitialiseStats(){
		//Randomise stats
		maxHunger *= Random.Range(0.8f, 1.2f);
		maxThirst *= Random.Range(0.8f, 1.2f);
		maxHealth *= Random.Range(0.8f, 1.2f);
		maxSleep *= Random.Range(0.8f, 1.2f);

		//Assign maximum
		hunger = maxHunger;
		thirst = maxThirst;
		happiness = maxHappiness;
		health = maxHealth;
		sleep = maxSleep;
	}

	IEnumerator HungerTick(){
		while(health > 0){
			if(hunger > 0){
				hunger -= 1.0f;
			}else{
				Debug.Log ("Ouch, hurt because hungry. HungryJob = "+mobBehave.hungryJob.ToString());
				DamageHealth(1);
			}
			HungerCheck();
			yield return new WaitForSeconds(1.0f);
		}
	}

	IEnumerator ThirstTick(){
		while (health > 0){
			if(thirst > 0){
				thirst -= 1.0f;
			}else{
				Debug.Log ("Ouch, hurt because thirsty. ThirstyJob = "+mobBehave.thirstyJob.ToString());
				DamageHealth(1);
			}
			ThirstCheck();
			yield return new WaitForSeconds(1.0f);
		}
	}

	IEnumerator SleepTick(){
		while (health > 0){
			if(sleep > 0){
				sleep -= 1.0f;
			}else{
				Debug.Log ("Too Sleepy!");
			}
			SleepCheck();
			yield return new WaitForSeconds(1.0f);
		}
	}

	IEnumerator HappinessCheck(){
		while (health > 0){
			if(thirst < maxThirst * 0.4f){ // Less than 40% of max, start losing happiness
				happiness -= 1.0f;
			}
			if(hunger < maxHunger * 0.4f){
				happiness -= 1.0f;
			}
			yield return new WaitForSeconds(10.0f);
		}
	}



	void HealthCheck(){
		if(health <= 0){
			//Is ded
			//Spawn dead body item
			//GameObject deadBody = Instantiate(manager.TerrainTypes[(int)GameManager.TerrainIndex.humanCorpse], transform.position, transform.rotation) as GameObject;
			Instantiate(manager.TerrainTypes[(int)GameManager.TerrainIndex.humanCorpse], transform.position, transform.rotation);

			//Destroy current person
			Destroy(gameObject);
		}else if(health > maxHealth){
			health = maxHealth;
		}
	}

	void HungerCheck(){
		// At 60% hunger, you are hungry
		if(hunger < maxHunger * 0.6f){
			mobBehave.hungry = true;
			mobBehave.Eat();
		}else {
			mobBehave.hungry = false;
		}
		// At 40% hunger, you lose happiness
		// At 20% hunger, you get "weak" debuff
	}
	void ThirstCheck(){
		// At 60% thirst, you are thirst
		if(thirst < maxThirst * 0.6f){
			mobBehave.thirsty = true;
			mobBehave.Drink();
		}else {
			mobBehave.thirsty = false;
		}
		// At 40% thirst, you lose happiness
		// At 20% thirst, you get "weak" debuff
	}
	void SleepCheck(){
		// At 60% thirst, you are thirst
		if(sleep < maxSleep * 0.2f){
			mobBehave.sleepy = true;
			mobBehave.Sleep (maxSleep);
		}else {
			mobBehave.sleepy = false;
		}
		// At 40% thirst, you lose happiness
		// At 20% thirst, you get "weak" debuff
	}

	public float GetHunger(){
		return hunger;
	}

	public void EatFood(int percent){
		hunger += (percent*maxHunger) / 100.0f;
	}

	public float GetThirst(){
		return thirst;
	}

	public void Drink(int percent){
		thirst += (percent*maxThirst) / 100.0f;
	}
	public void Sleep(){
		sleep = maxSleep;
	}

	public void DamageHealth(int dam){
		health -= dam;
		HealthCheck();
	}

	public void RestoreHealth(int heal){
		health += heal;
		HealthCheck();
	}
}