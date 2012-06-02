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
	NULL,
	CITY,
	COMMANDER,
	SWORD,
	SCOUT,
	NECTOWER,
	SHRINE	
}

public class Planet : MonoBehaviour {
	
	/* 
	 * this generates new planets, saves, loads, and keeps track of the tiles.
	 * 
	 * 
	 */

	public int planetSize = 5; // the number of subdivides used in CreateHexPlanet
	
	public List<Tile> tiles = new List<Tile>();  // an array of all the tiles on the planet
	public List<int> selTiles = new List<int>();  // an array of all the tiles on the planet
	
	public List<Faction> f = new List<Faction>(); // a list of all factions in game
	public int numberOfFactions;
	
	public Chit selectedChit; // do I really need selectedChit and orderingChit?
	public Chit orderingChit = null;
	
	public Tile mouseOverTile; // 
	
	public Turn turn;
	public UI ui;
	
	List<Tile> selectedTiles = new List<Tile>(); //  used for passing lists of triangles/tiles to submeshs to change highlighted tiles
	
	// texture codes, for references textures/meshes
	
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
	
	public void NewPlanet(int wS, int nF) 
	{		
		planetSize = wS;		
		foreach (GameObject g in (GameObject.FindGameObjectsWithTag("Tile")))
		{
			tiles.Add(g.GetComponent<Tile>());	
			g.GetComponent<Tile>().LinkPlanet(this);
		}
		CreateFactions(nF);
		ui = gameObject.GetComponent<UI>();
	//	SaveWorld();
	}
	
	public void SaveWorld()
	{
		// save world info
		System.IO.StreamWriter file = new System.IO.StreamWriter("Perontor.txt");
		string s;
		s =  planetSize.ToString();
		Debug.Log(s);
		file.WriteLine(s);	
		for (int v=0; v<tiles.Count; v++)
		{
			s = tiles[v].altitude.ToString();
			file.WriteLine(s);		
		}	
		for (int v=0; v<tiles.Count; v++)
		{
			s = tiles[v].terrain.ToString();
			file.WriteLine(s);			
		}	
		file.Close();
	}
	
	public void CreateFactions(int nF)
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
		Debug.Log("creating factions");
		numberOfFactions = nF+1;
		
		Faction faction = CreateTheLurkingNec();
		f.Add(faction);
		f[0].fname = "NEC";
		for (int c=1; c<numberOfFactions; c++)
		{
			faction = gameObject.AddComponent<Faction>();
			f.Add(faction);
			if (c==1)
			{
				faction.fcol = Color.red;
				f[c].fname = "Red faction ";
			}
			else
			{
				faction.fcol = Color.blue;	
				f[c].fname = "Blue faction ";
			}			
			int startingLoc  = 0;
			bool needToKeepLooking = true;
			while (needToKeepLooking)
			{
				startingLoc = Random.Range(0, tiles.Count);
				if (tiles[startingLoc].terrain == Terrains.GRASS & tiles[startingLoc].tileType == TileType.HEX)
				{
					needToKeepLooking = false;
				}				
			}
			tiles[startingLoc].AttachChit(f[c], ChitTypes.CITY, "faction " + c.ToString() + " city");
		}	
		
		// make sure Factions recognise that they have settlements
		for (int c=0; c<numberOfFactions; c++)
		{
			f[c].UpdateListOfCharacters();	
		}		
	}
	
	private Faction CreateTheLurkingNec()
	{
		Faction necFaction = gameObject.AddComponent<Faction>();
		necFaction.fname = "The Lurking Nex";
		bool lookingForNecsStartingLocation = true;
		int necsStartingLoc = 0;
		while (lookingForNecsStartingLocation)
		{
			necsStartingLoc = Random.Range(0, tiles.Count);
			if (tiles[necsStartingLoc].terrain == Terrains.SAND | tiles[necsStartingLoc].terrain == Terrains.ICE)
			{
				lookingForNecsStartingLocation = false;
			}				
		}
		tiles[necsStartingLoc].AttachChit(necFaction, ChitTypes.NECTOWER, "EvilNec");	
		return necFaction;
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
	public void SaveFactions()
	{
		// save (all) faction and turn info
		System.IO.StreamWriter file = new System.IO.StreamWriter("PerontorFactions.txt");
		string s;		
		// save turn info
		Debug.Log("Turn = " + turn.currentFctr.ToString());
		s = turn.currentFctr.ToString();
		file.WriteLine(s);		
		
		// now write faction info
		s = numberOfFactions.ToString();
		file.WriteLine(s);
		for (int t=0; t<numberOfFactions; t++)
		{
			s = f[t].fname; // write faction name
			file.WriteLine(s);
			s = f[t].characters.Count.ToString(); // write number of characters
			file.WriteLine(s);
			s = f[t].turn.ToString(); // write turn 
			file.WriteLine(s);
			s = f[t].cycle.ToString(); // write turn 
			file.WriteLine(s);			
			
			for (int c=0; c<f[t].characters.Count; c++)
			{
				Character currentCharacter = f[t].characters[c];
				s = currentCharacter.cType;
				file.WriteLine(s);
				s = currentCharacter.characterName;
				file.WriteLine(s);
				s = currentCharacter.GetComponent<Chit>().tile.id.ToString();
				file.WriteLine(s);
				// if character was a city, save city details
				if (currentCharacter.cType == "City")
				{
					Debug.Log("saving city details");
					City city = currentCharacter.transform.GetComponent<City>();
					if (city.currentyBuilding == null)
					{
						s = "null";
					}
					else
					{
						s = city.currentyBuilding;
					}
					file.WriteLine(s);
					s = city.turnsLeftToBuild.ToString();
					file.WriteLine(s);
				}
			}
		}
		file.Close();
	}
	
	public void LoadPlanet()
	{
		// Read the file as one string.
		System.IO.StreamReader myFile =	new System.IO.StreamReader("Perontor.txt");
		string myString = myFile.ReadLine();
		int nT = int.Parse(myString);
		// init new vector for vertices
		Vector3[] newT = new Vector3[nT];
		
		for (int v=0; v<nT; v++)
		{
			myString = myFile.ReadLine();
			newT[v].x = float.Parse(myString);
			myString = myFile.ReadLine();
			newT[v].y = float.Parse(myString);
			myString = myFile.ReadLine();
			newT[v].z = float.Parse(myString);
		}
	
		vertices = newT;
		mesh.vertices = vertices;
		// read in triangles... do I need to ? ***************************
		triangles = mesh.triangles;
		
		// now read in factions
		LoadFactions();
	}
	
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


