using UnityEngine;
using System.Collections;

public class DroppedObject : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		GameObject human = GameObject.FindGameObjectWithTag("Human");
		Player p = (Player)human.GetComponent(typeof(Player));
		if(Vector3.Distance(this.transform.position, human.transform.position) < 25)
		{
			this.Destroy(gameObject);
			p.PickUpAmmo();
		}
	}
}
