using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ZombieAI : MonoBehaviour {
	
	public GameObject player;
	public float maxSpeed;
	public float viewDistance;
	public int maxDepth;
	
	bool tracking = false;
	public float centerHeight;
	
	public int framesToPath;
	public int frameCounter = 1;
	
	public LinkedList<PathNode> currentPath = new LinkedList<PathNode>();
	public LinkedList<PathNode> nextPath = new LinkedList<PathNode>();
	public LinkedListNode<PathNode> currentTranslation;
	public Vector3 wanderTarget = Vector3.zero;
	public float wanderLimit;
	
	//used to change wander if stuck trying to go through wall
	bool wallFix = true;
	
	// Use this for initialization
	void Start () 
	{
		centerHeight = this.transform.localScale.y/2; 
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	
	void FixedUpdate()
	{
		if(seeTarget(this.transform.position, player.transform.position))
		{
			if(tracking)
			{
				frameCounter--;
				if(frameCounter == 0)
				{
					StartCoroutine(pathtoTarget(this.transform.localPosition, player.transform.localPosition));
					frameCounter = framesToPath;
				}
				
				if(currentPath.Count == 0)
				{
					currentPath = new LinkedList<PathNode>(nextPath);	
					currentTranslation = currentPath.First;
				}
				else
				{
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
			}
			else
			{
				frameCounter = 0;
				if(frameCounter == 0)
				{
					StartCoroutine(pathtoTarget(this.transform.position, player.transform.localPosition));
					frameCounter = framesToPath;
				}
				
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
			}
		}
		else
		{			
			if(wanderTarget == Vector3.zero)
			{
				changeWanderTarget();
			}
			
			if(getDistance(wanderTarget, this.transform.position) < Time.fixedDeltaTime*maxSpeed*2)
			{
				changeWanderTarget();
			}
			
			if(tracking)
			{
				frameCounter = 1;
				tracking = false;
			}
			
			frameCounter--;
			if(frameCounter == 0)
			{
				StartCoroutine(pathtoTarget(this.transform.position, wanderTarget));
				frameCounter = framesToPath;
			}
			
			if(currentPath.Count == 0)
			{
				currentPath = new LinkedList<PathNode>(nextPath);	
				currentTranslation = currentPath.First;
			}
			else
			{
				if(currentTranslation != null)
				{
					gameObject.transform.position = currentTranslation.Value.pos;	
					currentTranslation = currentTranslation.Next;
				}
				else
				{	
					if(getDistance(currentPath.First.Value.pos, currentPath.Last.Value.pos) < Time.fixedDeltaTime*maxSpeed*5 && wallFix)
					{
						changeWanderTarget();	
						wallFix = false;
					}
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
	
	void changeWanderTarget()
	{
		print ("change wander target");
		wanderTarget = new Vector3(Random.Range(this.transform.position.x - wanderLimit, this.transform.position.x + wanderLimit),
										centerHeight*2,
										Random.Range(this.transform.position.z - wanderLimit, this.transform.position.z + wanderLimit));
	}

	IEnumerator pathtoTarget(Vector3 start, Vector3 end)
	{
		LinkedList<PathNode> listChecked = new LinkedList<PathNode>();
		LinkedList<PathNode> listToCheck = new LinkedList<PathNode>();
		
		PathNode pathNode = new PathNode(null, start, 0, getDistance(start, end));
		LinkedListNode<PathNode> current = new LinkedListNode<PathNode>(pathNode);
		
		listToCheck.AddFirst(current);
		
		LinkedListNode<PathNode> newNode;
		SearchObject path;
		Vector3 endLocation;
		LinkedListNode<PathNode> temp;
		LinkedListNode<PathNode> root;
		
		float timePass = Time.fixedDeltaTime;
		
		bool found = false;
		
		Vector3 sweep = Vector3.zero;
		int j = maxDepth;
		int i;
		while(listToCheck.Count != 0 && j > 0 && !found)
		{
			current = listToCheck.First;
			//print(current.Value.distToTarget);
			listToCheck.RemoveFirst();
			
			for(i = 0; i < 4; i++)
			{
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
				
				path = checkForBlock(current.Value.pos, current.Value.pos + endLocation, sweep);
				
				if(path.status == 0)
				{
					pathNode = new PathNode(current.Value, current.Value.pos + endLocation, 
						current.Value.totalDist + getDistance(current.Value.pos, current.Value.pos + endLocation), 
						getDistance(current.Value.pos + endLocation, end));
					newNode = new LinkedListNode<PathNode>(pathNode);	
					
					if(!checkIfExists(listChecked, newNode))
					{												
						addInRightSpot(listToCheck.First, newNode, listToCheck);
					}
				}
				else if(path.status == 2)
				{
					pathNode = new PathNode(current.Value, current.Value.pos + endLocation, 
						current.Value.totalDist + getDistance(current.Value.pos, current.Value.pos + endLocation), 
						getDistance(current.Value.pos + endLocation, end));
					newNode = new LinkedListNode<PathNode>(pathNode);	
					
					if(!checkIfExists(listChecked, newNode))
					{
						addInRightSpot(listToCheck.First, newNode, listToCheck);
					}
					
					//return newNode.Value;
					nextPath = returnPath(newNode);
					found = true;
					//current = newNode;
					break;
				}
			}
			listChecked.AddLast(current);
			j--;
		}
		
		if(found)
		{
			current = listToCheck.First;
			listToCheck.RemoveFirst();	
		}
		
		if(!found)
		{
			nextPath = returnPath(current);
		}
		
		yield return null;
	}
	
	LinkedList<PathNode> returnPath(LinkedListNode<PathNode> last)
	{
		PathNode pathN;
		LinkedListNode<PathNode> temp;
		LinkedList<PathNode> path = new LinkedList<PathNode>();
		
		path.AddFirst(last.Value);
		last = path.First;
		while(last.Value.prev != null)
		{
			pathN = new PathNode(last.Value.prev);
			path.AddFirst(pathN);
			last = new LinkedListNode<PathNode>(pathN);
		}
		
		return path;
	}
	
	void addInRightSpot(LinkedListNode<PathNode> root, LinkedListNode<PathNode> newNode, LinkedList<PathNode> list)
	{
		LinkedListNode<PathNode> temp;
		
		while(root != null)
		{
			if(newNode.Value.distToTarget < root.Value.distToTarget)
			{
				root.List.AddBefore(root, newNode);
				return;
			}
			
			root = root.Next;
		}
		if(list.Count != 0)
		{
			list.AddLast(newNode);
		}
		else
		{
			list.AddFirst(newNode);	
		}
	}
	
	bool checkIfExists(LinkedList<PathNode> list, LinkedListNode<PathNode> node)
	{
		float dist;
		LinkedListNode<PathNode> check = list.First;
		while(check != null)
		{
			dist = (check.Value.pos - node.Value.pos).magnitude;
			if(dist < Time.fixedDeltaTime*maxSpeed)
			{
				return true;
			}
			check = check.Next;
		}
		
		return false;
	}
	
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
		
		
		if(Physics.Raycast(start + new Vector3(transform.localScale.x/2, 0, 0), direction, out hit, maxSpeed*Time.fixedDeltaTime))
		{			
			if(hit.collider.gameObject.tag == "Human")
			{
				print ("hit person");
				status = 2;	
			}
			else if(hit.collider.gameObject.tag == "Zombie")
			{
				status = 0;
			}
			else
			{
				//print ("hit: " + hit.point);
				status = 1;
			}
			
			return(new SearchObject(status, hit));
		}
		else if(Physics.Raycast(start + new Vector3(-transform.localScale.x/2, 0, 0), direction, out hit, maxSpeed*Time.fixedDeltaTime))
		{
			if(hit.collider.gameObject.tag == "Human")
			{
				print ("hit person");
				status = 2;	
			}
			else if(hit.collider.gameObject.tag == "Zombie")
			{
				status = 0;
			}
			else
			{
				//print ("hit: " + hit.point);
				status = 1;
			}
			
			return(new SearchObject(status, hit));
		}
		else if(Physics.Raycast(start + new Vector3(0, 0, transform.localScale.z/2), direction, out hit, maxSpeed*Time.fixedDeltaTime))
		{
			if(hit.collider.gameObject.tag == "Human")
			{
				print ("hit person");
				status = 2;	
			}
			else if(hit.collider.gameObject.tag == "Zombie")
			{
				status = 0;
			}
			else
			{
				//print ("hit: " + hit.point);
				status = 1;
			}
			
			return(new SearchObject(status, hit));
		}
		else if(Physics.Raycast(start + new Vector3(0, 0, -transform.localScale.z/2), direction, out hit, maxSpeed*Time.fixedDeltaTime))
		{
			if(hit.collider.gameObject.tag == "Human")
			{
				print ("hit person");
				status = 2;	
			}
			else if(hit.collider.gameObject.tag == "Zombie")
			{
				status = 0;
			}
			else
			{
				//print ("hit: " + hit.point);
				status = 1;
			}
			
			return(new SearchObject(status, hit));
		}
		else
		{
			status = 0;
		}
		
		return(new SearchObject(status, hit));
	}
	
	
	
	
	
	
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
		
		/*
		if(!Physics.CapsuleCast(p1, p2, this.transform.localScale.x, transform.forward, out hit, moveDist))
		{
			this.transform.Translate(directionToMove * moveDist);	
		}
		*/
		
	}
	
	bool seeTarget(Vector3 startPos, Vector3 targetPos)
	{
		RaycastHit hit;
		if(Physics.Raycast(startPos, (targetPos - startPos).normalized, out hit, viewDistance))
		{
			if(hit.collider.gameObject.tag == "Human")
			{
				return true;	
			}
			return false;	
		}
		return true;
	}
	
	float getDistance(Vector3 start, Vector3 end)
	{
		float norm;		
		norm = Mathf.Abs(start.x - end.x);
		norm += Mathf.Abs(start.z - end.z);
		return norm;
	}
	
	float checkDistance()
	{
		float dist;
		dist = Mathf.Abs(this.transform.localPosition.x - player.transform.localPosition.x);
		//dist += Math.Abs(this.transform.localPosition.y - player.transform.localPosition.y);
		dist += Mathf.Abs(this.transform.localPosition.z - player.transform.localPosition.z);
		return dist;
	}
}
