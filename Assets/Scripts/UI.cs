using UnityEngine;
using System.Collections;

public class UI : MonoBehaviour {

	Turn turn;
	Planet planet;
	Faction faction;
	
	City city;
	
	bool displayCityInfoWindow = false;
	public bool displayNewUnitBuiltWindow 	= false;
	public bool displayNoOrdersLeftWindow 	= false;
	public bool gameLoadedSummaryWindow 	= false;
	
	GUIStyle panel = new GUIStyle();
	Texture2D t;
	
	// Use this for initialization
	void Start () 
	{
		turn = transform.GetComponent<Turn>();
		planet = transform.GetComponent<Planet>();
		planet.ui = this;
		faction = planet.playerFaction;
		
		// used to colour the UI panels
		t = new Texture2D(1,1);
		UpdatePanelColour(Color.gray);
	}
	
	public void UpdatePanelColour(Color col)
	{
		// update the color of the background panel depending on who's shot it is.
		col.a = 0.9f; // change transparency
		t.SetPixel(0,0,col);
		t.Apply();
		panel.normal.background = t;
	}				
	
	void OnGUI()
	{	
		faction = planet.playerFaction;	
		
		GUILayout.BeginArea(new Rect(5, 5, 255, 500), panel);
		
		GUILayout.Label("Faction: " + faction.fname);
		GUILayout.Label("Turn: " + planet.turnCtr.ToString());
		GUILayout.Label("Cycle: " + faction.cycle.ToString());
		GUILayout.Label("Step: " + (faction.step+1).ToString() + "/" + (faction.controllers.Count).ToString());
		
		if (GUILayout.Button("End Turn"))
		{
			Order[] orders = planet.playerFaction.GetOrders();
			System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(orders.GetType());
			// create file
			//file = new System.IO.TextWriter();
			//x.Serialize(file);
		}

		GUILayout.Label("Active controller: " + faction.controllers[faction.step].name.ToString());	
		GUILayout.Label("Commands left: " + faction.controllers[faction.step].remainingCR.ToString());

		DisplayTileInfo();
		// Display Selected Chit Info
		DisplaySelectedChitInfo();

		GUILayout.EndArea();
		
		// now Display controller activation order somewhere else on screen
		DisplayCharActOrder();
		
		// pop-up windows
		if (displayCityInfoWindow == true)		{
			GUI.Window(1, new Rect(300, 200, 250, 200), DisplayCityInfoWindow, city.name);
		}
		if (displayNewUnitBuiltWindow == true)		{
			GUI.Window(1, new Rect(300, 200, 200, 50), DisplayNewUnitBuiltWindow, "Please Place New Unit");
		}
		if (displayNoOrdersLeftWindow == true)		{
			GUI.Window(1, new Rect(300, 200, 200, 70), DisplayNoOrdersLeftWindow, "Can't move");
		}
		if (gameLoadedSummaryWindow == true)		{
			GUI.Window(1, new Rect(300, 200, 300, 300), DisplayLoadedSummaryWindow, "New turn!");
		}
	}	
	
	public void DisplayCityInfo(City c)
	{
		displayCityInfoWindow = true;
		city = c;
/*		GUILayout.BeginArea(new Rect(50, 20, 150, 200));
		GUILayout.Label("Commands left: " + faction.controllers[faction.step].remainingCR.ToString());
		
		GUILayout.Label("Active City: " + faction.controllers[faction.step].name.ToString());
		GUILayout.Label("Population: " + city.pop.ToString()); 
		string buttonString;
		if (city.currentyBuilding == ChitTypes.NULL)
		{
			buttonString = "Awaiting Build Order";		
		}
		else
		{
			buttonString = "Currently building " + city.currentyBuilding + " - " + city.turnsLeftToBuild.ToString() + " cycle(s) left";
		}
		if (GUILayout.Button(buttonString))
		{
			//displayCityInBuildWindow = true;
		}	
		GUILayout.EndArea();*/
	}
	
	void DisplayCharActOrder()
	{
		GUILayout.BeginArea(new Rect(Screen.width-250, 5, 250, 150), panel);
		GUILayout.Label("controller Activation Order:");
		for (int c=0; c<faction.controllers.Count; c++)
		{
			GUILayout.Label(faction.controllers[c].GetComponent<Chit>().ctype + " - " + faction.controllers[c].name);	
		}
		GUILayout.EndArea();		
	}
	
	void DisplayTileInfo()
	{
		if (planet.mouseOverTile != null)
		{
			GUILayout.Label("Tile ID: " + planet.mouseOverTile.id.ToString());
			GUILayout.Label("Altitude: " + planet.mouseOverTile.altitude.ToString());
			GUILayout.Label("Within move range: " + planet.mouseOverTile.withinMovementRange.ToString());
		}				
	}
	
	void DisplaySelectedChitInfo()
	{
		Chit selChit = planet.selectedChit;
		if (selChit != null)	
		{
			if (selChit.canMoveThisTurn == true)
			{
				GUILayout.Label("Selected chit can move");
			}
			else 
			{
				GUILayout.Label("Selected chit can not move this turn");
			}
		}		
	}
	
	/*
	 ****** pop-up windows *****
	 * */
	
	void DisplayCityInfoWindow(int windowID)
	{
		GUILayout.BeginArea(new Rect(50, 20, 150, 200));
		GUILayout.Label("Welcome to " + city.name);
		GUILayout.Label("Population " + city.pop.ToString());
		GUILayout.Label("Growth Rate " + city.growthrate.ToString());
		
		if(GUILayout.Button("enlist unit (currently " + city.enlistCount.ToString() + ")"))
		{
			city.Enlist();
			//displayCityInfoWindow = false;
		}
		
		if(GUILayout.Button("Close"))
		{
			displayCityInfoWindow  = false;
		}
		GUILayout.EndArea ();		
	}
	
	void DisplayNewUnitBuiltWindow(int windowID)
	{
		GUILayout.BeginArea(new Rect(25, 25, 150, 25));
		
		if(GUILayout.Button("please place new unit"))
		{
			displayNewUnitBuiltWindow = false;
		}

		GUILayout.EndArea ();
	}
	
	void DisplayNoOrdersLeftWindow(int windowID)
	{
		GUILayout.BeginArea(new Rect(25, 25, 150, 25));
		
		if(GUILayout.Button("No orders left for this commander"))
		{

			displayNoOrdersLeftWindow = false;
		}
		GUILayout.EndArea ();
	}
	
	void DisplayLoadedSummaryWindow(int windowID)
	{
		GUILayout.BeginArea(new Rect(25, 25, 250, 225));
		
		if (planet.turnCtr>1)
		{
			GUILayout.Label ("Welcome back!");
			GUILayout.Label("you are playing as player " + planet.playerFaction.ToString());
		}
		else
		{
			GUILayout.Label ("Welcome to the game");
			GUILayout.Label ("We have " + (planet.numberOfFactions-1).ToString() + " players.");
			GUILayout.Label ("please name your faction:");
			planet.playerFaction.fname 	= GUILayout.TextField(planet.playerFaction.fname);
			
			GUILayout.Label ("please name your starting city:");
			planet.currentChar.name 	= GUILayout.TextField(planet.currentChar.name);			
		}
		
		if(GUILayout.Button("Thanks"))
		{

			gameLoadedSummaryWindow = false;
		}
		
		GUILayout.EndArea ();
	}
	
}
