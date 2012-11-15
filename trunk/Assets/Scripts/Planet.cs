using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;



public enum Terrains
{
	GRASS, 
	ROCK, 
	SAND, 
	WATER, 
	ICE, 
	FOREST
}

public enum TileType
{
	PENT,
	HEX
}

public enum ChitTypes
{
	NULL, 		// I'm not sure why we'd need a NULL
	
	CITY, 		// settlements.. stationary controller unit. Can build.
	COMMANDER,  // mobile controller unit
	FLAGSHIP,   // naval controller unit
	
	SWORD,		// basic ground unit	
	KNIGHT,		// alt ground unit
	SHIP,		// basic naval unit
	
	SCOUT,		// ground unit - doesn't need influence
	BUILDER, 	// builds new cities - doesn't need influence
	
	SHRINE, 	// capture these for points and perks
	NECTOWER 	// the evil Nec!
	
}

public class Planet : MonoBehaviour {
	
	/* 
	 * this generates new planets, saves, loads, and keeps track of the tiles.
	 * 
	 * 
	 */

	public int 			planetSize; // the number of subdivides used in CreateHexPlanet
	public float		seaLevel;
	
	public string 		savegameFolder;//
	public Orders orders;
	
	public List<Tile> tiles = new List<Tile>();  	// list of all the tiles on the planet
	public List<int> selTiles = new List<int>();  	// list of all the tiles that are selected (probably)
	List<int> activeTiles = new List<int>();	  	// list of tiles withing character act radius
	
	public List<Faction> factions = new List<Faction>(); // a list of all factions in game
	public int numberOfFactions;
	public int playerFactionID; // faction currently being played
	public Faction playerFaction;
	
	public int turnCtr = 1;
	
	public Controller currentChar;	// the currently active character/settlement
	
	public Chit selectedChit; // do I really need selectedChit and orderingChit?
	public Chit orderingChit = null;
	
	public Tile mouseOverTile; // 
	
	public CamControl camControls; 	// does this need to be public???
	public UI ui;
	
	public bool isHost = false;
	
	public PlanetaryGeometry geometry;
	
	GameObject activeRegionHoop;
	
	List<Tile> selectedTiles = new List<Tile>(); //  used for passing lists of triangles/tiles to submeshs to change highlighted tiles
	
	void Awake()
	{
		Debug.Log("making geometry");
		geometry = gameObject.AddComponent<PlanetaryGeometry>();
	}
	
	void Start()
	{
		isHost 			= this.GetComponentInChildren<MainMenu>().isHost;
		playerFactionID	= this.GetComponentInChildren<MainMenu>().factionID;
		savegameFolder  = this.GetComponentInChildren<MainMenu>().saveGamePath;		
	}
	
		
	public void StartGame()
	{
//		orders 			= gameObject.AddComponent<Orders>();
		Debug.Log("switching on UI");
		ui = gameObject.AddComponent<UI>();
		GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().enabled = true;
		GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CamControl>().enabled = true;

		ui.gameLoadedSummaryWindow = true;
	//	
		camControls 	= Camera.main.transform.GetComponent<CamControl>();
		
		// update lists of chits and controllers
		for (int f=0; f<factions.Count; f++)
		{
			factions[f].UpdateListOfChits();
			factions[f].UpdateListOfControllers();
		}
		
		
		currentChar 			= playerFaction.controllers[playerFaction.step];
		currentChar.remainingCR = currentChar.commandRating;

		numberOfFactions = factions.Count;
		
		int tileID = currentChar.GetComponent<Chit>().tile.id;
		// Get the ActiveRegionHoop, and set it up
		activeRegionHoop = (GameObject)GameObject.Find("ActiveRegionHoop");
		activeRegionHoop.GetComponent<ActiveRegionHoop>().setPlanet(this);
		SetActiveTiles(tileID);		
		
		
	}
	
	void SetActiveTiles(int tileID)
	{
		//switch off old active tiles!
		for (int t=0; t<activeTiles.Count; t++)
		{
			tiles[activeTiles[t]].UnActivateTile();		
		}		
		// get new active tiles
		activeTiles = GetTilesWithinRange(tileID, currentChar.range);
		for (int t=1; t<activeTiles.Count; t++)
		{
			tiles[activeTiles[t]].ActivateTile();
		}
		
		//this.GetComponent<ActiveRegionHoop>().SetPerimiterTiles(activeTiles, planet.tiles[tileID].midpoint);
		activeRegionHoop.GetComponent<ActiveRegionHoop>().SetPerimiterTiles(activeTiles, tiles[tileID]);
		//Debug.Log ("currentchar: " + currentChar.name);
		//Debug.Log (currentChar.GetComponent<Chit>().tile.id.ToString());
		camControls.CentreCamera(currentChar.GetComponent<Chit>().tile);
	}
	
	public void NewSelectedTiles(List<int> t)
	{
		//Debug.Log("NewSelectedTiles");
		for (int c = 0; c<selectedTiles.Count; c++)
		{
			selectedTiles[c].ReapplyTerrainTexture();			
		}
		selectedTiles.Clear();
		for (int s=0; s<t.Count; s++)
		{
			selectedTiles.Add(tiles[t[s]]);
		}		
	}	
	
	
	public void UnHighLightTiles()
	{
		foreach (int t in selTiles)
		{
			tiles[t].ReapplyTerrainTexture();
			tiles[t].withinMovementRange = false;
		}
		selTiles.Clear();		
	}
	
