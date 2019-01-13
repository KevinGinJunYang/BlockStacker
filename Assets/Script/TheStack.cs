using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TheStack : MonoBehaviour {
	public Text scoreText;
	public GameObject endPanel;
	public Color32[] gameColors = new Color32[4];
	public Material Stackmat;

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
			colorMesh(theStack[i].GetComponent<MeshFilter>().mesh);
		}
		stackIndex = transform.childCount - 1;
	}
	
	// Update is called once per frame
	private void Update () {
		if (gameOver) {
			return;
		}

		if (Input.GetMouseButtonDown (0)) {
			if(placeTile()){
				spawnTile ();
				scoreCount++;
				scoreText.text = scoreCount.ToString ();
			} else{
				endGame();
			}
		}

		moveTile ();

		//move the stack
		transform.position = Vector3.Lerp (transform.position, desiredPosition, STACK_MOVING_SPEED * Time.deltaTime);
	}

	private void moveTile(){
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

		colorMesh(theStack[stackIndex].GetComponent<MeshFilter> ().mesh);

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
				createRubble (
					new Vector3 ((t.position.x > 0) 
						? t.position.x + (t.localScale.x / 2)
						: t.position.x - (t.localScale.x / 2)
						, t.position.y
						, t.position.z),
					new Vector3(Mathf.Abs(deltaX),1,t.localScale.z)
				);
				t.localPosition = new Vector3 (middle - (lastTilePos.x / 2), scoreCount, lastTilePos.z);
			} else {
				if (combo > COMBO_START_GAIN) {
					if (stackBounds.x > BOUNDS_SIZE) {
						stackBounds.x = BOUNDS_SIZE;
					}
					stackBounds.x += STACK_BOUNDS_GAIN;
					float middle = lastTilePos.x + t.localPosition.x / 2; 
					t.localScale = new Vector3 (stackBounds.x, 1, stackBounds.y);
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
				createRubble (
					new Vector3 (t.position.x
						, t.position.y
						, (t.position.z > 0) 
						? t.position.z + (t.localScale.z / 2)
						: t.position.z - (t.localScale.z / 2)),
					new Vector3 (t.localScale.x, 1, Mathf.Abs (deltaZ))
				);
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
		go.GetComponent<MeshRenderer> ().material = Stackmat;
			
		colorMesh(go.GetComponent<MeshFilter> ().mesh);
	}


	private Color32 Lerp4(Color32 a, Color32 b, Color32 c, Color32 d, float t){
		if( t < 0.33f ){
			return Color.Lerp (a,b,t/0.33f);
		} else if ( t < 0.66f ){
			return Color.Lerp(b,c,(t-0.33f) / 0.33f);
		} else{
			return Color.Lerp(c,d,(t-0.66f)/0.66f);
		}
	}

	private void colorMesh(Mesh mesh){
		Vector3[] vertices = mesh.vertices;
		Color32[] colors = new Color32[vertices.Length];
		float f = Mathf.Sin (scoreCount * 0.25f);


		for (int i = 0; i < vertices.Length; i++) {
			colors [i] = Lerp4 (gameColors [0], gameColors [1], gameColors [2], gameColors [3], f);

		}

		mesh.colors32 = colors;
		
	}
	private void endGame(){
		if (PlayerPrefs.GetInt ("score") < scoreCount) {
			PlayerPrefs.SetInt ("score", scoreCount);
		}
		gameOver = true;
		//Makes stack fall 
		endPanel.SetActive(true);
		theStack [stackIndex].AddComponent<Rigidbody> ();
	}

	public void onButtonClick(string sceneName){
		SceneManager.LoadScene (sceneName);
	}
				
}
