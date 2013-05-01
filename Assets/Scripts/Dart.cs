using UnityEngine;
using System.Collections;

public class Dart : MonoBehaviour {
	
	//Dart speed
	public float constantVelocity = 200;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		//Move dart at constant speed in direction it's facing
		rigidbody.velocity = transform.up * constantVelocity;
	}
	
	void OnBecameInvisible() {
		this.Destroy(gameObject);
	}
	
	void OnCollisionEnter(Collision collision)
	{
		if(collision.gameObject.tag != "Human")
		{
			this.Destroy(gameObject);
		}
	}
}
