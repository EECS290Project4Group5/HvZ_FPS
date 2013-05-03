using UnityEngine;
using System.Collections;

public class GameMaster : MonoBehaviour {
	
	public bool isPaused = true;
	public string pauseKey = "escape";
	public float currentTime;
	public GameObject objective;
	public GameObject player;
	public bool vic;
	private Vector3 startLoc;
	private Vector3 victLoc;
	
	// Use this for initialization
	void Start () {
		// get the victory and start location, and time limit based on current level
		if (GameState.currLevel == 1){
			victLoc = GameState.victLvl1;
			startLoc = GameState.startLvl1;
			currentTime = GameState.timeLvl1;
		}
		if (isPaused)
			Time.timeScale = 0;
		//set up objective marker and place player in the proper location
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
		// if not paused, pause
		if(!isPaused)
			Time.timeScale = 0;
		else
		// otheriwse unpause
			Time.timeScale = 1;
		isPaused = !isPaused;
	}
	
	/* We can use this method to check for other conditions based on level.
	 * Currently, only the location ofthe player is checked, so it is an immediate victory if
	 * player reaches the objective marker */
	public void atObjective(){
		GameState.victory = true;
		Debug.Log ("victory");
		//Application.LoadLevel("PostLevelScreen");
	}
	
	/* Post level screen will display information based on victory or defeat. There
	 * will be a different scene for "death" */
	void lost(){
		GameState.victory = false;
		Debug.Log ("Lost");
		//Application.LoadLevel("PostLevelScreen");
	}
}
