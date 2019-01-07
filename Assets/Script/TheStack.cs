using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheStack : MonoBehaviour {

	private const float BOUNDS_SIZE = 3.5f;
	private const float STACK_MOVING_SPEED = 5.0f;
	private const float ERROR_MARGIN = 0.1f;
	private const float STACK_BOUNDS_GAIN = 0.25f;
	private const int COMBO_START_GAIN = 5;

	private float tileTransistion = 0.0f;
	private float tileSpeed = 2.5f;
	private bool isMovingOnX = true;
	private float secondaryPosition;
	private bool gameOver = false;

	private GameObject[] theStack;
	private int stackIndex;
	private int scoreCount = 0;
	private int combo = 0;
	//Array of stack
	private Vector2 stackBounds = new Vector2 (BOUNDS_SIZE, BOUNDS_SIZE);

	private Vector3 lastTilePos;
	private Vector3 desiredPosition;
	// Use this for initialization
	private void Start () {
		theStack = new GameObject[transform.childCount];
		for (int i = 0; i < transform.childCount; i++) {
			theStack [i] = transform.GetChild (i).gameObject;
		}

		stackIndex = transform.childCount - 1;
			
	}
	
	// Update is called once per frame
	private void Update () {
		if (Input.GetMouseButtonDown (0)) {
			if(placeTile()){
				spawnTile ();
				scoreCount++;
			} else{
				endGame();
			}
		}

		moveTile ();

		//move the stack
		transform.position = Vector3.Lerp (transform.position, desiredPosition, STACK_MOVING_SPEED * Time.deltaTime);
	}

	private void moveTile(){
		if (gameOver) {
			return;
		}
		//Movement 
		tileTransistion += Time.deltaTime * tileSpeed;
		if (isMovingOnX) {
			theStack [stackIndex].transform.localPosition = new Vector3 (Mathf.Sin (tileTransistion) * BOUNDS_SIZE, scoreCount, secondaryPosition);
		} else {
			theStack [stackIndex].transform.localPosition = new Vector3 (secondaryPosition, scoreCount, Mathf.Sin (tileTransistion) * BOUNDS_SIZE);
		}

	}

	private void spawnTile(){
		lastTilePos = theStack [stackIndex].transform.localPosition;
		stackIndex--;
		if (stackIndex < 0)
			stackIndex = transform.childCount - 1;
		desiredPosition = (Vector3.down) * scoreCount;
		//Set stack to current position 
		theStack [stackIndex].transform.localPosition = new Vector3 (0, scoreCount, 0);
		//Set size to current scale 
		theStack[stackIndex].transform.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);

	}

	private bool placeTile(){
		Transform t = theStack [stackIndex].transform;

		//MOVEMENT
		if (isMovingOnX) {
			float deltaX = lastTilePos.x - t.position.x;
			//FAIL CHECK 
			if (Mathf.Abs (deltaX) > ERROR_MARGIN) {
				//CUT TILE
				combo = 0;
				stackBounds.x -= Mathf.Abs (deltaX);
				if (stackBounds.x <= 0) {
					return false;
				}

				float middle = lastTilePos.x + t.localPosition.x / 2; 
				t.localScale = new Vector3 (stackBounds.x, 1, stackBounds.y);
				t.localPosition = new Vector3 (middle - (lastTilePos.x / 2), scoreCount, lastTilePos.z);
			} else {
				if (combo > COMBO_START_GAIN) {
					if (stackBounds.x > BOUNDS_SIZE) {
						stackBounds.x = BOUNDS_SIZE;
					}
					stackBounds.x += STACK_BOUNDS_GAIN;
					float middle = lastTilePos.x + t.localPosition.x / 2; 
					t.localScale = new Vector3 (stackBounds.x, 1, stackBounds.y);
					//createRubble ();
					t.localPosition = new Vector3 (middle - (lastTilePos.x / 2), scoreCount, lastTilePos.z);
				}
				combo++;
				t.localPosition = new Vector3 (lastTilePos.x, scoreCount, lastTilePos.z);
			}
		} else {
			float deltaZ = lastTilePos.z - t.position.z;
			if (Mathf.Abs (deltaZ) > ERROR_MARGIN) {
				//CUT TILE
				combo = 0;
				stackBounds.y -= Mathf.Abs (deltaZ);
				if (stackBounds.y <= 0) {
					return false;
				}

				float middle = lastTilePos.z + t.localPosition.z / 2; 
				t.localScale = new Vector3 (stackBounds.x, 1, stackBounds.y);
				t.localPosition = new Vector3 (lastTilePos.x, scoreCount, middle - (lastTilePos.z / 2));
			} else {
				if (combo > COMBO_START_GAIN) {
					if (stackBounds.y > BOUNDS_SIZE) {
						stackBounds.y = BOUNDS_SIZE;
					}
					stackBounds.y += STACK_BOUNDS_GAIN;
					float middle = lastTilePos.z + t.localPosition.z / 2; 
					t.localScale = new Vector3 (stackBounds.x, 1, stackBounds.y);
					t.localPosition = new Vector3 (lastTilePos.x, scoreCount, middle - (lastTilePos.z / 2));
				}
				combo++;
				t.localPosition = new Vector3 (lastTilePos.x, scoreCount, lastTilePos.z);
			}
		}

		secondaryPosition = (isMovingOnX)
			? t.localPosition.x
			: t.localPosition.z;
		isMovingOnX = !isMovingOnX;
		return true;
			
	}

	private void createRubble(Vector3 pos, Vector3 scale){
		GameObject go = GameObject.CreatePrimitive (PrimitiveType.Cube);
		go.transform.localPosition = pos;
		go.transform.localScale = scale;
		go.AddComponent<Rigidbody> ();
	}

	private void endGame(){
		gameOver = true;
		//Makes stack fall 
		theStack [stackIndex].AddComponent<Rigidbody> ();
	}
	

					
}
