using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Character))]

public class City : MonoBehaviour {
	
	public int range = 3;
	
	public int pop = 1000;
	public float growthrate = 0.3f;	
	
	public int enlistCost = 100;
	public int enlistCount = 0;
	
	void Start()
	{
		
	}
	
/*	public void UpdateBuilding()
	{
		UI ui = GameObject.Find("Planet").GetComponent<UI>();	
		if (currentyBuilding != ChitTypes.NULL)
		{
			turnsLeftToBuild--;

			if (turnsLeftToBuild < 1)
			{
				// we have build a new unit!
				Debug.Log("creating new unit! - " + currentyBuilding.ToString());
				Chit c = gameObject.GetComponent<Chit>();
				Tile t = c.tile;
				ui.displayNewUnitBuiltWindow = true;
				// create new unit
				t.AttachChit(c.faction, currentyBuilding, "new unit");
				// pre-select new unit for ordering
				t.chitOnTile.canMoveThisTurn = true;			
				currentyBuilding = ChitTypes.NULL;
			}
		}
	}

	public void StartBuilding(ChitTypes unitToBuild) 
	{
		currentyBuilding = unitToBuild;	
		switch (unitToBuild)
		{
		case ChitTypes.SWORD:
			turnsLeftToBuild = turnsToBuildUnit;
			break;
		case ChitTypes.COMMANDER:
			turnsLeftToBuild = turnsToBuildCommander;
			break;
		}
	}*/
	
/*	void OnMouseDown()
	{
		if (this.GetComponentInChildren<Chit>().faction == transform.parent.GetComponent<Planet>().playerFaction)
		{
			transform.parent.GetComponent<Planet>().ui.DisplayCityInfo(this);
		}
	}
	*/
	public void Enlist()
	{
		if ( pop > enlistCost )
		{
			enlistCount++;
			pop -= enlistCost	;
			transform.parent.GetComponent<Planet>().orders.EnlistOrder(this);
		}
	}
}
