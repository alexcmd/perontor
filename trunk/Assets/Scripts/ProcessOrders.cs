using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
//using System;

public class ProcessOrders : MonoBehaviour {
	
	int numSubDivides;
	int numFactions;
	
	public List<Faction> f = new List<Faction>(); // a list of all factions in game
	public List<Tile> tiles = new List<Tile>();  // an array of all the tiles on the planet
		
	int[] triangles; // list of vertex IDs for triangles
	Vector3[] vertices;
	
	List<Vector2> edges = new  List<Vector2>();
	float edgeLength;

	int nmbrTris;
	int nmbrVertices;
	int nmbrHexs;
	
	List<List<int>> trisForEdges = new List<List<int>>();
	
	float edgeAngle;
	Planet planet;
	
	float articCircle = 1.4f;
	float seaLevel = 1.86f;
	float equatorDesert = 0.4f;
	
	System.IO.StreamReader planetFile;
	string planetFileName;
	
	
	int turnCtr = 1;
	// Use this for initialization
	void Start () {
		 /* 
		  * really, i need to improve this so player choses file, 
		  * and then factionID is loaded from file
		  */
		
		planet = gameObject.AddComponent<Planet>();	
		
		string folder = this.GetComponentInChildren<MainMenu>().saveGamePath;
		planetFileName = folder + "Perontor_faction0_turn" + turnCtr.ToString() + ".world";
		planetFile =	new System.IO.StreamReader(planetFileName);
		string myString = planetFile.ReadLine();
		numSubDivides = int.Parse(myString);
		
		planet.geometry.Sqrt3Subdivsion();
		/* 
		 * the first nmbrHexs values in vertices give the midpoints for the hex tiles.
		 * collect these and apply terraforming to them to get altitudes
		 */		
		float[] alts = SaveLoad.LoadPlanet1(planet, planetFile);
		
		// sort out ocean	
		GameObject ocean = (GameObject)GameObject.Find("Ocean");
		ocean.transform.localScale = new  Vector3(2*seaLevel, 2*seaLevel, 2*seaLevel);
		
		
		planet.geometry.PlaceTiles(alts);	
		
		SaveLoad.LoadPlanet2(planet, planetFile);
		planet.turnCtr = planet.turnCtr + 1	;
		planetFile.Close();
		
		// check all order files are present!
		DirectoryInfo dir = new DirectoryInfo(folder);
		
		FileInfo[] info = dir.GetFiles("*.orders");
		Debug.Log (info.Length.ToString() + " order files found");
		if (info.Length < numFactions-1)
		{
			// missing some order files!
			Debug.Log(info.Length.ToString() + " out of " + numFactions.ToString());
			gameObject.GetComponentInChildren<MainMenu>().displayMissingOrdersWindow = true;
			Destroy(planet);
			Destroy(this);				
		}	
		else
		{
			// we have all the files!
			for (int c=1; c<numFactions; c++)
			{	
				string ordersFileName = info[c-1].FullName;
				System.IO.StreamReader ordersFile =	new System.IO.StreamReader(ordersFileName);				
				ParseOrdersFile(ordersFile, c);
			}
		}
		SaveLoad.SaveWorld(planet, f);
		Application.Quit();
	}
	
	private void ParseOrdersFile(System.IO.StreamReader file, int factionID)
	{
		/*
		 * keep reading orders in the file until we hit "End"
		 * 
		 * Currently available orders are:
		 * -enlist
		 * -end
		 */
		string s;
		s = file.ReadLine();
		if (int.Parse(s) == factionID)
		{
			// faction IDs match - do nothing
		}
		else
		{
			Debug.Log("bug with faction ID");
		}
		
		// get (potentially new) faction name
		f[factionID].fname = file.ReadLine();
		// read in active city/commander... we don't really care about this just now
		s = file.ReadLine();
		bool keepreading = true;
		while (keepreading)
		{
			s = file.ReadLine();
			switch (s)
			{
			case "End":
				keepreading = false;
				break;
			case "Enlist":
				int tileID = int.Parse(file.ReadLine());
				ProcessEnlistOrder(factionID, tileID);
				break;
			default:
				keepreading = false;
				break;
			}	
		}
	}
	
	void ProcessEnlistOrder(int factionID, int tileID)
	{
		/* 
		 * player has enlisted a new bog standard unit at their city
		 * so lets spawn one there
		 */
		tiles[tileID].AttachChit(f[factionID], ChitTypes.SWORD, "name");		
	}
	
	
		

	
	

}
