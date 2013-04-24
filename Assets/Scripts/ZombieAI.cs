using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ZombieAI : MonoBehaviour {
	
	//player to look for
	public GameObject player;
	//max speed of zombie
	public float maxSpeed;
	//how far the zombie can see
	public float viewDistance;
	//how many steps can be obtained for a path
	public int maxDepth;
	
	//if the zombie is currently chasing someone
	bool tracking = false;
	//height of zombie
	public float centerHeight;
	
	//number of fixedUpdates before finding a new path
	public int framesToPath;
	//keeps track of number of fixedUpdates until new path
	public int frameCounter = 1;
	
	//holds the current path for the zombie
	public LinkedList<PathNode> currentPath = new LinkedList<PathNode>();
	//stores the next possible path for the zombie
	public LinkedList<PathNode> nextPath = new LinkedList<PathNode>();
	//current translation zombie is taking
	public LinkedListNode<PathNode> currentTranslation;
	
	//where the zombie is going to wander
	public Vector3 wanderTarget = Vector3.zero;
	//how far the zombie can wander from it's last target
	public float wanderLimit;
	
	//used to change wander if stuck trying to go through wall
	bool wallFix = true;
	
	//time that zombie is frozen in seconds
	public float freezeTime;
	//when zombie is frozen
	private float freezeStart;
	//if the zombie is frozen
	private bool inFreeze;
	
	// Use this for initialization
	void Start () 
	{
		//set height for faster access, kinda not needed...
		centerHeight = this.transform.localScale.y; 
	}
	
	// Update is called once per frame
	void Update () 
	{
		//test to freeze zombie
		/*
		if(Input.GetKey(KeyCode.E)) 
		{
			print ("freeze");
			freeze();
		}
		*/
	}
	
	void FixedUpdate()
	{		
		//unfreezes zombie if it waited long enough
		if(inFreeze)	
		{
			if(Time.fixedTime - freezeStart > freezeTime)
			{
				inFreeze = !inFreeze;	
			}
		}
		
		//for when the zombie isn't frozen
		if(!inFreeze)
		{
			//if the zombie can see a person
			if(seeTarget(this.transform.position, player.transform.position))
			{
				//if the zombie is currently tracking that person
				if(tracking)
				{
					//decrements framcounter if it is zero, calculates a new path 
					frameCounter--;
					if(frameCounter == 0)
					{
						if(currentPath.Count != 0)
						{
							StartCoroutine(pathtoTarget(currentPath.Last.Value.pos, player.transform.localPosition));
							frameCounter = framesToPath;
						}
						else
						{
							StartCoroutine(pathtoTarget(this.transform.localPosition, player.transform.localPosition));
							frameCounter = framesToPath;
						}
					}
					
					//put the next path into the current one if the current one is empty
					if(currentPath.Count == 0)
					{
						currentPath = new LinkedList<PathNode>(nextPath);	
						currentTranslation = currentPath.First;
					}
					else
					{
						//if the next move isn't empty, do the move and get the next move ready
						if(currentTranslation != null)
						{
							gameObject.transform.position = currentTranslation.Value.pos;	
							currentTranslation = currentTranslation.Next;
						}
						else
						{
							//put the next path into the current one
							currentPath = new LinkedList<PathNode>(nextPath);	
							currentTranslation = currentPath.First;
						}
					}
				}
				else
				{
					//make sure to make a new path to the human
					tracking = true;
					frameCounter = 0;
					if(frameCounter == 0)
					{
						StartCoroutine(pathtoTarget(this.transform.position, player.transform.localPosition));
						frameCounter = framesToPath;
					}
					
					//clears out these values so it doesn't move until it has a path
					nextPath = new LinkedList<PathNode>();
					currentPath = new LinkedList<PathNode>();
					currentTranslation = null;
					
					/*
					if(currentPath.Count == 0)
					{
						currentPath = new LinkedList<PathNode>(nextPath);	
						currentTranslation = currentPath.First;
					}
					else
					{
						tracking = true;
						if(currentTranslation != null)
						{
							gameObject.transform.position = currentTranslation.Value.pos;	
							currentTranslation = currentTranslation.Next;
						}
						else
						{
							currentPath = new LinkedList<PathNode>(nextPath);	
							currentTranslation = currentPath.First;
						}
					}
					*/
				}
			}
			//if the zombie doesn't see anyone
			else
			{	
				//if the wanderTarget is empty, set it
				//I realize this doesn't allow the spot(0,0,0), that should be ok, it can get infinetly close to it
				if(wanderTarget == Vector3.zero)
				{
					changeWanderTarget();
				}
				
				//if the zombie is within a movement of the wanderTarget, it find a new target
				if(getDistance(wanderTarget, this.transform.position) < Time.fixedDeltaTime*maxSpeed*2)
				{
					changeWanderTarget();
				}
				
				//switches tracking to false, because it can't track if it can't see
				if(tracking)
				{
					//frameCounter = 1;
					tracking = false;
				}
				
				//decrement frameCounter
				frameCounter--;
				if(frameCounter == 0)
				{
					//get a new path from the end of the current one
					if(currentTranslation != null)
					{
						print ("new Path");
						StartCoroutine(pathtoTarget(currentPath.Last.Value.pos, wanderTarget));
						frameCounter = framesToPath;
					}
					//get a path from the current position if it has not current movement
					else
					{
						StartCoroutine(pathtoTarget(this.transform.position, wanderTarget));
						frameCounter = framesToPath;
					}
				}
				
				//if the current path is empty, put the next path into it
				if(currentPath.Count == 0)
				{
					currentPath = new LinkedList<PathNode>(nextPath);	
					currentTranslation = currentPath.First;
				}
				else
				{
					//if the next move isn't empty, do the move and get the next move ready
					if(currentTranslation != null)
					{
						gameObject.transform.position = currentTranslation.Value.pos;	
						currentTranslation = currentTranslation.Next;
					}
					else
					{	
						//used if the zombie gets stuck on a wall
						//changes where he is going
						if(getDistance(currentPath.First.Value.pos, currentPath.Last.Value.pos) < Time.fixedDeltaTime*maxSpeed*10 && wallFix)
						{
							changeWanderTarget();	
							wallFix = false;
						}
						//put the next path into the current one
						else
						{
							print ("switch");
							currentPath = new LinkedList<PathNode>(nextPath);	
							currentTranslation = currentPath.First;
							wallFix = true;
						}
					}
				}
			}
			
			/*
			 * old really basic way of
			float dist = checkDistance();
			if(dist < viewDistance)
			{
				tracking = true;
				chaseHuman(player);
			}
			else
			{
				tracking = false;	
			}
			*/
		}
	}
	
	//function to change where the zombie is wondering to
	void changeWanderTarget()
	{
		print ("change wander target");
		wanderTarget = new Vector3(Random.Range(this.transform.position.x - wanderLimit, this.transform.position.x + wanderLimit),
										centerHeight,
										Random.Range(this.transform.position.z - wanderLimit, this.transform.position.z + wanderLimit));
	}
	
	//function to be used as a coroutine
	//given a start position and end position, it updates the nextPath list with a better path between the two locations
	//doesn't give a full path, but one that can be as deep as the maxDepth
	//gives the path based off of a gradient that chooses the next position based off of what is closer to the target (best first)
	IEnumerator pathtoTarget(Vector3 start, Vector3 end)
	{
		//list to keep track of whats been checked and what to check
		LinkedList<PathNode> listChecked = new LinkedList<PathNode>();
		LinkedList<PathNode> listToCheck = new LinkedList<PathNode>();
		
		//pathNode to be used to add values to LinkedListNodes
		//starts off at the start position
		PathNode pathNode = new PathNode(null, start, 0, getDistance(start, end));
		//LinkedListNode to add to lists
		LinkedListNode<PathNode> current = new LinkedListNode<PathNode>(pathNode);
		
		//add first position to list
		listToCheck.AddFirst(current);
		
		LinkedListNode<PathNode> newNode;
		SearchObject path;
		Vector3 endLocation;
		
		//get time for movement distance
		float timePass = Time.fixedDeltaTime;
		//checks if the path makes it to the destination
		bool found = false;
		
		//sweep is used for raycasting to test if something will be in the way
		Vector3 sweep = Vector3.zero;
		int j = maxDepth;
		int i;
		//while there are still spots to check go in the loop
		//stop loop if it goes past maxDepth
		//stop loop if the distination has been found
		while(listToCheck.Count != 0 && j > 0 && !found)
		{
			current = listToCheck.First;
			//print(current.Value.distToTarget);
			listToCheck.RemoveFirst();
			
			//checks the 4 cardinal directions from the given spot for the next possible spot to move
			for(i = 0; i < 4; i++)
			{
				//sets the end location and sweep vector based off of what direction to from the current position(up, down, left or right)
				endLocation = new Vector3(maxSpeed*timePass, 0, 0);
				sweep = Vector3.right;
				if(i == 1)
				{
					endLocation = new Vector3(-maxSpeed*timePass, 0, 0);
					sweep = -Vector3.right;
				}
				else if(i == 2)
				{
					endLocation = new Vector3(0, 0, maxSpeed*timePass);
					sweep = Vector3.forward;
				}
				else if(i == 3)
				{
					endLocation = new Vector3(0, 0, -maxSpeed*timePass);
					sweep = -Vector3.forward;
				}
				
				//checks if something will block the suggested movement
				path = checkForBlock(current.Value.pos, current.Value.pos + endLocation, sweep);
				
				//if nothing blocks the movement
				if(path.status == 0)
				{
					pathNode = new PathNode(current.Value, current.Value.pos + endLocation, 
						current.Value.totalDist + getDistance(current.Value.pos, current.Value.pos + endLocation), 
						getDistance(current.Value.pos + endLocation, end));
					newNode = new LinkedListNode<PathNode>(pathNode);	
					
					//check if spot was already found and if not add it the list of things to look from
					if(!checkIfExists(listChecked, newNode))
					{												
						addInRightSpot(listToCheck.First, newNode, listToCheck);
					}
				}
				//if a human blocks the path
				else if(path.status == 2)
				{
					pathNode = new PathNode(current.Value, current.Value.pos + endLocation, 
						current.Value.totalDist + getDistance(current.Value.pos, current.Value.pos + endLocation), 
						getDistance(current.Value.pos + endLocation, end));
					newNode = new LinkedListNode<PathNode>(pathNode);	
					
					//check if spot was already found and if not add it the list of things to look from
					if(!checkIfExists(listChecked, newNode))
					{
						addInRightSpot(listToCheck.First, newNode, listToCheck);
					}
					
					//sets that the human was found and breaks from the loop
					//return newNode.Value;
					//sets the next path as the one to the human
					nextPath = returnPath(newNode);
					found = true;
					//current = newNode;
					break;
				}
			}
			listChecked.AddLast(current);
			j--;
		}
		
		//if the human was found
		if(found)
		{
			current = listToCheck.First;
			listToCheck.RemoveFirst();	
		}
		//if the desitnation wasn't found use the last location it went to to make a new path
		if(!found)
		{
			nextPath = returnPath(current);
		}
		
		yield return null;
	}
	
	//given a destination LinkedListNode that contains a destination PathNode
	//it returns a list in order of the moves to get there
	LinkedList<PathNode> returnPath(LinkedListNode<PathNode> last)
	{
		PathNode pathN;
		LinkedList<PathNode> path = new LinkedList<PathNode>();
		
		path.AddFirst(last.Value);
		last = path.First;
		
		//while there is still a previous PathNode, keep adding it as a LinkedListNode to the front
		//of the list path
		while(last.Value.prev != null)
		{
			pathN = new PathNode(last.Value.prev);
			path.AddFirst(pathN);
			last = new LinkedListNode<PathNode>(pathN);
		}
		
		return path;
	}
	
	//adds a new node into the list given the root and the list
	//the list is a priority queue with nodes with a shorter distToTarget going first
	void addInRightSpot(LinkedListNode<PathNode> root, LinkedListNode<PathNode> newNode, LinkedList<PathNode> list)
	{		
		while(root != null)
		{
			//checks if the distToTarget is actually smaller then the current node in the list
			if(newNode.Value.distToTarget < root.Value.distToTarget)
			{
				root.List.AddBefore(root, newNode);
				return;
			}
			
			root = root.Next;
		}
		//if the new node should go at the end
		if(list.Count != 0)
		{
			list.AddLast(newNode);
		}
		//if the list is actually empty
		else
		{
			list.AddFirst(newNode);	
		}
	}
	
	//check if a given node exists in the given list
	//a given node is already in the list if the position of it is within one hop from another position
	bool checkIfExists(LinkedList<PathNode> list, LinkedListNode<PathNode> node)
	{
		float dist;
		
		//iterates over all of the nodes in the list
		LinkedListNode<PathNode> check = list.First;
		while(check != null)
		{
			//compare distances between 2 points
			//if within a hop of each other then they are the "same"
			dist = (check.Value.pos - node.Value.pos).magnitude;
			if(dist < Time.fixedDeltaTime*maxSpeed)
			{
				return true;
			}
			check = check.Next;
		}
		
		return false;
	}
	
	//function to check if a movement from a given position will hit a wall, human, etc
	//couldn't figure out CapsuleCast so just used a few RayCasts from the edges of the zombie
	//3 == something bad
	//2 == hit human
	//1 == something blocking path
	//0 == it's clear
	SearchObject checkForBlock(Vector3 start, Vector3 end, Vector3 direction)
	{		
		int status;
		RaycastHit hit = new RaycastHit();
		
		//if(Physics.CapsuleCast(start, end, this.transform.localScale.x/2, direction, out hit, (end - start).magnitude))
		
		if((start - end).sqrMagnitude == 0)
		{
			return(new SearchObject(3, hit));
		}
		
		/*
		 * didn't work from just center
		//if(Physics.Raycast(start, direction, out hit, (end - start).magnitude + transform.localScale.x/2))
		if(Physics.Raycast(start, direction, out hit, maxSpeed*Time.fixedDeltaTime + transform.localScale.x/2))
		{
			if(hit.collider.gameObject.tag == "Human")
			{
				print ("hit person");
				status = 2;	
			}
			else
			{
				//print ("hit: " + hit.point);
				status = 1;
			}
		}
		*/
		
		//check from right edge
		if(Physics.Raycast(start + new Vector3(transform.localScale.x/2, 0, 0), direction, out hit, maxSpeed*Time.fixedDeltaTime))
		{			
			//hit human
			if(hit.collider.gameObject.tag == "Human")
			{
				print ("hit person");
				status = 2;	
			}
			//hit zombie
			else if(hit.collider.gameObject.tag == "Zombie")
			{
				status = 0;
			}
			//hit a wall or something that it can't walk through
			else
			{
				//print ("hit: " + hit.point);
				status = 1;
			}
			
			//returns the status of the hit and the hit itself
			return(new SearchObject(status, hit));
		}
		//from left edge
		else if(Physics.Raycast(start + new Vector3(-transform.localScale.x/2, 0, 0), direction, out hit, maxSpeed*Time.fixedDeltaTime))
		{
			//hit human
			if(hit.collider.gameObject.tag == "Human")
			{
				print ("hit person");
				status = 2;	
			}
			//hit zombie
			else if(hit.collider.gameObject.tag == "Zombie")
			{
				status = 0;
			}
			//hit a wall or something that it can't walk through
			else
			{
				//print ("hit: " + hit.point);
				status = 1;
			}
			
			//returns the status of the hit and the hit itself
			return(new SearchObject(status, hit));
		}
		//from top edge
		else if(Physics.Raycast(start + new Vector3(0, 0, transform.localScale.z/2), direction, out hit, maxSpeed*Time.fixedDeltaTime))
		{
			//hit human
			if(hit.collider.gameObject.tag == "Human")
			{
				print ("hit person");
				status = 2;	
			}
			//hit zombie
			else if(hit.collider.gameObject.tag == "Zombie")
			{
				status = 0;
			}
			//hit a wall or something that it can't walk through
			else
			{
				//print ("hit: " + hit.point);
				status = 1;
			}
			
			//returns the status of the hit and the hit itself
			return(new SearchObject(status, hit));
		}
		//from bottom edge
		else if(Physics.Raycast(start + new Vector3(0, 0, -transform.localScale.z/2), direction, out hit, maxSpeed*Time.fixedDeltaTime))
		{
			//hit human
			if(hit.collider.gameObject.tag == "Human")
			{
				print ("hit person");
				status = 2;	
			}
			//hit zombie
			else if(hit.collider.gameObject.tag == "Zombie")
			{
				status = 0;
			}
			//hit a wall or something that it can't walk through
			else
			{
				//print ("hit: " + hit.point);
				status = 1;
			}
			
			//returns the status of the hit and the hit itself
			return(new SearchObject(status, hit));
		}
		//for no raycast hitting anything
		else
		{
			status = 0;
		}
		
		return(new SearchObject(status, hit));
	}
	
	//function to freeze a zombie
	void freeze()
	{
		//zombie can't get refrozen while frozen
		if(!inFreeze)
		{
			//makes the zombie frozen and records the time
			inFreeze = true;
			freezeStart = Time.fixedTime;
		}
	}
		
	
	//old bad zombie movement
	/*
	void chaseHuman(GameObject target)
	{
		RaycastHit hit;
		float xDist = player.transform.localPosition.x - this.transform.localPosition.x;
		float zDist = player.transform.localPosition.z - this.transform.localPosition.z;
		
		Vector3 p1 = transform.position;
		Vector3 p2 = transform.position;
		Vector3 directionToMove = Vector3.zero;
		float moveDist = 0;
		float moveDist1 = maxSpeed * Time.fixedDeltaTime * Mathf.Sign(xDist);
		float moveDist2 = maxSpeed * Time.fixedDeltaTime * Mathf.Sign(zDist);
		
		//choose which direction to move
		if(Mathf.Abs(xDist) < Mathf.Abs(zDist))
		{
			p2 = p1 + Vector3.right * moveDist1;
			
			if(xDist > moveDist1)
			{
				//print ("Right: 1");
				directionToMove = Vector3.right;
				moveDist = moveDist1;
			}
			else if(xDist <= moveDist1 && xDist > .001)
			{
				print ("Right: 2");
				moveDist = xDist;
				directionToMove = Vector3.right;
			}
			else
			{
				p2 = p1 + Vector3.up * moveDist2;
				if(zDist > moveDist2)
				{
					//print (zDist);
					//print ("Up: 1.2");
					directionToMove = new Vector3(0,0,1);
					moveDist = moveDist2;
				}
				else if(zDist <= moveDist2 && zDist > .001)
				{
					print ("Up: 2.2");
					directionToMove = new Vector3(0,0,1);
					moveDist = zDist;
				}
				else
				{
					print ("Staying Still");	
				}
			}
		}
		else if(Mathf.Abs(xDist) > Mathf.Abs(zDist))
		{
			p2 = p1 + Vector3.up * moveDist2;
			if(zDist > moveDist2)
			{
				print ("Up: 1");
				directionToMove = new Vector3(0,0,1);
				moveDist = moveDist2;
			}
			else if(zDist <= moveDist2 && zDist > .001)
			{
				print ("Up: 2");
				directionToMove = new Vector3(0,0,1);
				moveDist = zDist - .001f;
			}
			else
			{
				p2 = p1 + Vector3.right * moveDist1;
				if(xDist > moveDist1)
				{
					print ("Right: 1.2");
					directionToMove = Vector3.right;
					moveDist = moveDist1;
				}
				else if(xDist <= moveDist1 && xDist > .001)
				{
					print ("Right: 2.2");
					moveDist = xDist;
					directionToMove = Vector3.right;
				}
				else
				{
					print ("Staying Still");	
				}
			}
			
		}
		else
		{
			print ("Staying Still");	
		}
		
		this.transform.Translate(directionToMove * moveDist);
		

//		if(!Physics.CapsuleCast(p1, p2, this.transform.localScale.x, transform.forward, out hit, moveDist))
//		{
//			this.transform.Translate(directionToMove * moveDist);	
//		}

		
	}
	*/
	
	//checks if a zombie can see a human given the zombie's position and the human's position
	//returns true only if it hits the human
	bool seeTarget(Vector3 startPos, Vector3 targetPos)
	{
		RaycastHit hit;
		if(Physics.Raycast(startPos, (targetPos - startPos).normalized, out hit, viewDistance))
		{
			if(hit.collider.gameObject.tag == "Human")
			{
				return true;	
			}	
		}
		return false;
	}
	
	//gets distance between two positions using 1-norm in x and z
	//(absolute difference in x and z coordiantes)
	float getDistance(Vector3 start, Vector3 end)
	{
		float norm;		
		norm = Mathf.Abs(start.x - end.x);
		norm += Mathf.Abs(start.z - end.z);
		return norm;
	}
	
	//gets checks if the human is within the zombies range of vision
	float checkDistance()
	{
		float dist;
		dist = Mathf.Abs(this.transform.localPosition.x - player.transform.localPosition.x);
		//dist += Math.Abs(this.transform.localPosition.y - player.transform.localPosition.y);
		dist += Mathf.Abs(this.transform.localPosition.z - player.transform.localPosition.z);
		return dist;
	}
}
