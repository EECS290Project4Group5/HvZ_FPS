// Tim Rogers
// This class creates the GUI for the game.

using UnityEngine;
using System.Collections;

public class MasterGUI : MonoBehaviour {
	
	//public variables
	public Vector2 staminaLoc;
	public Vector2 staminaSize;
	public Vector2 timerLoc;
	public Vector2 timerSize;
	public Vector2 gunLoc;
	public Vector2 gunSize;
	public Vector2 ammoNumLoc;
	public Vector2 reloadLabelLoc;
	public Vector2 reloadLabelSize;
	public Vector2 reloadMeterOffset;
	public Vector2 reloadMeterSize;
	public Vector2 pauseMenuLoc;
	public Vector2 pauseMenuSize;
	
	public Texture2D staminaEmpty;
	public Texture2D staminaFull;
	public Texture2D ammoEmpty;
	public Texture2D ammoFull;
	public Texture2D reloadEmpty;
	public Texture2D reloadFull;
	
	public GUIStyle ammoStyle;
	public GUIStyle timerStyle;
	public GUIStyle reloadStyle;
	//public GUIStyle pauseMenuStyle;
	
	public string[] pauseMenuItems;
	
	public GameObject player;
	
	//private variables
	private Vector3 reloadMeterLoc;
	private float reloadElapsed;
	
	private bool reloading = false;
	private bool mainMenu = true;
	private bool[] menuBools;
	
	private Player playerScript;
	private GameMaster masterScript;

