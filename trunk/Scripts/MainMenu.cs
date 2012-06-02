using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {
	
	/* this is the main menu GUI. 
	 * Stuff like NEW GAME, LOAD GAME, OPTIONS, QUIT
	 * etc. You know the score 
	 * */

	GameObject planet;
	int numOfFactions = 2;
	public int worldSize = 3;
	public bool loadingWord = false;
	
	void Start()
	{
		// find the planet
		planet = GameObject.Find("Planet");
	}
	
	void OnGUI()
	{		
		int x = (int)Screen.width/2-50;
		int y = (int)Screen.height/2-150;
		GUILayout.BeginArea(new Rect(x, y, 200, 200));		
	
		if (GUILayout.Button("Load World"))
		{			
			// Not, loading (and saving for that matter) is only partially implemented. 
//			planet.GetComponent<Planet>().LoadPlanet();	
			loadingWord = true;
			StartGame();
			//this.enabled = false;				 
		}
		if (GUILayout.Button("New World"))
		{							
			StartGame();
			
		}
		GUILayout.Label("Select New World Size: " + worldSize.ToString());
		worldSize = (int)(GUILayout.HorizontalSlider(worldSize, 2, 5));
		if (GUILayout.Button("Options"))
		{							
			/* we currently have no options. But worldgen stuff, number of factions,
			  * key bindings, etc will eventually go here 
			  */
		}
		if (GUILayout.Button("Exit Game"))
		{
			Application.Quit();
		}
		GUILayout.EndArea();

	}
	
	void StartGame()
	{
		/* returning a bool, as something this stuff seems to run on seperate threads
		 * I'm not sure if that is actually the case, but this fixes it. */			
		//bool r
		planet.AddComponent<CreateHexPlanet>();
		// switch on camera controls
		GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CamControl>().enabled = true;
		// add in game UI
		planet.AddComponent<UI>();
		// create the Turn class, which controls game flow
		planet.AddComponent<Turn>();

		// switch this (the Main Menu) off. 
		this.enabled = false;
	}
}
