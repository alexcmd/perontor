using UnityEngine;
using System.Collections;

public class UI : MonoBehaviour {

	Turn turn;
	Planet planet;
	Faction cFaction;
	
	bool displayCityBuildWindow = false;
	public bool displayNewUnitBuiltWindow = false;
	public bool displayNoOrdersLeftWindow = false;
	
	GUIStyle panel = new GUIStyle();
	Texture2D t;
	
	// Use this for initialization
	void Start () 
	{
		turn = transform.GetComponent<Turn>();
		planet = transform.GetComponent<Planet>();
		planet.ui = this;
		cFaction = planet.f[turn.currentFctr];
		
		// used to colour the UI panels
		t = new Texture2D(1,1);
	}
	
	public void UpdatePanelColour(Color col)
	{
		// update the color of the background panel depending on who's shot it is.
		col.a = 0.5f; // change transparency
		t.SetPixel(0,0,col);
		t.Apply();
		panel.normal.background = t;
	}				
	
	void OnGUI()
	{	
		cFaction = planet.f[turn.currentFctr];
		
		GUILayout.BeginArea(new Rect(5, 5, 255, 500), panel);
		
		if (GUILayout.Button("Save World"))
		{
			planet.SaveWorld();
	//		planet.SaveFactions();
		}
		
		GUILayout.Label("Faction: " + cFaction.fname);
		GUILayout.Label("Cycle: " + cFaction.cycle.ToString());
		GUILayout.Label("Turn: " + (cFaction.turn+1).ToString() + "/" + (cFaction.characters.Count).ToString());
		if (GUILayout.Button("End Turn"))
		{
			turn.UpdateTurn();    	
		}
		if (cFaction.characters[cFaction.turn].tag == "City")
		{
			DisplayCityInfo();
		}
		else
		{
			GUILayout.Label("Active Character: " + cFaction.characters[cFaction.turn].characterName.ToString());	
			GUILayout.Label("Commands left: " + cFaction.characters[cFaction.turn].remainingCR.ToString());
		
		}
		DisplayTileInfo();
		// Display Selected Chit Info
		DisplaySelectedChitInfo();

		GUILayout.EndArea();
		
		// now Display character activation order somewhere else on screen
		DisplayCharActOrder();
		
		// pop-up windows
		if (displayCityBuildWindow == true)		{
			GUI.Window(1, new Rect(300, 200, 250, 200), DisplayCityBuildWindow, "Build Something");
		}
		if (displayNewUnitBuiltWindow == true)		{
			GUI.Window(1, new Rect(300, 200, 200, 50), DisplayNewUnitBuiltWindow, "Please Place New Unit");
		}
		if (displayNoOrdersLeftWindow == true)		{
			GUI.Window(1, new Rect(300, 200, 200, 70), DisplayNoOrdersLeftWindow, "Can't move");
		}
	}	
	
	void DisplayCityInfo()
	{
		City city = cFaction.characters[cFaction.turn].GetComponent<City>();
		GUILayout.Label("Commands left: " + cFaction.characters[cFaction.turn].remainingCR.ToString());
		
		GUILayout.Label("Active City: " + cFaction.characters[cFaction.turn].characterName.ToString());
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
			displayCityBuildWindow = true;
		}			    	
	}
	
	void DisplayCharActOrder()
	{
		GUILayout.BeginArea(new Rect(Screen.width-250, 5, 250, 150), panel);
		GUILayout.Label("Character Activation Order:");
		for (int c=0; c<cFaction.characters.Count; c++)
		{
			GUILayout.Label(cFaction.characters[c].cType + " - " + cFaction.characters[c].characterName);	
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
	
	void DisplayCityBuildWindow(int windowID)
	{
		GUILayout.BeginArea(new Rect(50, 20, 150, 200));
		GUILayout.Label("What would you like to build?");
		if(GUILayout.Button("a normal unit"))
		{
			cFaction.characters[cFaction.turn].GetComponent<City>().StartBuilding(ChitTypes.SWORD);
			displayCityBuildWindow = false;
		}
		if(GUILayout.Button("a commander"))
		{
			cFaction.characters[cFaction.turn].GetComponent<City>().StartBuilding(ChitTypes.COMMANDER);
			displayCityBuildWindow = false;
		}
		if(GUILayout.Button("nothing"))
		{
			cFaction.characters[cFaction.turn].GetComponent<City>().StartBuilding(ChitTypes.NULL);
			displayCityBuildWindow = false;
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
}
