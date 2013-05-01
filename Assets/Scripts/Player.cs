using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	//Ammo currently in clip
	public int currentClip;
	//Ammo allowed in clip
	public int maxClipSize;
	//Total ammo in inventory
	public int currentAmmo;
	//Total ammo allowed in inventory
	public int maxAmmo;
	//Current sock number
	public int socks;
	//Number of socks allowed in inventory
	public int maxSocks;
	//Current water number
	public int water;
	//Number of waters allowed.
	public int maxWater;
	//Percentage of Max Stamina that is added when water is drank.
	public float waterRegenerationFactor;
	//Current Stamina
	public float stamina;
	//Maximum Stamina
	public float maxStamina;
	//Percentage of stamina to decrease
	public float exhaustionFactor;
	//Percentage of stamina to increase
	public float regenerationFactor;
	//Time that must elapse before reload will work again.
	public float reloadTime;
	//Time that must elapse before shoot will work again.
	public float shootTime;
	
	public float WalkSpeed;
	public float SprintSpeed;
	
	public GameObject master;
	
	//Move speed, set either to walk speed or sprint speed, depending on state.
	private float MoveSpeed;
	//Time when last reload happens
	private float time;
	//Time between last reload/shoot and next attempted reload/shoot
	private float elapsedTimeR;
	private float elapsedTimeS;
	//Boolean for if the player is inside a building
	private bool inBuilding;
	
	//Camera of the game
	public GameObject camera;
	
	//The last direction of travel by the player
	Vector3 lastDirection = Vector3.forward;
	
	// Use this for initialization
	void Start () {
		MoveSpeed = WalkSpeed;
		stamina = maxStamina;
		inBuilding = false;
		time = 0;
		
		//get camera to follow player
		camera.transform.position = this.transform.position + new Vector3 (0, 100, 0);
		camera.transform.parent = this.transform;
	
	}
	
	// FixedUpdate is called once per physics simulation frame
	void FixedUpdate () {
		
		//If stamina is ever greater than total stamina, reset it.
		if(stamina > maxStamina) {
			stamina = maxStamina;
		}
		
		//W A S D keys call move function
		if(Input.GetKey(KeyCode.W)) {
			move(Vector3.forward);
		} else if(Input.GetKey(KeyCode.A)) {
			move(Vector3.left);
		} else if(Input.GetKey(KeyCode.S)) {
			move(Vector3.back);
		} else if (Input.GetKey(KeyCode.D)) {
			move(Vector3.right);
		}
		
		//While the left shift key is pressed, sprint is called.
		if(Input.GetKey(KeyCode.LeftShift)) {
			Sprint();
		}
		
		//If the left shift key is not held down, MoveSpeed is set to WalkSpeed
		if(!Input.GetKey(KeyCode.LeftShift)) {
			MoveSpeed = WalkSpeed;
		}
		
		//While stamina isn't full and sprint is not in use, regenerate stamina
		if(stamina < maxStamina && !Input.GetKey(KeyCode.LeftShift)) {
			stamina = stamina + (maxStamina * regenerationFactor);
		}
		
		
		if(Input.GetKeyDown(KeyCode.Q)) {
			ThrowSock();
		}
		
		if(Input.GetKeyDown(KeyCode.E)) {
			DrinkWater();
		}
		
		if(Input.GetKeyDown(KeyCode.Space)) {
			elapsedTimeS = Time.fixedTime - time;
			if(elapsedTimeS >= shootTime) {
				Shoot ();
			}
		}
		
		//Checks elapsedTime from last reload, if it's greater than reloadTime, reload.
		if(Input.GetKeyDown(KeyCode.R)) {
			elapsedTimeR = Time.fixedTime - time;
			if(elapsedTimeR >= reloadTime) {
				Reload();
			}
		}
		
		//print("Stamina: " + stamina);
		//print(currentClip);
		//print(currentAmmo + "|" + maxAmmo);
		
			
	}
	
	void move(Vector3 direction) {
		this.transform.Translate(direction * MoveSpeed * Time.fixedDeltaTime);
		lastDirection = direction;
	}
	
	//Checks if you have ammo to reload with. If so, checks to see if clip is full. 
	//If not, add one bullet to clip, remove from current ammo.
	void Reload() {
		if(currentAmmo > 0) {
			if(currentClip < maxClipSize) {
				master.SendMessage ("reload");
				currentClip++;
				currentAmmo--;
				time = Time.fixedTime;
				print(currentClip);
				print(currentAmmo + "|" + maxAmmo);
			}	
		}
	}
	
	//Sets MoveSpeed to SprintSpeed and decreases stamina by exhaustionFactor.
	//If stamina reaches zero, set MoveSpeed to WalkSpeed.
	void Sprint() {
		if(stamina > 0) {
			MoveSpeed = SprintSpeed;
			stamina = stamina - (maxStamina * exhaustionFactor);
		} else {
			MoveSpeed = WalkSpeed;
			stamina = 0;
		}
	}
	
	public Rigidbody dart;
	//Checks to see if bullet is available, if so, remove one from clip and spawn a bullet.
	void Shoot() {
		if(currentClip > 0) {
			currentClip = currentClip - 1;
			time = Time.fixedTime;
			print(currentClip);
			print(currentAmmo + "|" + maxAmmo);
			//Spawn/shoot a dart
			
			//Rigidbody clone = Instantiate(dart, transform.position, lastDirection) as Rigidbody;
			Rigidbody clone = Instantiate(dart) as Rigidbody;
			clone.transform.position = this.transform.position;
			float direction = 0;
			if(lastDirection == Vector3.forward) {
				direction = 0;
			} else if(lastDirection == Vector3.right) {
				direction = 90;
			} else if(lastDirection == Vector3.back) {
				direction = 180;
			} else if(lastDirection == Vector3.left) {
				direction = 280;
			}
			
			clone.transform.eulerAngles = new Vector3(90,direction,0);
		}
	}
	
	void ThrowSock() {
		if(socks > 0) {
			socks = socks - 1;
		}
	}
	
	void DrinkWater() {
		if(water > 0) {
			stamina = stamina + (waterRegenerationFactor * maxStamina);
			water = water - 1;
		}
	}
	
	void PickUpAmmo() {
		if(currentAmmo == maxAmmo) {
		} else {
			currentAmmo = currentAmmo + 1;
		}
	}
	
	void PickUpWater() {
		if(water == maxWater) {
		} else { 
			water = water + 1; 
		}
	}
}
