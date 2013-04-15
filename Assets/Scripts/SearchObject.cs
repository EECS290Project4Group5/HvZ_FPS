using UnityEngine;
using System.Collections;

public class SearchObject{

	public int status;
	public RaycastHit hit;
	
	public SearchObject(int status, RaycastHit hit)
	{
		this.status = status;
		this.hit = hit;
	}
	
}