	// Use this for initialization
	void Start () {
		playerScript = (Player) player.GetComponent ("Player");
		masterScript = (GameMaster) GetComponent ("GameMaster");
		menuBools = new bool[pauseMenuItems.Length];
		for (int i = 0; i < menuBools.Length; i++)
			menuBools[i] = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI(){
		//display the timer
		timerDisplay ();
		
		//display the stamina bar
		staminaBar ();
		
		//display current ammo
		gunDisplay ();
		
		//display the reloading meter if currently reloading
		if (reloading){
			//place reload meter above player
			reloadMeterLoc = Camera.main.WorldToScreenPoint (player.transform.position);
			reloadMeterLoc.x -= (reloadMeterSize.x / 2) + reloadMeterOffset.x;
			reloadMeterLoc.y = (Screen.height - reloadMeterLoc.y) - reloadMeterSize.y / 2 + reloadMeterOffset.y;
			//decrement the reloading timer
			reloadElapsed -= Time.deltaTime;
			reloadMeter ();
			if (reloadElapsed <= 0)
				//if finished reloading, take meter off screen
				reloading = false;
		}
		
		//display pause menu if paused
		if (masterScript.isPaused){
			if (mainMenu){
				pauseMenu ();
			}
			//if resume game is clicked, resume game
			if (menuBools[0])
				resume ();
			//show options if options is clicked
			if (menuBools[1]){
				showOptions ();
				mainMenu = true;
			}
			//restart level
			if (menuBools[2]){
				restart ();
			}
			//quit to main menu
			if (menuBools[3])
				quit ();
		}
	}
	
	// Displays time remaining
	void timerDisplay (){
		//floor function to convert to int
		int timeToDisplay = Mathf.FloorToInt(masterScript.currentTime);
		
		//convert seconds into minutes and seconds
		int min = timeToDisplay / 60;
		int sec = timeToDisplay % 60;
		
		//place zero in front if seconds is single digit
		string secString;
		if(sec < 10)
			secString = "0" + sec;
		else
			secString = sec + "";
		
		//create a new label to display the time
		GUI.BeginGroup(new Rect(timerLoc.x * Screen.width, timerLoc.y * Screen.height, timerSize.x * Screen.width, timerSize.y * Screen.height));
			GUI.Label (new Rect(0, 0, timerSize.x * Screen.width, timerSize.y * Screen.height), min + ":" + secString, timerStyle);
		GUI.EndGroup();
	}
	
	// Displays stamina meter
	void staminaBar (){
		GUI.BeginGroup (new Rect(staminaLoc.x * Screen.width, staminaLoc.y * Screen.height, staminaSize.x * Screen.width, staminaSize.y * Screen.height));
			GUI.Label (new Rect(0, 0, staminaEmpty.width, staminaEmpty.height), staminaEmpty);
			// adjust the amount of meter shown based on amount of stamina left
			GUI.BeginGroup (new Rect(0, 0, (playerScript.stamina / playerScript.maxStamina) * staminaSize.x * Screen.width, staminaSize.y * Screen.height));
				GUI.Label (new Rect(0, 0, staminaFull.width, staminaFull.height), staminaFull);
			GUI.EndGroup();
		GUI.EndGroup();
	}
	
	// Displays ammo in clip and total ammo carried
	void gunDisplay (){
		GUI.BeginGroup (new Rect(gunLoc.x * Screen.width, gunLoc.y * Screen.height, gunSize.x * Screen.width, gunSize.y * Screen.height));
			GUI.Label (new Rect(0, 0, ammoEmpty.width, ammoEmpty.height), staminaEmpty);
			//adjust the amount of meter shown based on amount of ammo in clip
			GUI.BeginGroup (new Rect(0, 0, ((float)playerScript.currentClip / (float)playerScript.maxClipSize) * gunSize.x * Screen.width, gunSize.y * Screen.height));
				GUI.Label (new Rect(0, 0, ammoFull.width, ammoFull.height), ammoFull);
			GUI.EndGroup ();
			//display the ammo in clip and total ammo
			GUI.Label (new Rect(ammoNumLoc.x * gunSize.x * Screen.width, ammoNumLoc.y * gunSize.y * Screen.height, gunSize.x * Screen.width, gunSize.y * Screen.height),
						playerScript.currentClip + " / " + playerScript.currentAmmo, ammoStyle);
		GUI.EndGroup ();
	}
	
	// Displays the reloading meter
	void reloadMeter (){
		GUI.Label (new Rect(reloadMeterLoc.x + reloadLabelLoc.x, reloadMeterLoc.y + reloadLabelLoc.y, reloadLabelSize.x, reloadLabelSize.y), "Reloading!", reloadStyle);
		GUI.BeginGroup (new Rect(reloadMeterLoc.x, reloadMeterLoc.y, reloadMeterSize.x, reloadMeterSize.y));
			GUI.Label (new Rect(0, 0, reloadEmpty.width, reloadEmpty.height), reloadEmpty);
			// adjust the amount of meter shown based on amount of time left
			GUI.BeginGroup (new Rect(0, 0, (reloadElapsed / playerScript.reloadTime) * reloadMeterSize.x, reloadMeterSize.y));
				GUI.Label (new Rect(0, 0, reloadFull.width, reloadFull.height), reloadFull);
			GUI.EndGroup ();
		GUI.EndGroup ();
	}
	
	public void reload (){
		reloadElapsed = playerScript.reloadTime;
		reloading = true;
	}
	
	// display the pause menu
	void pauseMenu (){
		GUI.BeginGroup (new Rect(pauseMenuLoc.x * Screen.width, pauseMenuLoc.y * Screen.height, pauseMenuSize.x * Screen.width, pauseMenuSize.y * Screen.height));
			float itemSize = (pauseMenuSize.y * Screen.height) / pauseMenuItems.Length;
			for (int i = 0; i < pauseMenuItems.Length; i++){
				if (GUI.Button (new Rect(0, itemSize * i, pauseMenuSize.x * Screen.width, itemSize), pauseMenuItems[i]/*, pauseMenuStyle*/))
					menuBools[i] = true;
			}
		GUI.EndGroup ();
	}
	
	//unpause the game
	void resume (){
		masterScript.pause ();
	}
	
	// shows an options screen (not implemented yet
	void showOptions (){
		mainMenu = false;
		
	}
	
	//restarts level (not implemented)
	void restart (){
		mainMenu = false;
		//masterScript.restartLevel ();
	}
	
	//quits to start screen (not implemented)
	void quit (){
		mainMenu = false;
		//masterScript.quitGame ();
	}
}