using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

static public class SaveLoad {

	
	static public void SaveWorld(Planet planet, List<Faction> f)
	{
		string docsDir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
		string totalDir = docsDir + "\\My Games\\Perontor\\";
		
		Debug.Log(totalDir);
		
		if (!Directory.Exists(totalDir))
		{
			Directory.CreateDirectory(totalDir);
		}
		
		// save world info - once for each player!
		List<Tile> tiles = planet.tiles;
		int numFactions = f.Count;
		for (int p = 0; p < numFactions; p++)
		{		
			//string folder = planet.GetComponentInChildren<MainMenu>().saveGamePath;
			string filename = totalDir + "Perontor_faction" + p.ToString() + "_turn" + planet.turnCtr.ToString() + ".world";
			Debug.Log (filename);
			System.IO.StreamWriter file = new System.IO.StreamWriter(filename);
			
			string s;
			s =  planet.planetSize.ToString();
			Debug.Log (s);
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
			
			SaveFactions(file, f);
			
			file.Close();
		}
	}
	
	static	void SaveFactions(System.IO.StreamWriter file, List<Faction> f)
	{
		int numFactions = f.Count;
		// save (all) faction and turn info
		//System.IO.StreamWriter file = new System.IO.StreamWriter("PerontorFactions.txt");
		string s;		
		// save turn info - new game so Turn = 0
		file.WriteLine("Turn");	
		s = "1";
		file.WriteLine(s);	
		
		// now write faction info
		file.WriteLine("num factions");	
		s = numFactions.ToString();
		file.WriteLine(s);
		for (int t=0; t<numFactions; t++)
		{
			s = f[t].fname; // write faction name
			file.WriteLine(s);
			s = f[t].fcol.ToString();
			file.WriteLine(s);
			file.WriteLine("num chars");	
			s = f[t].characters.Count.ToString(); // write number of characters
			file.WriteLine(s);	
			
			// new game so step = 0 and cycle = 1
			file.WriteLine("current step");	
			s = "0"; // write step 
			file.WriteLine(s);
			file.WriteLine("current cycle");	
			s = "1"; // write cycle
			file.WriteLine(s);			
			file.WriteLine("CHAR Details below");	
			for (int c=0; c<f[t].characters.Count; c++)
			{
				Character currentCharacter = f[t].characters[c];
				s = currentCharacter.cType.ToString();
				file.WriteLine(s);
				s = currentCharacter.name;
				file.WriteLine(s);
				s = currentCharacter.GetComponent<Chit>().tile.id.ToString();
				file.WriteLine(s);
				// if character was a city, save city details
			/*	if (currentCharacter.cType == ChitTypes.CITY)
				{
//					Debug.Log("saving city details");
					City city = currentCharacter.transform.GetComponent<City>();
					if (city.currentyBuilding == null)
					{
						s = "null";
					}
					else
					{
						s = city.currentyBuilding.ToString();
					}
					file.WriteLine(s);
					s = city.turnsLeftToBuild.ToString();
					file.WriteLine(s);
				}*/
			}	
		}
		file.WriteLine("ChitInfo");
		GameObject[] chits = GameObject.FindGameObjectsWithTag("Chit");
		foreach (GameObject g in chits)
		{
			s = g.GetComponent<Chit>().tile.id.ToString();
			file.WriteLine(s);
			s = g.GetComponent<Chit>().faction.id.ToString();
			file.WriteLine(s);
		}
	}

	static public float[] LoadPlanet1(Planet planet, StreamReader planetFile)
	{
		float[] alts = new float[planet.geometry.vertices.Length];	
		Terrains[] terrains = new Terrains[planet.geometry.nmbrHexs];
		// get alts
		for (int v=0; v<planet.geometry.nmbrHexs; v++)
		{
			string myString = planetFile.ReadLine();
			alts[v] = float.Parse(myString);
		}
		//get terrain types
		for (int v=0; v<planet.geometry.nmbrHexs; v++)
		{
			string terrainString = planetFile.ReadLine();
			terrains[v] = TerrainTypeParse(terrainString);	
		}
		string s = planetFile.ReadLine();	
		s = planetFile.ReadLine();
		planet.turnCtr = int.Parse(s);
		s = planetFile.ReadLine();	
		s = planetFile.ReadLine();	
		planet.numberOfFactions = int.Parse(s);	
		return alts;
	}
	
