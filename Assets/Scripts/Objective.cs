using UnityEngine;
using System.Collections;

public class Objective : MonoBehaviour {
	
	public GameObject master;
	private GameMaster gm;

	// Use this for initialization
	void Start () {
		master = GameObject.FindWithTag("GM");
		gm = (GameMaster)master.GetComponent ("GameMaster");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerEnter(Collider obj){
		if (obj.CompareTag("Human"))
			gm.atObjective();
	}
}
