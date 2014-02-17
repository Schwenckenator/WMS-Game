using UnityEngine;
using System.Collections;

public class ItemDetails : MonoBehaviour {
	public bool assigned = false;

	public string itemName;
	private int objNumber;
	public bool edible;
	public bool drinkable;

	public int eatPower;
	public int drinkPower;



	void Start(){
		objNumber = GameManager.IdentifyItem(itemName);
	}



	public void Initialise(bool _eat, int _eatPwr, bool _drink, int _drinkPwr){
		edible = _eat;
		eatPower = _eatPwr;
		drinkable = _drink;
		drinkPower = _drinkPwr;
	}

	public int GetEatPower(){
		return eatPower;
	}
	public int GetDrinkPower(){
		return drinkPower;
	}
	public bool IsEdible(){
		return edible;
	}
	public bool IsDrinkable(){
		return drinkable;
	}

	public int GetItemNumber(){
		return objNumber;
	}

	public void ConsumeItem(){
		Destroy(gameObject);
	}
}