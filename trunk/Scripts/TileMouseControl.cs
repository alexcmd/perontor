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
	
	}	
}
