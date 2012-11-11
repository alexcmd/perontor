using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile : MonoBehaviour {

	/*
	 * This class does a lot of grunt work
	 * Will document more when things are a little more settled.
	 * 
	 * Important methods:
	 * AttachChit()
	 * MoveChit ()
	 */
	
	public int id;
	public float altitude;
	public TileType tileType;

	public Vector3 midpoint;
	
	public Terrains terrain;	
	
	public List<int> nbrTiles = new List<int>();

	Planet planet;
	UI ui;
	public Chit chitOnTile;
	
	public bool tileActive = false;
	public bool withinMovementRange = false;
	
	float chitScalingFactor = 0.6f;
	
	GameObject activeRegionHoop;
	
//	AudioClip click;
	
	void Start()
	{
		//	click =  (AudioClip)Resources.Load("sfx_unit.wav");

	}
	
	public void SetTileType(TileType tt)
	{
		tileType = tt;
		if (tileType == TileType.PENT)
		{
			// attach a shrine!
			// first randomly decide which of the 4 shrines we want!
			int r = Random.Range(1,4);		
			Debug.Log ("Shrine" + r.ToString());
			Vector3 m =   GameObject.FindGameObjectWithTag("Planet").transform.localToWorldMatrix * midpoint;
			GameObject gameObject = (GameObject)GameObject.Instantiate(Resources.Load("Shrine" + r.ToString()), m, Quaternion.LookRotation(midpoint));	
			gameObject.name = "Shrine";
			gameObject.transform.localScale = new Vector3(chitScalingFactor,chitScalingFactor,chitScalingFactor);
			gameObject.transform.parent = GameObject.FindGameObjectWithTag("Planet").transform;
			chitOnTile = gameObject.GetComponent<Chit>();
		}		
	}
	
	public void LinkPlanet(Planet p)
	{
		planet = p;		
	}
	
	public float Elevation()
	{	
		float hyp = Mathf.Abs(Mathf.Sqrt(midpoint.x * midpoint.x + midpoint.z * midpoint.z));
		float elevation = Mathf.Atan(midpoint.y/hyp);	
		return elevation * 180/Mathf.PI;
	}	
	
	public float Azimuth()
	{	
		float azimuth = Mathf.Atan2(midpoint.x, midpoint.z) + Mathf.PI;
		return azimuth * 180/Mathf.PI;
	}	
	
	public void MoveChit(Chit chit)
	{
//		Debug.Log("trying to move chit");
		Vector3 m = planet.transform.localToWorldMatrix * midpoint;
		// detach chit from current tile
		chit.tile.chitOnTile = null;
		// move chit to new tile
		chit.transform.position =  m;
		chit.transform.rotation =  Quaternion.LookRotation(midpoint);	
		//chit.transform.RotateAround(Vector3.left, 90);
		// add to new tile
		chitOnTile = chit;
		chitOnTile.tile = this;
		// can't move again!
		chitOnTile.canMoveThisTurn = false;	
		// unhighlight tiles
		planet.ResetTileMovementRangeFlag();
		planet.UnHighLightTiles();
		// reduce current char's remaning CR
		chit.faction.characters[chit.faction.step].remainingCR--;
	}
	
	public void ReapplyTerrainTexture()
	{
		Material m;
		switch (terrain)
		{
			case Terrains.GRASS:
				m = this.GetComponent<MeshRenderer>().material = (Material)Resources.Load("TerrainMaterials/Tile_Grass", typeof(Material));
				break;
			case Terrains.ICE:
				m = this.GetComponent<MeshRenderer>().material = (Material)Resources.Load("TerrainMaterials/Tile_Ice", typeof(Material));
				break;
			case Terrains.SAND:
				m = this.GetComponent<MeshRenderer>().material = (Material)Resources.Load("TerrainMaterials/Tile_Sand", typeof(Material));
				break;
			case Terrains.WATER:
				m = this.GetComponent<MeshRenderer>().material = (Material)Resources.Load("TerrainMaterials/Tile_Water", typeof(Material));
				break;
			default:
				m = this.GetComponent<MeshRenderer>().material = (Material)Resources.Load("TerrainMaterials/Tile_Grass", typeof(Material));
				break;
		}
		this.GetComponent<MeshRenderer>().material = m;		
	}
	
	public void ClickedOnTile()
	{
		//		 audio.PlayOneShot(click);
		//Debug.Log("click");
		//Debug.Log("Clicked on a tile. Tile is active: " + tileActive.ToString());	
		/*		
		if there is a chit on this tile,
		and it is within range...
		*/		
		if (chitOnTile != null)
		{
			switch(chitOnTile.ctype)
			{
			case ChitTypes.CITY:
				City city = chitOnTile.GetComponent<City>();
				transform.parent.GetComponent<Planet>().ui.DisplayCityInfo(city);
				break;
			case ChitTypes.SWORD:
				ClickedOnATileWithAChit();
				break;
			default:
				break;
			}
		}
		/* 
		if there isn't a chit on this tile, but we have previously selected a chit
		then move that chit to this tile
		*/
		else 
		{
//			Debug.Log("no chit on tile");
			ClickedOnAnEmptyTile();
		}
	}
	
	private void ClickedOnAnEmptyTile()
	{
		Debug.Log ("clicked on an empty tile");
		planet = GameObject.FindGameObjectWithTag("Planet").GetComponent<Planet>();
		
		if (planet.orderingChit == null)
		{
			// we are not currently doing anything, so nothing to do?
			//HighLightTile();
		}
		else
		{
			/*
			* We are currently ordering a chit, 
			* so lets try and move it to this tile
			*/
			if (withinMovementRange == true)
			{
			//	Debug.Log("can move here!");				
			//	MoveChit(planet.orderingChit);
				planet.orderingChit.MovementArrowDone();
				planet.orders.MovementOrder(planet.orderingChit.tile, this);
				planet.orderingChit = null;
				
			}
			else
			{
//				Debug.Log("too far away :(");		
			}			
		}
	}
	
	public void TileInMovementRange()
	{
		if (planet.orderingChit.canEnterTerrains.Contains(terrain))
		{
			withinMovementRange = true;
			Material m;
			switch (terrain)
			{
				case Terrains.GRASS:
					m = this.GetComponent<MeshRenderer>().material = (Material)Resources.Load("TerrainMaterials/Tile_GrassA", typeof(Material));
					break;
				case Terrains.ICE:
					m = this.GetComponent<MeshRenderer>().material = (Material)Resources.Load("TerrainMaterials/Tile_IceA", typeof(Material));
					break;
				case Terrains.SAND:
					m = this.GetComponent<MeshRenderer>().material = (Material)Resources.Load("TerrainMaterials/Tile_SandA", typeof(Material));
					break;
				case Terrains.WATER:
					m = this.GetComponent<MeshRenderer>().material = (Material)Resources.Load("TerrainMaterials/Tile_WaterA", typeof(Material));
				break;
				default:
					m = this.GetComponent<MeshRenderer>().material = (Material)Resources.Load("TerrainMaterials/Tile_GrassA", typeof(Material));
					break;
			}
		}
	}
	
	public void ActivateTile()
	{
		tileActive = true;
		if (chitOnTile!=null)
		{
			chitOnTile.canMoveThisTurn = true;
		}
	}
	
	public void UnActivateTile()
	{
		tileActive = false;
	}	
	
	void HighLightTile()
	{
		planet.NewSelectedTiles(nbrTiles);
		Material m;
		switch (terrain)
		{
			case Terrains.GRASS:
				m = this.GetComponent<MeshRenderer>().material = (Material)Resources.Load("TerrainMaterials/Tile_GrassA", typeof(Material));
				break;
			case Terrains.ICE:
				m = this.GetComponent<MeshRenderer>().material = (Material)Resources.Load("TerrainMaterials/Tile_IceA", typeof(Material));
				break;
			case Terrains.SAND:
				m = this.GetComponent<MeshRenderer>().material = (Material)Resources.Load("TerrainMaterials/Tile_SandA", typeof(Material));
				break;
			case Terrains.WATER:
				m = this.GetComponent<MeshRenderer>().material = (Material)Resources.Load("TerrainMaterials/Tile_WaterA", typeof(Material));
				break;
			default:
				m = this.GetComponent<MeshRenderer>().material = (Material)Resources.Load("TerrainMaterials/Tile_GrassA", typeof(Material));
				break;
		}
	}
	
	public void ClickedOnATileWithAChit()
	{
		Debug.Log ("click");
		planet = GameObject.FindGameObjectWithTag("Planet").GetComponent<Planet>();	
		HighLightTile();
		// deselect this chit if we click on the currently selected chit.
		if (chitOnTile == planet.orderingChit)
		{
			planet.orderingChit = null;
			planet.UnHighLightTiles();
			planet.ResetTileMovementRangeFlag();			
		}
		else if (chitOnTile.canMoveThisTurn)
		{
			Faction currFaction = planet.playerFaction;
			if (currFaction.characters[currFaction.step].remainingCR>0)
			{
				// switch in line renderer/movement arrow
				chitOnTile.MovementArrowOn();
				
				// highlight tiles within range!
				//List<int> selTri = SelectTilesForMoving();			
				//planet.HighlightTiles(selTri.ToArray(), Terrains.move);
				
				planet.orderingChit = chitOnTile;
				List<int> tilesWithinRange = planet.GetTilesWithinRange(id, chitOnTile.movementRange, chitOnTile.canEnterTerrains);
				planet.selTiles = tilesWithinRange;
		
				foreach (int t in tilesWithinRange)
				{
					planet.tiles[t].TileInMovementRange();
				}
				
			}
			else
			{
				ui.displayNoOrdersLeftWindow = true;
			}
		}	
	}
		
	public void AttachChit(Faction f, ChitTypes cType, string cname)
	{
		planet = GameObject.FindGameObjectWithTag("Planet").GetComponent<Planet>();	
		//account for rotation
		Vector3 m =  planet.transform.localToWorldMatrix * midpoint;
		GameObject gameObject;
		switch (cType)
		{
			case ChitTypes.COMMANDER:
				gameObject = (GameObject)GameObject.Instantiate(Resources.Load("Commander"), m, Quaternion.LookRotation(midpoint));	
				gameObject.GetComponent<Character>().name = cname;
				gameObject.GetComponent<Chit>().movementRange = 2;
				gameObject.GetComponent<Character>().cType = cType;	
				break;
			case ChitTypes.CITY:		
				gameObject = (GameObject)GameObject.Instantiate(Resources.Load("City"), m, Quaternion.LookRotation(midpoint));	
				gameObject.GetComponent<Character>().name = cname;
				gameObject.GetComponent<Character>().cType = cType;					
				break;
			case ChitTypes.SWORD:
				gameObject = (GameObject)GameObject.Instantiate(Resources.Load("Sword"), m, Quaternion.LookRotation(midpoint));	
				gameObject.GetComponent<Chit>().movementRange = 2;	
				gameObject.tag = "Chit";
				break;
			case ChitTypes.NECTOWER:		
				gameObject = (GameObject)GameObject.Instantiate(Resources.Load("NecTower"), m, Quaternion.LookRotation(midpoint));	
				gameObject.GetComponent<Character>().name = "The Necromancer";
				gameObject.GetComponent<Character>().cType = cType;
				break;
			default:
				gameObject = new GameObject();
				break;
		}

		gameObject.transform.localScale = new Vector3(chitScalingFactor,chitScalingFactor,chitScalingFactor);
		gameObject.name = cType.ToString();
		
		chitOnTile = gameObject.GetComponent<Chit>();		
		chitOnTile.faction = f;
		chitOnTile.transform.parent = planet.transform;
		chitOnTile.tile = this;
		chitOnTile.ctype = cType;
		if (cType!=ChitTypes.NECTOWER)
		{
			gameObject.transform.FindChild("BaseChit").renderer.material.color = chitOnTile.faction.fcol;
		}
	}
	
	public void GrowTree()
	{
		planet = GameObject.FindGameObjectWithTag("Planet").GetComponent<Planet>();	

		//account for rotation
		Vector3 m =  planet.transform.localToWorldMatrix * midpoint;
		GameObject tree = (GameObject)GameObject.Instantiate(Resources.Load("Tree"), m, Quaternion.LookRotation(midpoint));	
		tree.transform.parent = this.transform;
		
	}
	

}
