/* Tim Rogers
 * This class is used to hold information about the game across multiple scenes.
 * We could potentially use this information to write to a save file.
 * */

using UnityEngine;
using System.Collections;

public static class GameState {
	
	static public int currLevel = 1;  //current level loaded
	static public bool victory;		// true is player successfully completed level (would be used in post level screen)
	static public Vector3 victLvl1 = new Vector3(-970, 25, -772);	//objective location for level 1;
	static public Vector3 startLvl1 = new Vector3(-650, 25, -55);	//start location for level 1;
	static public float timeLvl1 = 100;	//time limit for level 1;

}