using UnityEngine;
using System.Collections;

public class GameMaster : MonoBehaviour {
	
	public bool isPaused = true;
	public string pauseKey = "escape";
	public float startTime;
	public float currentTime;
	
	// Use this for initialization
	void Start () {
		currentTime = startTime;
		if (isPaused)
			Time.timeScale = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown (pauseKey))
			pause();
		
		//checkForVictory();
		
		currentTime -= Time.deltaTime;
		//if (currentTime <= 0)
			//lose();
	}
	
	void pause(){
		if(!isPaused)
			Time.timeScale = 0;
		else
			Time.timeScale = 1;
		isPaused = !isPaused;
	}
}
