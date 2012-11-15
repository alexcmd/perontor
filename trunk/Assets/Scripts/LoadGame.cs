using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


//using System;

public class LoadGame : MonoBehaviour {

	/*
	 * this is used to set up a game for a client player!
	 */
	

	int numFactions;	
	
	Planet planet;
	
	
	System.IO.StreamReader planetFile;
	string planetFileName;
	
	
	int turnCtr;
	// Use this for initialization
	void Start () {
		
		planet = gameObject.AddComponent<Planet>();	
		 /* 
		  * really, i need to improve this so player choses file, 
		  * and then factionID is loaded from file
		  */
		int factionID = this.GetComponentInChildren<MainMenu>().factionID;
		int turn = 		this.GetComponentInChildren<MainMenu>().turn;
		string folder = this.GetComponentInChildren<MainMenu>().saveGamePath;
		planetFileName = folder + "Perontor_faction" + factionID.ToString() + "_turn" + turn.ToString() + ".world";
		planetFile =	new System.IO.StreamReader(planetFileName);
		string myString = planetFile.ReadLine();
		planet.geometry.numSubDivides = int.Parse(myString);
		
		planet.geometry.Sqrt3Subdivsion();
		/* 
		 * the first nmbrHexs values in vertices give the midpoints for the hex tiles.
		 * collect these and apply terraforming to them to get altitudes
		 */		
		float[] alts = SaveLoad.LoadPlanet1(planet, planetFile);
		
		// sort out ocean	
		
		planet.geometry.scaleOcean();
			
		planet.geometry.PlaceTiles(alts);	
		
		SaveLoad.LoadPlanet2(planet, planetFile);
		planet.playerFaction = planet.factions[factionID];
		/*planet.f = f;
		planet.tiles = tiles;
		
		planet.turnCtr = turnCtr;
		if (turn!=turnCtr)
		{
			Debug.Log ("something wrong with turn ctr!");
		}*/
		planetFile.Close();
		
		planet.StartGame();
		
		Destroy(this);		
	}
			
	
	
}
	
	
