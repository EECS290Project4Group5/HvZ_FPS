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
	
	public Texture2D staminaEmpty;
	public Texture2D staminaFull;
	public Texture2D ammoEmpty;
	public Texture2D ammoFull;
	public Texture2D reloadEmpty;
	public Texture2D reloadFull;
	
	public GUIStyle ammoStyle;
	public GUIStyle timerStyle;
	public GUIStyle reloadStyle;
	
	public GameObject player;
	
	//private variables
	private Vector3 reloadMeterLoc;
	private float reloadElapsed;
	private bool reloading = false;
	private Player playerScript;
	private GameMaster masterScript;

	// Use this for initialization
	void Start () {
		playerScript = (Player) player.GetComponent ("Player");
		masterScript = (GameMaster) GetComponent ("GameMaster");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI(){
		timerDisplay ();
		
		staminaBar ();
		
		gunDisplay ();
		
		if (reloading){
			reloadMeterLoc = Camera.main.WorldToScreenPoint (player.transform.position);
			reloadMeterLoc.x -= (reloadMeterSize.x / 2) + reloadMeterOffset.x;
			reloadMeterLoc.y = (Screen.height - reloadMeterLoc.y) - reloadMeterSize.y / 2 + reloadMeterOffset.y;
			reloadElapsed -= Time.deltaTime;
			reloadMeter ();
			if (reloadElapsed <= 0)
				reloading = false;
		}
	}
	
	// Displays time remaining
	void timerDisplay(){
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
	void staminaBar(){
		GUI.BeginGroup (new Rect(staminaLoc.x * Screen.width, staminaLoc.y * Screen.height, staminaSize.x * Screen.width, staminaSize.y * Screen.height));
			GUI.Label (new Rect(0, 0, staminaEmpty.width, staminaEmpty.height), staminaEmpty);
			GUI.BeginGroup (new Rect(0, 0, (playerScript.stamina / playerScript.maxStamina) * staminaSize.x * Screen.width, staminaSize.y * Screen.height));
				GUI.Label (new Rect(0, 0, staminaFull.width, staminaFull.height), staminaFull);
			GUI.EndGroup();
		GUI.EndGroup();
	}
	
	// Displays ammo in clip and total ammo carried
	void gunDisplay(){
		GUI.BeginGroup (new Rect(gunLoc.x * Screen.width, gunLoc.y * Screen.height, gunSize.x * Screen.width, gunSize.y * Screen.height));
			GUI.Label (new Rect(0, 0, ammoEmpty.width, ammoEmpty.height), staminaEmpty);
			GUI.BeginGroup (new Rect(0, 0, ((float)playerScript.currentClip / (float)playerScript.maxClipSize) * gunSize.x * Screen.width, gunSize.y * Screen.height));
				GUI.Label (new Rect(0, 0, ammoFull.width, ammoFull.height), ammoFull);
			GUI.EndGroup ();
			GUI.Label (new Rect(ammoNumLoc.x * gunSize.x * Screen.width, ammoNumLoc.y * gunSize.y * Screen.height, gunSize.x * Screen.width, gunSize.y * Screen.height),
						playerScript.currentClip + " / " + playerScript.maxAmmo, ammoStyle);
		GUI.EndGroup ();
	}
	
	// Displays the reloading meter
	void reloadMeter(){
		GUI.Label (new Rect(reloadMeterLoc.x + reloadLabelLoc.x, reloadMeterLoc.y + reloadLabelLoc.y, reloadLabelSize.x, reloadLabelSize.y), "Reloading!", reloadStyle);
		GUI.BeginGroup (new Rect(reloadMeterLoc.x, reloadMeterLoc.y, reloadMeterSize.x, reloadMeterSize.y));
			GUI.Label (new Rect(0, 0, reloadEmpty.width, reloadEmpty.height), reloadEmpty);
			GUI.BeginGroup (new Rect(0, 0, (reloadElapsed / playerScript.reloadTime) * reloadMeterSize.x, reloadMeterSize.y));
				GUI.Label (new Rect(0, 0, reloadFull.width, reloadFull.height), reloadFull);
			GUI.EndGroup ();
		GUI.EndGroup ();
	}
	
	public void reload(){
		reloadElapsed = playerScript.reloadTime;
		reloading = true;
	}
}