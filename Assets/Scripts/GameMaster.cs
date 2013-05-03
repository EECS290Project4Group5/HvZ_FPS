using UnityEngine;
using System.Collections;

public class GameMaster : MonoBehaviour {
	
	public bool isPaused = true;
	public string pauseKey = "escape";
	public float startTime;
	public float currentTime;
	public GameObject objective;
	public GameObject player;
	public bool vic;
	private Vector3 startLoc;
	private Vector3 victLoc;
	
	// Use this for initialization
	void Start () {
		if (GameState.currLevel == 1)
			victLoc = GameState.victLvl1;
		if (GameState.currLevel == 1)
			startLoc = GameState.startLvl1;
		currentTime = startTime;
		if (isPaused)
			Time.timeScale = 0;
		Instantiate (objective, victLoc, Quaternion.identity);
		player.transform.position = startLoc;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown (pauseKey)) {
			pause();
			Debug.Log ("escape pressed");
		}
		currentTime -= Time.deltaTime;
		if (currentTime <= 0)
			lost ();
		vic = GameState.victory;
	}
	
	public void pause(){
		if(!isPaused)
			Time.timeScale = 0;
		else
			Time.timeScale = 1;
		isPaused = !isPaused;
	}
	
	public void atObjective(){
		GameState.victory = true;
		Debug.Log ("victory");
		//Application.LoadLevel("PostLevelScreen");
	}
	
	void lost(){
		GameState.victory = false;
		Debug.Log ("Lost");
		//Application.LoadLevel("PostLevelScreen");
	}
}
