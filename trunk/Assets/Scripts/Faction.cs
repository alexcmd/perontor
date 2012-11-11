using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Faction {
	
	public string fname = "Dinosaurs";
	public int 		id = 0;
	public int cycle = 0;
	public int step = -1;
	public Color fcol;
	
	public List<Character> characters = new List<Character>();
	
	
	public void UpdateListOfCharacters()		
	{
		List<Character> newList = new List<Character>();	
		// get all commanders belonging to faction
		GameObject[] allCommanders = GameObject.FindGameObjectsWithTag("Commander");
		
		foreach (GameObject g in allCommanders)
		{
			if (g.GetComponent<Chit>().faction == this)
			{
				newList.Add(g.GetComponent<Character>());
				Debug.Log(g.GetComponent<Character>().name);
			}
		}
		// get all cities belonging to faction
		GameObject[] allCities = GameObject.FindGameObjectsWithTag("City");
		foreach (GameObject g in allCities)
		{
			if (g.GetComponent<Chit>().faction == this)
			{
				newList.Add(g.GetComponent<Character>());	
			}
		}
		characters.Clear();
		characters = newList;
		characters = ShuffleCharacters(characters);		
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
	
	private List<Character>  ShuffleCharacters(List<Character> cList)
	{
		Character[] cArray = cList.ToArray();
		// Knuth shuffle algorithm :: courtesy of Wikipedia :)
        for (int t = 0; t < cArray.Length; t++ )
        {
            Character tmp = cArray[t];
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
