using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CreateNewGame : MonoBehaviour {
	
	// this class is used to create a brand new game!!!

	int numFactions;
	float terrainScale = 1.5f;
		
	public List<Faction> f = new List<Faction>(); // a list of all factions in game
	public string[] playerNames;
	
	Planet planet;
	
	string planetFileName;
	System.IO.StreamReader planetFile;
	
	
	// Use this for initialization
	void Start () {
		
		planet = gameObject.AddComponent<Planet>();
		
		Debug.Log(planet);
		Debug.Log(planet.geometry);
		numFactions = this.GetComponentInChildren<MainMenu>().numOfFactions;
		playerNames = this.GetComponentInChildren<MainMenu>().playerNames;
		
		planet.geometry.numSubDivides = this.GetComponentInChildren<MainMenu>().worldSize;
		Debug.Log (planet.geometry.numSubDivides.ToString());
		planet.geometry.Sqrt3Subdivsion();
		
		/* 
		 * the first nmbrHexs values in vertices give the midpoints for the hex tiles.
		 * collect these and apply terraforming to them to get altitudes
		 */		

		float[] alts = planet.geometry.CreateTerrain(terrainScale);			

		// sort out ocean	
	//	planet.geometry.scaleOcean();

		bool ready = planet.geometry.PlaceTiles(alts);	
		
		planet.planetSize = planet.geometry.numSubDivides;
		foreach (GameObject g in (GameObject.FindGameObjectsWithTag("Tile")))
		{
			planet.tiles.Add(g.GetComponent<Tile>());	
		}

		CreateFactions();
		
		SaveLoad.SaveWorld(planet, f);
		// remove this script!!!
		Destroy(this);		
	}
	
	public void CreateFactions()
	{
		/*
		 * Creates numFaction factions.
		 * We need to allow for different faction names, colours etc.
		 * Each faction currently starts off with one settlement,
		 * placed in a random location.
		 * 
		 * TODO: make sure settlement does not start underwater!
		 * TODO: create distinct factions with set colours and names!
		 */
		List<Tile> tiles = planet.tiles;
		Debug.Log("creating factions");
		
		// add one to number of factions, so we have the non-playable Nex!
		numFactions = numFactions+1;
		
		Faction faction = CreateTheLurkingNec();
		f.Add(faction);
		f[0].fname = "NEC";

		for (int c=1; c<numFactions; c++)
		{
			Debug.Log("creating faction " + c.ToString());
			faction = new Faction();
			faction.fcol = Color.magenta;
			faction.id = c;
			f.Add(faction);
			switch (c)
			{
			case 1:
				faction.fcol = Color.red;
				break;
			case 2:
				faction.fcol = Color.blue;
				break;
			case 3:
				faction.fcol = Color.yellow;
				break;
			case 4:
				faction.fcol = Color.cyan;
				break;
			default:
				faction.fcol = Color.white;
				break;					
			}
			
			f[c].fname = playerNames[c-1];
				
			int startingLoc  = 0;
			bool needToKeepLooking = true;
			while (needToKeepLooking)
			{
				// generate a random starting location
				startingLoc = Random.Range(0, tiles.Count);
				// check starting location is GOOD
				if (tiles[startingLoc].terrain == Terrains.GRASS & tiles[startingLoc].tileType == TileType.HEX)
				{
					// check tile is empty!
					if(tiles[startingLoc].chitOnTile == null)
					{
						needToKeepLooking = false;
					}
				}				
			}
			tiles[startingLoc].AttachChit(f[c], ChitTypes.CITY, "faction " + c.ToString() + " city");
			// now lets out down an initial chit too!
			int neighbourID = tiles[startingLoc].nbrTiles[1];
			tiles[neighbourID].AttachChit(f[c], ChitTypes.SWORD, null); 
		}	
		
		// make sure Factions recognise that they have settlements
		for (int c=0; c<numFactions; c++)
		{
			f[c].UpdateListOfControllers();	
		}		
	}
	
	private Faction CreateTheLurkingNec()
	{
		List<Tile> tiles = planet.tiles;
		Faction necFaction = new Faction();
		necFaction.fname = "The Lurking Nex";
		bool lookingForNecsStartingLocation = true;
		int necsStartingLoc = 0;
		while (lookingForNecsStartingLocation)
		{
			// generate a random starting location
			necsStartingLoc = Random.Range(0, tiles.Count);
			// check random starting location is GOOD!
			if (tiles[necsStartingLoc].terrain == Terrains.SAND | tiles[necsStartingLoc].terrain == Terrains.ICE)
			{
				lookingForNecsStartingLocation = false;
			}				
		}
		tiles[necsStartingLoc].AttachChit(necFaction, ChitTypes.NECTOWER, "EvilNec");	
		return necFaction;
	}

}
