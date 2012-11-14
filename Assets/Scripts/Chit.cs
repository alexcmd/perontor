using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Chit : MonoBehaviour {

	/*
	 * the basic class attached to all playing pieces on the planet
	 */
	
	public Tile tile;
	public Faction faction;
	
	// stats
	public ChitTypes ctype;
	public bool cornerFacing;
	public int movementRange;
	public List<Terrains> canEnterTerrains = new List<Terrains>();
	public bool requires_influence;
	public int str; 
	
	public bool canMoveThisTurn = false;
	bool updatingMovementArrow = false;
	/* not used yet, but no doubt we'll come up with some stats soon enough
	* for example:
	* int health;
	* int movement;
	* int attack;
	* int defense;
	*/
	
	void Start()
	{
		canEnterTerrains.Add(Terrains.GRASS);
		canEnterTerrains.Add(Terrains.FOREST);
		canEnterTerrains.Add(Terrains.ICE);
		canEnterTerrains.Add(Terrains.ROCK);
		canEnterTerrains.Add(Terrains.SAND);
	}
	
	void Update()
	{
		if (updatingMovementArrow)
		{
			Tile pointingToTile = transform.parent.GetComponent<Planet>().mouseOverTile;			
			LineRenderer arrow = this.GetComponent<LineRenderer>();
			
			// draw curved arrow line thing!¬
			for (int p=0; p < 30; p++)
			{
				float a = (float)(225-(p-15)*(p-15))/100;
				Vector3 pt = a * Vector3.Lerp(tile.midpoint, pointingToTile.midpoint, (float)p/29);
				arrow.SetPosition(p, pt);
			}	
		}
	}
	
	public void MovementArrowOn()
	{
		Debug.Log ("switching arrow on");
		updatingMovementArrow = true;		
		LineRenderer arrow = this.GetComponent<LineRenderer>();
		arrow.enabled = true;
		
		arrow.SetVertexCount(30);
		arrow.SetPosition(0, transform.position);		
		arrow.SetPosition(1, transform.position);
	}
	
	public void MovementArrowDone()
	{
		updatingMovementArrow = false;	
	}
	
	public void SetMoveFlag(bool b)
	{
		if (transform.tag != "City")	
		{
			canMoveThisTurn = b;	
		}
	}
	
	void OnMouseDown()
	{
	//	tile.ClickedOnATileWithAChit();	
	}
		
	 
    void OnCollisionEnter() {
       
    }
}
