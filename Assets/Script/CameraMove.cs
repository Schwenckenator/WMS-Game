using UnityEngine;
using System.Collections;

public class CameraMove : MonoBehaviour {
	//This is the script that moves the camera RTS style
	//Also you can move the camera via arrow keys
	float speed = 0.1f;
	int mousePixelBuffer = 2;
	bool moveUp;
	bool moveRight;
	bool moveDown;
	bool moveLeft;

	float upBound, rightBound, downBound, leftBound;
	float adjustment =  -0.125f;

	// Use this for initialization
	void Start () {

		upBound = GameManager.GridToWorld(GameManager.MaxGrid()) + adjustment;
		rightBound = GameManager.GridToWorld(GameManager.MaxGrid()) + adjustment;
		leftBound = GameManager.GridToWorld(0) + adjustment;
		downBound = GameManager.GridToWorld(0) + adjustment;
	}
	
	// Update is called once per frame
	void Update () {
		//Check for keyboard input

		if(Input.GetKey(KeyCode.UpArrow)){
			Up ();
		}
		if(Input.GetKey(KeyCode.RightArrow)){
			Right ();
		}
		if(Input.GetKey(KeyCode.DownArrow)){
			Down ();
		}
		if(Input.GetKey(KeyCode.LeftArrow)){
			Left ();
		}

		//Check for mouse input
		if(Input.mousePosition.x < mousePixelBuffer){
			Left();
		}
		if(Input.mousePosition.y < mousePixelBuffer){
			Down (); // I hope this is right lol
		}
		if(Input.mousePosition.x > Screen.width - mousePixelBuffer){
			Right ();
		}
		if(Input.mousePosition.y > Screen.height - mousePixelBuffer){
			Up ();
		}

		//All checks done, make the move
		MakeMove ();
	}

	void MakeMove(){
		if(moveUp){
			if(CompareBounds(0,1)){
				Vector3 newPos = Vector2.up*speed;
				transform.position += newPos;
			}
		}
		if(moveRight){
			if(CompareBounds(1,0)){
				Vector3 newPos = Vector2.right*speed;
				transform.position += newPos;
			}
		}
		if(moveDown){
			if(CompareBounds(0,-1)){
				Vector3 newPos = -Vector2.up*speed;
				transform.position += newPos;
			}
		}
		if(moveLeft){
			if(CompareBounds(-1,0)){
				Vector3 newPos = -Vector2.right*speed;
				transform.position += newPos;
			}
		}
		Clear();
	}
	void Clear(){
		moveUp = false;
		moveRight = false;
		moveLeft = false;
		moveDown = false;
	}
	void Up(){
		moveUp= true;
	}
	void Right(){
		moveRight = true;
	}
	void Down(){
		moveDown = true;
	}
	void Left(){
		moveLeft = true;
	}

	// -1 for left or down
	// 0 for no comparison
	// 1 for right or up
	bool CompareBounds(int xIn, int yIn){
		bool inBounds = true;
		int midHeight = Screen.height/2;
		int maxHeight = Screen.height;
		int midWidth = Screen.width/2;
		int maxWidth = Screen.width;


		if(xIn > 0){ //right
			if(Camera.main.ScreenToWorldPoint(new Vector3(maxWidth, midHeight)).x > rightBound){
				inBounds = false;
			}
		}else if(xIn < 0){ // Left
			if(Camera.main.ScreenToWorldPoint(new Vector3(0, midHeight)).x < leftBound){
				inBounds = false;
			}
		}
		if(yIn > 0){ // up
			if(Camera.main.ScreenToWorldPoint(new Vector3(midWidth, maxHeight)).y > upBound){
				inBounds = false;
			}
		}
		if(yIn < 0){ // down
			if(Camera.main.ScreenToWorldPoint(new Vector3(midWidth, 0)).y < downBound){
				inBounds = false;
			}
		}

		return inBounds;
	}
}
