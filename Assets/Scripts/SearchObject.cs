using UnityEngine;
using System.Collections;

//Author: Daniel Pfeffer dnp19
//object used to test what a zombie will hit while it's walking
public class SearchObject{

	public int status;
	public RaycastHit hit;
	
	public SearchObject(int status, RaycastHit hit)
	{
		this.status = status;
		this.hit = hit;
	}
	
}
