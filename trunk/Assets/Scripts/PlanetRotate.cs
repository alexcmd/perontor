using UnityEngine;
using System.Collections;

public class PlanetRotate : MonoBehaviour {

	/* 
	 * This simply causes the planet to rotate, giving us a day/night 
	 * cycle. Purely cosmetic. Maybe it will grow into something with 
	 * real game mechanics at a later day. 
	 */
	
	public float rotSpeed = 0.1f;
	private float rot;
	
	void Start()
	{
		rot = rotSpeed;	
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		transform.RotateAround(Vector3.up, rot * Time.deltaTime);	
	}
	
	void OnMouseOver()		
	{
		// Stop rotation
		rot = 0;			
	}
	
	void OnMouseExit()
	{
		rot = rotSpeed;	
	}
}
