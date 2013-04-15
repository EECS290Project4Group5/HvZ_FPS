using UnityEngine;
using System.Collections;

public class PathNode{
	
	public PathNode prev;
	public Vector3 pos;
	public float totalDist;
	public float distToTarget;
	
	public PathNode()
	{
		this.prev = null;
		this.pos = Vector3.zero;
		this.totalDist = 0;
		this.distToTarget = float.PositiveInfinity;
	}
	
	public PathNode(PathNode newNode)
	{
		this.prev = newNode.prev;
		this.pos = newNode.pos;
		this.totalDist = newNode.totalDist;
		this.distToTarget = newNode.distToTarget;
	}
	
	public PathNode(PathNode prev, Vector3 pos, float dist, float dist2)
	{
		this.prev = prev;
		this.pos = pos;
		this.totalDist = dist;
		this.distToTarget = dist2;
	}
	
	public void SetVals(PathNode prev, Vector3 pos, float dist, float dist2)
	{
		this.prev = prev;
		this.pos = pos;
		this.totalDist = dist;
		this.distToTarget = dist2;
	}
}
