using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Faction {
	
	public string fname = "Dinosaurs";
	public int 		id = 0;
	public int cycle = 0;
	public int step = -1;
	public Color fcol;
	
	public List<Controller> controllers = new List<Controller>();
	
	
	public void UpdateListOfControllers()		
	{
		List<Controller> newList = new List<Controller>();	
		// get all commanders belonging to faction
		GameObject[] allCommanders = GameObject.FindGameObjectsWithTag("Commander");
		
		foreach (GameObject g in allCommanders)
		{
			if (g.GetComponent<Chit>().faction == this)
			{
				newList.Add(g.GetComponent<Controller>());
				Debug.Log(g.GetComponent<Controller>().name);
			}
		}
		// get all cities belonging to faction
		GameObject[] allCities = GameObject.FindGameObjectsWithTag("City");
		foreach (GameObject g in allCities)
		{
			if (g.GetComponent<Chit>().faction == this)
			{
				newList.Add(g.GetComponent<Controller>());	
			}
		}
		controllers.Clear();
		controllers = newList;
		controllers = ShuffleControllers(controllers);		
	}
	
	/*public void UpdateCities()
	{
		GameObject[] allCities = GameObject.FindGameObjectsWithTag("City");
		foreach (GameObject g in allCities)
		{
			if (g.GetComponent<Chit>().faction == this)
			{
				g.GetComponent<City>().UpdateBuilding();
			}
		}
	}*/
	
	private List<Controller>  ShuffleControllers(List<Controller> cList)
	{
		Controller[] cArray = cList.ToArray();
		// Knuth shuffle algorithm :: courtesy of Wikipedia :)
        for (int t = 0; t < cArray.Length; t++ )
        {
            Controller tmp = cArray[t];
            int r = Random.Range(t, cArray.Length);
            cArray[t] = cArray[r];
            cArray[r] = tmp;
        }
		// now turn Array into list
		cList.Clear();
		for (int t = 0; t < cArray.Length; t++ )
        {
			cList.Add(cArray[t]);
		}
		return cList;
	 }
	

}
