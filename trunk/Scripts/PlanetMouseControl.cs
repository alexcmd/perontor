using UnityEngine;
using System.Collections;

public class PlanetMouseControl : MonoBehaviour {
	
	
	/* 
	 * This allows us to interact with the planet using the mouse
	 */
	
	Planet planet;
	
	void Start () 
	{
		planet = this.GetComponent<Planet>();
	}
	
	void OnMouseDown()
	{
		/* ray cast from camera to mousepointer (in world)
		 * and if we click on a tile, call the ClickedOnTile()
		 * function for the tile that was clicked on! 
		 * This allows for moving counters, etc
		 */
		 
		Ray ray = Camera.mainCamera.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit = new RaycastHit();

		if (Physics.Raycast (ray, out hit, 100, 5)) 
		{
			// get tile index
			int t = hit.triangleIndex;
			planet.tiles[t].ClickedOnTile();
//			Debug.Log(hit.triangleIndex.ToString() + " - " + planet.tiles[t].withinMovementRange.ToString());
		}
	}
	
	void OnMouseOver()
	{
		Ray ray = Camera.mainCamera.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit = new RaycastHit();
		if (Physics.Raycast (ray, out hit, 100, 5)) 
		{
			// get tile index
			int t = hit.triangleIndex;		
//			Debug.Log(t.ToString());
			planet.mouseOverTile = planet.tiles[t];
//			planet.tiles[t].HighlightTile();
			planet.selectedChit = planet.tiles[t].chitOnTile;			
		}
		else
		{
	//		planet.UnhighlightTiles(1);	
		}
	}	
}
