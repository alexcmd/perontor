using UnityEngine;
using System.Collections;
using System.IO;

public class MainMenu : MonoBehaviour {
	
	/* this is the main menu GUI. 
	 * Stuff like NEW GAME, LOAD GAME, OPTIONS, QUIT
	 * etc. You know the score 
	 * */
	public string saveGamePath;// = "\\%USERPROFILE%\\Documents\\My Games\\Perontor\\";
	GameObject planet;
	public int numOfFactions = 2;
	public int worldSize = 2;
	string turnStr = "1";
	
	public bool isHost = false; // used to determine if we are processing turns, or playing a turn
	public int turn;
	public int factionID = 1;
	bool popup = false;
	
	public bool displayMissingOrdersWindow = false;
	bool displayWorldGenOptions = false;	
	bool displayWorldGenWindow = false;
	bool displayLoadWindow = false;
	
	void Awake()
	{
		saveGamePath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "\\My Games\\Perontor\\";
	}
	
	void Start()
	{
		// find the planet
		planet = GameObject.Find("Planet");
	}
	
	void OnGUI()
	{		
		int x = (int)Screen.width/2-50;
		int y = (int)Screen.height/2-150;
		GUILayout.BeginArea(new Rect(x, y, 400, 400));		
	
		saveGamePath  = GUILayout.TextField(saveGamePath);
			
		/*
		 * If there we are displaying a pop-up window then we 
		 * don't want to allow the user to click on main menu
		 * buttons!
		 */
		if (popup)
		{
			GUILayout.Box("Load World");
			GUILayout.Box("Process Turn Files");
			GUILayout.Box("New World");
			GUILayout.Box("Options");
			GUILayout.Box("Exit Game");	
		}
		else			
		{
			LoadButton();
			ProcessTurnButton();
			NewWorldButton();
			OptionsButton();
			ExitButton();
		}
		
		GUILayout.EndArea();
		
		/*
		 * Code for controlling pop-up windows
		 */
		if (displayMissingOrdersWindow == true)		{
			popup = true;
			GUI.Window(1, new Rect(300, 200, 250, 200), DisplayMissingOrdersWindow, "error");
		}
		if (displayWorldGenOptions == true)		{
			popup = true;
			GUI.Window(1, new Rect(300, 200, 250, 200), DisplayWorldGenOptions, "World Gen Options");
		}
		if (displayWorldGenWindow == true)		{
			popup = true;
			GUI.Window(1, new Rect(300, 200, 250, 200), DisplayWorldGenWindow, "World Generating");
		}
		if (displayLoadWindow == true)		{
			popup = true;
			GUI.Window(1, new Rect(300, 200, 350, 500), DisplayLoadWindow, "World Generating");
		}
	}
	
	/*
	 * Code for main menu buttons!
	 */
	void LoadButton()
	{
		if (GUILayout.Button("Load World"))
		{			
			displayLoadWindow = true;
		}
	}
	
	void ProcessTurnButton()
	{
		if (GUILayout.Button("Process Turn Files"))
		{							
			turn = 1;
			isHost = false;		
			planet.AddComponent<ProcessOrders>();
			
			GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().enabled = true;
			GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CamControl>().enabled = true;
		}	
	}
	
	void NewWorldButton()
	{
		if (GUILayout.Button("New World"))
		{							
			isHost = true;
			planet.AddComponent<CreateNewGame>();	
			displayWorldGenWindow = true;
		}
	}
	
	void OptionsButton()
	{
		if (GUILayout.Button("Options"))
		{							
			displayWorldGenOptions = true;
		}
	}
	
	void ExitButton()
	{
		if (GUILayout.Button("Exit Game"))
		{
			Application.Quit();
		}
	}
	
	/*
	 * Code for pop up windows goes down here!
	 */
	void DisplayWorldGenOptions(int windowID)
	{
		GUILayout.BeginArea(new Rect(25, 50, 200, 100));
		GUILayout.Label("Select New World Size: " + worldSize.ToString());
		worldSize 		= (int)(GUILayout.HorizontalSlider(worldSize, 2, 6));
		GUILayout.Label("Select Number of Factions: " + numOfFactions.ToString());
		numOfFactions 	= (int)(GUILayout.HorizontalSlider(numOfFactions, 2, 6));
		if (GUILayout.Button("Done"))
		{
			displayWorldGenOptions = false;
			popup = false;
		}
		GUILayout.EndArea();
	}
	
	void DisplayWorldGenWindow(int windowID)
	{
		GUILayout.BeginArea(new Rect(50, 50, 200, 100));
		GUILayout.Label ("Generat(ing)/(ed) world... click done to quit");
		if (GUILayout.Button("Click to Exit"))
		{
			Application.Quit();
		}
		GUILayout.EndArea();
	}
	
	void DisplayLoadWindow(int windowID)
	{
		GUILayout.BeginArea(new Rect(25, 50, 300, 500));
		GUILayout.Label ("Let us try and load a world :-)");
		
		GUILayout.BeginHorizontal();
		GUILayout.Box ("Enter turn number:");
		turnStr = GUILayout.TextField(turnStr);
		GUILayout.EndHorizontal();
		
		int.TryParse(turnStr, out turn);
		
		GUILayout.Label("Select the faction you would like to play");
		// now count up buttons for corresponding turn!
		DirectoryInfo dir = new DirectoryInfo(saveGamePath);
		FileInfo[] info = dir.GetFiles("*turn" + turn.ToString() + ".world");
		
		for (int ctr=1; ctr<info.Length; ctr++)
		{
			if (GUILayout.Button("Load faction " + ctr.ToString() + ", turn " + turn.ToString()))
			{
				isHost = false;		
				factionID = ctr;
				planet.AddComponent<LoadGame>();
				// switch this (the Main Menu) off. 
				this.enabled = false;
			}
		}
		GUILayout.Label("");
		
		if (GUILayout.Button("Cancel"))
		{
			displayLoadWindow = false;
			popup = false;
		}
		GUILayout.EndArea();
	}
	
	
	void DisplayMissingOrdersWindow(int windowID)
	{
		GUILayout.BeginArea(new Rect(50, 50, 200, 100));
		if (GUILayout.Button("missing order files"))
		{
			displayMissingOrdersWindow = false;
			Application.Quit();
		}
		GUILayout.EndArea();
	}
}
