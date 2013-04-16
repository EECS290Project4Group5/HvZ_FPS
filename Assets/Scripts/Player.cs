using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	public int currentClip;
	public int maxClipSize;
	public int currentAmmo;
	public int maxAmmo;
	public int socks;
	public int maxSocks;
	public int water;
	public int maxWater;
	
	public float stamina;
	public float maxStamina;
	public float regenerationFactor;
	public float WalkSpeed;
	public float SprintSpeed;
	public float reloadTime;
	
	private bool inBuilding;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// FixedUpdate is called once per physics simulation frame
	void FixedUpdate () {
		if(Input.GetKey(KeyCode.W)) {
			move(Vector3.forward);
		} else if(Input.GetKey(KeyCode.A)) {
			move(Vector3.left);
		} else if(Input.GetKey(KeyCode.S)) {
			move(Vector3.back);
		} else if (Input.GetKey(KeyCode.D)) {
			move(Vector3.right);
		}
		
		if(Input.GetKey(KeyCode.Q)) {
			ThrowSock();
		}
		
		if(Input.GetKey(KeyCode.E)) {
			DrinkWater();
		}
		
		if(Input.GetKey(KeyCode.Space)) {
			Shoot ();
		}
		
		if(Input.GetKey(KeyCode.R)) {
			Reload();
		}
			
	}
	
	void move(Vector3 direction) {
		this.transform.Translate(direction * WalkSpeed * Time.fixedDeltaTime);
	}
	
	void Reload() {
		
	}
	
	void Sprint() {
		
	}
	
	void Shoot() {
		
	}
	
	void ThrowSock() {
		
	}
	
	void DrinkWater() {
		
	}
}
