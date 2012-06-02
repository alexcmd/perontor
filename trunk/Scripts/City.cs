using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Character))]

public class City : MonoBehaviour {
	
	public string cityName;
	public int range = 3;
	public ChitTypes currentyBuilding = ChitTypes.NULL; 
	public int turnsLeftToBuild = -1;
	
	public int pop = 10000;
	
	int turnsToBuildUnit = 2;
	int turnsToBuildCommander = 1;
	
	
	void Start()
	{
		//currentyBuilding = null;
	}
	
	public void UpdateBuilding()
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
				Debug.Log(c.faction.fname);
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
	}
}
