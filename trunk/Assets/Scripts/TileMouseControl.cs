using UnityEngine;
using System.Collections;

public class TileMouseControl : MonoBehaviour {
	
	
	/* 
	 * This allows us to interact with the tile using the mouse
	 */
	

	
	void OnMouseDown()
	{
		this.GetComponent<Tile>().ClickedOnTile();		
	}
	
	void OnMouseOver()
	{
		if (this.tag == "Water")
		{
			transform.parent.parent.GetComponent<Planet>().mouseOverTile = transform.parent.GetComponent<Tile>();
			transform.parent.parent.GetComponent<Planet>().GetComponentInChildren<CamControl>().OutlineTile(transform.parent.GetComponent<Tile>());
		}
		else
		{
			transform.parent.GetComponent<Planet>().mouseOverTile = this.GetComponent<Tile>();
			transform.parent.GetComponent<Planet>().GetComponentInChildren<CamControl>().OutlineTile(this.GetComponent<Tile>());
		}
	}	
}
