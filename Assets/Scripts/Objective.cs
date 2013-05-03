using UnityEngine;
using System.Collections;

public class Objective : MonoBehaviour {
	
	public GameObject master;
	private GameMaster gm;

	// Use this for initialization
	void Start () {
		master = GameObject.FindGameObjectWithTag("GM");
		gm = (GameMaster)master.GetComponent ("GameMaster");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	//if player enters objective space, tell master to
	//check if all other conditions are met
	void OnTriggerEnter(Collider obj){
		if (obj.CompareTag("Human"))
			gm.atObjective();
	}
}