	public List<int> GetTilesWithinRange(int id, int range)
	{
		Debug.Log ("total tiles: " + tiles.Count.ToString());
		/*
		 * version with no interest in terrain types
		 */
		List<int> activeTiles = new List<int>();
		int frontier = 0; // entries before frontier have already been checked!
		activeTiles.Add(id);
		// number of times to iterate... = range
		for (int r=0; r<range; r++)
		{			
			// for each tile on our frontier...
			List<int> tilesToAdd = new  List<int>();
			for (int t=frontier; t<activeTiles.Count; t++)
			{
				Tile frontierTile = tiles[activeTiles[t]];
				// add each tile tile that is adjacent to the frontier tile
				for (int n=0; n<frontierTile.nbrTiles.Count; n++)
				{

					int nID = frontierTile.nbrTiles[n];				
					tilesToAdd.Add(nID);

				}				
			}
			// now move frontier forward so we don't double check tiles
			frontier = activeTiles.Count;
			// now add int all new tiles
			for (int n=0; n<tilesToAdd.Count; n++)			
			{
				if (activeTiles.Contains(tilesToAdd[n]) == false)
				{
					activeTiles.Add(tilesToAdd[n]);
				}
			}
		}
		// add perimter counter to start of the List
		// yes, I know inserting is a bad idea, but it will do for now :-)
		activeTiles.Insert(0, frontier);
		return activeTiles;
	}
	
	public List<int> GetTilesWithinRange(int id, int range, List<Terrains> canEnterTerrains)
	{
		/* 
		 * version which only adds tiles to frontier if they are correct terrain types
		 */
		List<int> activeTiles = new List<int>();
		int frontier = 0; // entries before frontier have already been checked!
		activeTiles.Add(id);
		// number of times to iterate... = range
		for (int r=0; r<range; r++)
		{			
			// for each tile on our frontier...
			List<int> tilesToAdd = new  List<int>();
			for (int t=frontier; t<activeTiles.Count; t++)
			{
				Tile frontierTile = tiles[activeTiles[t]];
				// add each tile tile that is adjacent to the frontier tile
				for (int n=0; n<frontierTile.nbrTiles.Count; n++)
				{
					if (canEnterTerrains.Contains(frontierTile.terrain))
					{
						int nID = frontierTile.nbrTiles[n];				
						tilesToAdd.Add(nID);
					}
				}				
			}
			// now move frontier forward so we don't double check tiles
			frontier = activeTiles.Count;
			// now add int all new tiles
			for (int n=0; n<tilesToAdd.Count; n++)			
			{
				if (activeTiles.Contains(tilesToAdd[n]) == false)
				{
					activeTiles.Add(tilesToAdd[n]);
				}
			}
		}
		// add perimter counter to start of the List
		// yes, I know inserting is a bad idea, but it will do for now :-)
		activeTiles.Insert(0, frontier);
		return activeTiles;
	}
		
	
//	public void UnhighlightTiles(int m)
//	{
	//	selTri = new int[0];
	//	Debug.Log("have switched of unhighlight tiles");
	//	mesh.SetTriangles(selTri, m);		
//	}
	
	public void ResetTileMovementRangeFlag()
	{
		/*
		  * set withinMovementRange = false for all tiles
		  * ie, call once a chit has been moved, or we are starting
		  * turn and the player is yet to click on a tile
		  */
		
		for (int t=0; t<tiles.Count; t++)
		{
			tiles[t].withinMovementRange = false;
		}
	}
	
	/* 
	 * Dull save/load stuff lives below this line
	 */
	

	/*
	public void LoadFactions()
	{	
		System.IO.StreamReader myFile =	new System.IO.StreamReader("PerontorFactions.txt");
		string myString;
		// get currentFctr
		myString = myFile.ReadLine();
		int turncurrentFctr = int.Parse(myString);
		
		// now get faction info
		myString = myFile.ReadLine();
		Debug.Log("number of factions = " + myString);
		
		numberOfFactions = int.Parse(myString); // number of factions in game
		int numCharsToRead;
		for (int t=0; t<numberOfFactions; t++)
		{
			Debug.Log("reading faction " + t.ToString());
			Faction faction = gameObject.AddComponent<Faction>(); // create new Faction
			f.Add(faction);		
			
			// read faction name
			f[t].fname = myFile.ReadLine(); 
			Debug.Log("now reading " + f[t].fname);
			
			// read number of characters
			myString = myFile.ReadLine();
			Debug.Log("There are " + myString + " characters to read");
			numCharsToRead = int.Parse(myString);	
			
			// read in turn and cycle numbers
			myString = myFile.ReadLine();
			f[t].turn = int.Parse(myString);
			myString = myFile.ReadLine();
			f[t].cycle = int.Parse(myString);			
			
			ReadInCharsForFaction(myFile, numCharsToRead,t);
			f[t].UpdateListOfCharacters();
		}

		myFile.Close();
		Debug.Log("loaded");	
		
		// now add UI
		ui = gameObject.AddComponent<UI>();
		turn = gameObject.AddComponent<Turn>();
		turn.currentFctr = turncurrentFctr;
	}

	void ReadInCharsForFaction(System.IO.StreamReader myFile, int numCharsToRead, int t)
	{
		for (int c=0; c<numCharsToRead; c++)
		{
			string cType = myFile.ReadLine();
			string characterName= myFile.ReadLine();
			int tileID = int.Parse( myFile.ReadLine());				
			// if character was a city, save city details
			if (cType == "City")
			{
				Debug.Log("reading city details");
				string currentlyBuilding = myFile.ReadLine();
				int turnsLeftToBuild = int.Parse(myFile.ReadLine());
				// now spawn unit
				tiles[tileID].AttachChit(f[t], cType, characterName);					
				if (currentlyBuilding == "null")
				{
					currentlyBuilding = null;
				}	
			}
		}	
	}*/
}


