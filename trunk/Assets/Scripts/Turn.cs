using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Turn : MonoBehaviour {
	
	/* This class is in charge of controlling the game flow. 
	 * Whose turn us it? Which chit is currently active, etc
	 * 
	 * The turn structure is a little odd (interesting!). 
	 * Factions take it in turns.
	 * On each turn, one of their controllers/settlements are activated this allows 
	 * them to give orders to that character/settlement, and give order to normal 
	 * chits that are near by (command radius or something)
	 * 
	 * Each faction has a "cycle" which is the number of turns needed to cycle through
	 * all their controllers/settlements. So the more you have, the longer it will take
	 * you to complete a cycle.
	 * 
	 * Economic stuff (building things in settlements, research, etc) take place per 
	 * cycle. 
	 * 
	 * At the start of each cycle, the game randomly re-orders your controllers and 
	 * settlements. 
	 * */
	
	Planet planet;
	GameObject activeRegionHoop;
	public CamControl camControls; 	// does this need to be public???
	
	int numFactions; 				// number of factions
	public int currentFctr = 0; 	// used to keep track of whose turn it is
	public Controller currentChar;	// the currently active character/settlement
	
	public int turn = 0;
	
	List<int> activeTiles = new List<int>();

	void Start() 
	{
		Debug.Log ("switching on Turn");
		camControls = Camera.main.transform.GetComponent<CamControl>();
		planet = GameObject.Find("Planet").GetComponent<Planet>();

		// Get the ActiveRegionHoop, and set it up
		activeRegionHoop = (GameObject)GameObject.Find("ActiveRegionHoop");
		activeRegionHoop.GetComponent<ActiveRegionHoop>().setPlanet(planet);
		
//		planet.turn = this;
		numFactions = planet.factions.Count;
		
		// randomly pick who gets first turn
		currentFctr = Random.Range(0, numFactions);

		planet.factions[currentFctr].cycle++;
		
	//	UpdateTurn();

	}
	
	public void EndTurn()
	{
		Debug.Log ("End Turn");
	}
		
	public void UpdateTurn () 
	{
		turn++;
		currentFctr = (currentFctr)%(numFactions-1)+1;
		Faction f = planet.factions[currentFctr];
		Debug.Log("start of UpdateTurn, t = " + f.step.ToString());
//		planet.ui.UpdatePanelColour(f.fcol);
		
		planet.ResetTileMovementRangeFlag();
		f.step = (f.step+1)%f.controllers.Count;
		if (f.step+1 == f.controllers.Count)
		{
			f.cycle++;			
			f.UpdateListOfControllers();
			f.UpdateListOfChits();
			Debug.Log("Num chars found:" + f.controllers.Count.ToString());
		}
		
		
		Debug.Log("end of UpdateTurn, t = " + f.step.ToString());
		

	}	
	
	void SetActiveTiles(int tileID)
	{
		//switch off old active tiles!
		for (int t=0; t<activeTiles.Count; t++)
		{
			planet.tiles[activeTiles[t]].UnActivateTile();		
		}		
		// get new active tiles
		activeTiles = planet.GetTilesWithinRange(tileID, currentChar.range);
		for (int t=1; t<activeTiles.Count; t++)
		{
			planet.tiles[activeTiles[t]].ActivateTile();
		}
		
		//this.GetComponent<ActiveRegionHoop>().SetPerimiterTiles(activeTiles, planet.tiles[tileID].midpoint);
		activeRegionHoop.GetComponent<ActiveRegionHoop>().SetPerimiterTiles(activeTiles, planet.tiles[tileID]);

		camControls.CentreCamera(currentChar.GetComponent<Chit>().tile);
	}
	
	
}
