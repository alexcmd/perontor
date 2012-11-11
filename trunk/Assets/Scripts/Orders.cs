using UnityEngine;
using System.Collections;

public class Orders : MonoBehaviour {

	// Use this for initialization
	Planet planet;
	System.IO.StreamWriter file;
	
	void Start () {
		planet = this.GetComponent<Planet>();
		string folder = planet.savegameFolder;
		// create filename
		string filename = folder + "Perontor" + planet.playerFactionID.ToString() + "_" + planet.turnCtr.ToString() + ".orders";
		// create file
		file = new System.IO.StreamWriter(filename);
		// write faction ID number
		file.WriteLine(planet.playerFactionID.ToString());
		//write faction name
		file.WriteLine(planet.playerFaction.fname);
		file.WriteLine(planet.currentChar.GetComponent<Chit>().tile.id.ToString());
		
	}
	
	public void CommitTurn()
	{
		file.WriteLine("End");
		file.Close();
		Application.Quit();
	}
	
	public void EnlistOrder(City city)
	{
		file.WriteLine("Enlist");
		int tileID = city.GetComponent<Chit>().tile.id;
		file.WriteLine(tileID.ToString());
	}
}