	static public void LoadPlanet2(Planet planet, StreamReader planetFile) 
	{				
		foreach (GameObject g in (GameObject.FindGameObjectsWithTag("Tile")))
		{
			planet.tiles.Add(g.GetComponent<Tile>());	
			g.GetComponent<Tile>().LinkPlanet(planet);
		}		
		
		// load faction info!
		
		for (int fctr=0; fctr<planet.numberOfFactions; fctr++)
		{
			Faction faction = new Faction();

			string s = planetFile.ReadLine();
			faction.fname = s;
			s = planetFile.ReadLine();
			faction.fcol = ParseColor(s);
			
			// Read in number of characters
			
			s = planetFile.ReadLine();
			s = planetFile.ReadLine();			
			int numChars = int.Parse(s);
			// read in Faction step 
			s = planetFile.ReadLine();
			s = planetFile.ReadLine();			
			faction.step = int.Parse(s);
	
			// read in faction cycle
			s = planetFile.ReadLine();
			s = planetFile.ReadLine();			
			faction.cycle = int.Parse(s);
			// read in faction char info
			s = planetFile.ReadLine();			
			for (int c=0; c<numChars; c++)
			{
				// get Chit Type
				s = planetFile.ReadLine();	
				ChitTypes cType = ChitTypeParse(s);
				// get Character name
				
				s = planetFile.ReadLine();	
				string charName = s;
				
				// get current location
				s = planetFile.ReadLine();	
				int tileID = int.Parse(s);
				Debug.Log (faction.fname);
				// spawn character!
				planet.tiles[tileID].AttachChit(faction, cType, charName);
				
				// now get city specific info, if relevant!
			/*	if (cType == ChitTypes.CITY)
				{
					City city = tiles[tileID].chitOnTile.GetComponent<City>();
					s = planetFile.ReadLine();
					city.currentyBuilding = ChitTypeParse(s);
					s = planetFile.ReadLine();
					city.turnsLeftToBuild = int.Parse(s);
				}	*/				
			}
			
			planet.factions.Add(faction);
			faction.UpdateListOfCharacters();
		}
		
		// now add in chits!
		string schit = planetFile.ReadLine();
//		Debug.Log("should say ChitInfo> " + schit);
		schit = planetFile.ReadLine();
		while (schit!=null)
		{
			
			int tID = int.Parse(schit);			
			int fID = int.Parse(planetFile.ReadLine());
			// get faction
			Faction faction = planet.factions[fID];
			
			planet.tiles[tID].AttachChit(faction, ChitTypes.SWORD, "new unit");
			schit = planetFile.ReadLine();
			
		}		
	}
	
	static Terrains TerrainTypeParse(string s)
	{
		Terrains t;
		switch (s)
		{
			case "GRASS":				
				t = Terrains.GRASS;
				break;
			case "FOREST":				
				t = Terrains.FOREST;
				break;
			case "ICE":				
				t = Terrains.ICE;
				break;
			case "ROCK":				
				t = Terrains.ROCK;
				break;
			case "SAND":				
				t = Terrains.SAND;
				break;
			case "WATER":				
				t = Terrains.WATER;
				break;
			default:
				t = Terrains.GRASS;
				Debug.Log("UNKNOWN TERRAIN TYPE!");
				break;
		}	
		return t;
	}	
	
	static ChitTypes ChitTypeParse(string s)
	{
		ChitTypes cType;
//		Debug.Log(s);
		switch (s)
		{			
			case "CITY":								
				cType = ChitTypes.CITY;
				break;		
			case "COMMANDER":								
				cType = ChitTypes.COMMANDER;
				break;
			case "SWORD":								
				cType = ChitTypes.SWORD;
				break;
			default:
			cType = ChitTypes.NULL;	
			Debug.Log("unknown CHIT type: " + s);
				break;
		}
		return cType;
	}
	
	static Color ParseColor(string s)
	{
	 	// converts the output of Color.ToString() back to a Color
		Color c = new Color();
		string[] nums = s.Split(',');
		for (int i=0; i<(nums.Length); i++)
		{
			float f;
			float.TryParse(Regex.Replace(nums[i], "[A-Z()]*", string.Empty), out f);
			c[i] = f;
		}
		return c;		
	}
}
