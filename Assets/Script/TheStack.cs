using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheStack : MonoBehaviour {

	private const float BOUNDS_SIZE = 3.5f;

	private float tileTransistion = 0.0f;
	private float tileSpeed = 2.5f;

	private GameObject[] theStack;
	private int stackIndex;
	private int scoreCount = 0;
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
	}

	private void moveTile(){
		tileTransistion += Time.deltaTime * tileSpeed;
		theStack [stackIndex].transform.localPosition = new Vector3 (Mathf.Sin (tileTransistion) * BOUNDS_SIZE, scoreCount, 0);
	}

	private void spawnTile(){
		stackIndex--;
		if (stackIndex < 0)
			stackIndex = transform.childCount - 1;
		theStack [stackIndex].transform.localPosition = new Vector3 (0, scoreCount, 0);
	}

	private bool placeTile(){
		return true;
			
	}

	private void endGame(){

	}
	

					
}
