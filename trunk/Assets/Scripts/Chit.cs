using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Chit : MonoBehaviour {

	/*
	 * the basic class attached to all playing pieces on the planet
	 */
	
	public Tile tile;
	public Faction faction;
	public bool canMoveThisTurn = false;
	public int movementRange;
	public List<Terrains> canEnterTerrains = new List<Terrains>();
	public ChitTypes ctype;

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
			Debug.Log (pointingToTile.midpoint.ToString());
			LineRenderer arrow = this.GetComponent<LineRenderer>();
			arrow.SetPosition(0, tile.midpoint);		
			arrow.SetPosition(1, pointingToTile.midpoint);
			
		}
	}
	
	public void MovementArrowOn()
	{
		Debug.Log ("switching arrow on");
		updatingMovementArrow = true;		
		LineRenderer arrow = this.GetComponent<LineRenderer>();
		arrow.enabled = true;
		
		arrow.SetVertexCount(2);
		arrow.SetPosition(0, transform.position);		
		arrow.SetPosition(1, transform.position);
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
