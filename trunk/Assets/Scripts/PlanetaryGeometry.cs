using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class PlanetaryGeometry : MonoBehaviour
{
	private float articCircle = 1.4f;
	private float seaLevel = 1.86f;
	private float equatorDesert = 0.4f;
	private int[] triangles; // list of vertex IDs for triangles
	public Vector3[] vertices;
	
	private List<Vector2> edges = new  List<Vector2>();
		private float edgeLength;
	
		public int nmbrTris;
		public int nmbrVertices;
		public int nmbrHexs;
		
		private List<List<int>> trisForEdges = new List<List<int>>();
		
		private float edgeAngle;
	
		public int numSubDivides; // controls the density of the hex grid
	
	
		public void scaleOcean()
		{
			GameObject ocean = (GameObject)GameObject.Find("Ocean");
			ocean.transform.localScale = new  Vector3(2*seaLevel, 2*seaLevel, 2*seaLevel);	
		}
		
		public float[] CreateTerrain(float scale)
		{
			Perlin noise = new Perlin();
			float gap = 0.1f;
			
			float[] alts = new float[vertices.Length];	
	
			for (var i=0;i<nmbrHexs;i++)
			{
				Vector3 vertex = vertices[i];
					
				vertex.x += noise.Noise(vertex.x, vertex.y, vertex.z) * scale;
				vertex.y += noise.Noise(vertex.x, vertex.y, vertex.z) * scale;
				vertex.z += noise.Noise(vertex.x, vertex.y, vertex.z) * scale;
				
				// NOTE: This section needs a bit of work!
				// freeze polar seas!
				if (Mathf.Abs(vertex.y)+0.5f>articCircle)
				{
					alts[i] = Mathf.Max(seaLevel+gap, 1+vertex.magnitude);				
				}			
				// adjust so we're now skimming sea level
				else if (Mathf.Abs(1+vertex.magnitude-seaLevel)<gap)
				{
					alts[i] = 1+vertex.magnitude+gap;	
				}
				else
				{			
					alts[i] = 1+vertex.magnitude;	
				}
			}		
			return alts;
		
		}
	
		static public Vector3[] CreateIcosahedronV()
		{
			// create points on Icosahedron with radius t
			Vector3[] v = new Vector3[12];
	
			float t = (1+Mathf.Sqrt(5))/2;
			float s = 1/Mathf.Sqrt(1+t*t);
			// create points
			v[0]  = s*(new Vector3( t, 1, 0));
			v[1]  = s*(new Vector3(-t, 1, 0));
			v[2]  = s*(new Vector3( t,-1, 0));
			v[3]  = s*(new Vector3(-t,-1, 0));
			v[4]  = s*(new Vector3( 1, 0, t));
			v[5]  = s*(new Vector3( 1, 0,-t));
			v[6]  = s*(new Vector3(-1, 0, t));
			v[7]  = s*(new Vector3(-1, 0,-t));
			v[8]  = s*(new Vector3( 0, t, 1));
			v[9]  = s*(new Vector3( 0,-t, 1));
			v[10] = s*(new Vector3( 0, t,-1));
			v[11] = s*(new Vector3( 0,-t,-1));
			
			return v;		
		}
		
		static public int[] CreateIcosahedronT()
		{
			int[] t = new int[60];
			t[0*3+0] = 0;   t[0*3+1] = 8;  t[0*3+2] = 4;
			t[1*3+0] = 0;   t[1*3+1] = 5;  t[1*3+2] = 10;
			t[2*3+0] = 2;   t[2*3+1] = 4;  t[2*3+2] = 9;
			t[3*3+0] = 2;   t[3*3+1] = 11; t[3*3+2] = 5;
			t[4*3+0] = 1;   t[4*3+1] = 6;  t[4*3+2] = 8;
			
			t[5*3+0] = 1;   t[5*3+1] = 10; t[5*3+2] = 7;
			t[6*3+0] = 3;   t[6*3+1] = 9;  t[6*3+2] = 6;
			t[7*3+0] = 3;   t[7*3+1] = 7;  t[7*3+2] = 11;
			t[8*3+0] = 0;   t[8*3+1] = 10; t[8*3+2] = 8;
			t[9*3+0] = 1;   t[9*3+1] = 8;  t[9*3+2] = 10;
			
			t[10*3+0] = 2;  t[10*3+1] = 9; t[10*3+2] = 11;
			t[11*3+0] = 3;  t[11*3+1] = 9; t[11*3+2] = 11;
			t[12*3+0] = 4;  t[12*3+1] = 2; t[12*3+2] = 0;
			t[13*3+0] = 5;  t[13*3+1] = 0; t[13*3+2] = 2;
			t[14*3+0] = 6;  t[14*3+1] = 1; t[14*3+2] = 3;
		
			t[15*3+0] = 7;  t[15*3+1] = 3; t[15*3+2] = 1;
			t[16*3+0] = 8;  t[16*3+1] = 6; t[16*3+2] = 4;
			t[17*3+0] = 9;  t[17*3+1] = 4; t[17*3+2] = 6;
			t[18*3+0] = 10; t[18*3+1] = 5; t[18*3+2] = 7;
			t[19*3+0] = 11; t[19*3+1] = 7; t[19*3+2] = 5;
			
			return t;		
		}
		
	static public int[] CreateTileTriangles(Vector3[] v)
	{
		bool isPent = v.Length==18;
		int[] tris;
		
		if (isPent)
		{
			tris = new int[3 * 15]; // 5 tile triangles + 2 triangles per edge face
			// tile triangles
			tris[3*0+0] = 12+0;
			tris[3*0+1] = 12+1;
			tris[3*0+2] = 12+2;
			
			tris[3*1+0] = 12+0;
			tris[3*1+1] = 12+2;
			tris[3*1+2] = 12+3;
				
			tris[3*2+0] = 12+0;
			tris[3*2+1] = 12+3;
			tris[3*2+2] = 12+4;
			
			tris[3*3+0] = 12+0;
			tris[3*3+1] = 12+4;
			tris[3*3+2] = 12+5;
			
			tris[3*4+0] = 12+0;
			tris[3*4+1] = 12+5;
			tris[3*4+2] = 12+1;
			
			// now do edge triangles
			tris[3*5+0]  = 7;
			tris[3*5+1]  = 2;
			tris[3*5+2]  = 1;
			tris[3*6+0]  = 7;
			tris[3*6+1]  = 8;
			tris[3*6+2]  = 2;

			tris[3*7+0]  = 8;
			tris[3*7+1]  = 3;
			tris[3*7+2]  = 2;
			tris[3*8+0]  = 8;
			tris[3*8+1]  = 9;
			tris[3*8+2]  = 3;

			tris[3*9+0]  = 9;
			tris[3*9+1]  = 4;
			tris[3*9+2]  = 3;
			tris[3*10+0] = 9;
			tris[3*10+1] = 10;
			tris[3*10+2] = 4;

			tris[3*11+0] = 10;
			tris[3*11+1] = 5;
			tris[3*11+2] = 4;
			tris[3*12+0] = 10;
			tris[3*12+1] = 11;
			tris[3*12+2] = 5;
			
			tris[3*13+0] = 11;
			tris[3*13+1] = 1;
			tris[3*13+2] = 5;
			tris[3*14+0] = 11;
			tris[3*14+1] = 7;
			tris[3*14+2] = 1;
		}
		else
		{	
			tris = new int[3 * 18]; // 6 tile triangles + 2 triangles per edge face
			
			tris[3*0+0] = 14+0;	
			tris[3*0+1] = 14+1;			
			tris[3*0+2] = 14+2;
			
			tris[3*1+0] = 14+0;
			tris[3*1+1] = 14+2;
			tris[3*1+2] = 14+3;
							
			tris[3*2+0] = 14+0;
			tris[3*2+1] = 14+3;
			tris[3*2+2] = 14+4;
			
			tris[3*3+0] = 14+0;
			tris[3*3+1] = 14+4;
			tris[3*3+2] = 14+5;
			
			tris[3*4+0] = 14+0;
			tris[3*4+1] = 14+5;
			tris[3*4+2] = 14+6;
			
			tris[3*5+0] = 14+0;
			tris[3*5+1] = 14+6;
			tris[3*5+2] = 14+1;
			
			// now do edge triangles
			tris[3*6+0]  = 8;
			tris[3*6+1]  = 2;
			tris[3*6+2]  = 1;
			tris[3*7+0]  = 8;
			tris[3*7+1]  = 9;
			tris[3*7+2]  = 2;

			tris[3*8+0]  = 9;
			tris[3*8+1]  = 3;
			tris[3*8+2]  = 2;
			tris[3*9+0]  = 9;
			tris[3*9+1]  = 10;
			tris[3*9+2]  = 3;

			tris[3*10+0]  = 10;
			tris[3*10+1]  = 4;
			tris[3*10+2]  = 3;
			tris[3*11+0] = 10;
			tris[3*11+1] = 11;
			tris[3*11+2] = 4;

			tris[3*12+0] = 11;
			tris[3*12+1] = 5;
			tris[3*12+2] = 4;
			tris[3*13+0] = 11;
			tris[3*13+1] = 12;
			tris[3*13+2] = 5;
			
			tris[3*14+0] = 12;
			tris[3*14+1] = 6;
			tris[3*14+2] = 5;
			tris[3*15+0] = 12;
			tris[3*15+1] = 13;
			tris[3*15+2] = 6;
			
			tris[3*16+0] = 13;
			tris[3*16+1] = 1;
			tris[3*16+2] = 6;
			tris[3*17+0] = 13;
			tris[3*17+1] = 8;
			tris[3*17+2] = 1;		
		}
		return tris;
	}	

	public void Sqrt3Subdivsion()
	{
		// create starting Icosahedron
		vertices  = CreateIcosahedronV();		
		triangles = CreateIcosahedronT();
		nmbrVertices = vertices.Length;
		nmbrTris = triangles.Length/3;
		// SQRT(3) SUBDIVISION
		for (int s=0; s< numSubDivides; s++) 
		{			
			edges = GetEdges();
			Vector3 e =  (vertices[(int)edges[0].x] - vertices[(int)edges[0].y]);
			edgeLength = e.magnitude;
			trisForEdges = GetTrianglesAlongEdge();
			nmbrHexs = vertices.Length;
			// now add all the subdivision new vertices
			vertices = SubdivideVertices(); 
			triangles =  SubdivideTriangles();
			nmbrVertices = vertices.Length;
			nmbrTris = triangles.Length/3;
		}
	}

	public bool PlaceTiles(float[] alts)
	{

		GameObject tile;
		// get vertices in default tile so I can work out what order they're in!
		for (int h=0; h<nmbrHexs; h++)//
		{
			// get neighbouring tiles
			List<int> nbringTiles = GetNbringTiles(h);
			float altitude = alts[h];
			Vector3[] hexVertices = GetHexVertices(h);
			TileType tileType = TileType.HEX;

			if (hexVertices.Length==5)
			{
				tileType = TileType.PENT; 				
			}
			hexVertices = ReorderVertices(hexVertices, vertices[h]);
			hexVertices = ExtrudeTile(hexVertices, altitude); // [tilesVerts, edgeVerts, baseVerts]

			int[] hexTriangles = CreateTileTriangles(hexVertices);

			Vector2[] uvs = new Vector2[hexVertices.Length];
    		int i = 0;
        	while (i < uvs.Length) 
			{
            	uvs[i] = new Vector2(hexVertices[i].x, hexVertices[i].z);
            	i++;
       	 	}	 
			Mesh hexMesh = new Mesh();
			hexMesh.vertices = hexVertices;		
			hexMesh.triangles = hexTriangles;
			hexMesh.RecalculateNormals();
			hexMesh.uv = uvs;
			tile = (GameObject)GameObject.Instantiate(Resources.Load("Tile"), Vector3.zero, Quaternion.identity);
			tile.GetComponent<MeshFilter>().sharedMesh = hexMesh;
			tile.GetComponent<MeshCollider>().mesh = hexMesh;
			
			tile.transform.parent = this.transform;
			tile.name = "Tile_" + h.ToString();
			Tile t = tile.AddComponent<Tile>();	
			t.id = h;
			t.nbrTiles = nbringTiles;
			t.midpoint = hexVertices[0];
			t.altitude = altitude;
			t.SetTileType(tileType);
			ApplyTerrainTexture(t, h, hexVertices[0], altitude);			
		}	
		
		/* generate Tile class objects and Factions! 
		 * probably doesn't really need to be done this way,
		 * but I can reuse more code if I do!
		 */

		return true;
	}
	
	public List<int> GetNbringTiles(int h)
{	
	List<int> nbringTiles = new List<int>();
	for (int g=0; g<nmbrHexs; g++)
	{
		if ((vertices[h] - vertices[g]).magnitude <= edgeLength)
		{
			nbringTiles.Add(g);	
			// quit early if we have everything
			if (nbringTiles.Count==7)
			{ 
				break;
			}
		}
	}
	return nbringTiles;
}
	
	private void ApplyTerrainTexture(Tile tile, int h, Vector3 m, float altitude)
{
	// apply terrain texture
	if (altitude<seaLevel)
	{
		tile.GetComponent<Tile>().terrain = Terrains.WATER;					
	}
	else if (Mathf.Abs(m.y) > articCircle)
	{
		tile.GetComponent<Tile>().terrain = Terrains.ICE;
	}
	else if(Mathf.Abs(m.y) < 0.4f)
	{
		tile.GetComponent<Tile>().terrain = Terrains.SAND;
	}
	else
	{
		if (Random.value<0.5f)
		{
			// spawn a tree!
			tile.GetComponent<Tile>().terrain = Terrains.FOREST;
		}
	}
}

	private Vector3[] ExtrudeTile(Vector3[] v, float a)
	{
		// we need two copies of the top tile verts as otherwise the normals go wonky
		Vector3[] vE = new Vector3[3*v.Length];
		a = Mathf.Max(1, a);

		for (int i=0; i<v.Length; i++)
		{
			v[i].Normalize();
			vE[i] = a * v[i];
			vE[i + v.Length] = v[i];
			vE[i + 2*v.Length] = vE[i];
		}
		return vE;
	}
	
	private Vector3[] ReorderVertices(Vector3[] hexVertices, Vector3 m)
	{
		// now we need to wind the vertices in the correct direction
		// first get a rotation so the midpoint is at (0 0 1);
		Quaternion q = Quaternion.FromToRotation(m, Vector3.forward*m.magnitude);
		Vector2[] rotatedV = new Vector2[hexVertices.Length];
		float[] angles = new float[hexVertices.Length];
		for (int i=0; i<hexVertices.Length; i++)
		{
			rotatedV[i] = new Vector2((q*hexVertices[i]).x, (q*hexVertices[i]).y);
			angles[i] = Mathf.Atan2(rotatedV[i].y, rotatedV[i].x);//*180/Mathf.PI;
			if (angles[i]<0)
			{
				angles[i] += 2* Mathf.PI;
			}
		}
		
		int[] idx = new int[hexVertices.Length];		
		idx[0] = 0;
		idx[1] = 1;
		idx[2] = 2;
		idx[3] = 3;
		idx[4] = 4;
		if (hexVertices.Length==6)
		{
			idx[5] = 5;
		}
		System.Array.Sort(angles, idx);	
		Vector3[] orderedV = new Vector3[hexVertices.Length+1];
		// add in centre point
		orderedV[0] = m;
		for (int v=1; v<(idx.Length+1); v++)
		{
			orderedV[v] = hexVertices[idx[v-1]];
		}
		// adjust midpoint so we make a flat pent/hex
		orderedV[0] = new Vector3();
		for (int i=1; i<orderedV.Length; i++)
		{
			orderedV[0] = orderedV[0] + orderedV[i];	
		}
		orderedV[0] = orderedV[0]/(orderedV.Length-1);

		return orderedV;
	}
			
	private Vector3[] GetHexVertices(int h)
	{
		List<Vector3> hexVertices  = new List<Vector3>();
		//find the 5 or 6 triangles that include point vertices[h] and add in the vertice ids. 
		for (int t=0; t<triangles.Length/3; t++)
		{
			if (triangles[3*t+0]==h | triangles[3*t+1]==h | triangles[3*t+2]==h)
			{
				// add in points of triangle that aren't already in our newHexList
				if (!hexVertices.Contains(vertices[triangles[3*t+1]]))
				{
					hexVertices.Add(vertices[triangles[3*t+1]]);
				}
				if (!hexVertices.Contains(vertices[triangles[3*t+2]]))
				{
					hexVertices.Add(vertices[triangles[3*t+2]]);
				}
			}			
		}
		return hexVertices.ToArray();
	}
		
	private int[] SubdivideTriangles()
	{
		int numberOfEdges = edges.Count;
		int[] newTriangles = new int[6*numberOfEdges];
		/* 
		 * for every edge, make two triangles, by connecting each end of the edge
		 * to the two midpoints in the adjacent triangles
		 */
		for (int e=0; e<numberOfEdges; e++)
		{			
			int t1 = trisForEdges[e][0];
			int t2 = trisForEdges[e][1];
			// first triangle
			newTriangles[6*e+0] = (int)edges[e][0];
			newTriangles[6*e+1] = nmbrVertices + t2;
			newTriangles[6*e+2] = nmbrVertices + t1;
			// second triangle
			newTriangles[6*e+3] = (int)edges[e][1];	
			newTriangles[6*e+4] = nmbrVertices + t2;
			newTriangles[6*e+5] = nmbrVertices + t1;
		}
		return newTriangles;		
	}
	
	private Vector3[] SubdivideVertices()
	{
		// create a new vertex in the centre of every triangle
		Vector3[] newVertices = new Vector3[vertices.Length + nmbrTris];
		//midpoints = new Vector3[nmbrTris];
		for (int v=0; v<vertices.Length; v++)
		{
			newVertices[v] = vertices[v];		
		}
		// for each triangle, create a new point in the centre
		int nV = vertices.Length;
		for (int n=0; n<nmbrTris; n++)
		{
			Vector3 v0 = vertices[triangles[3*n+0]];
			Vector3 v1 = vertices[triangles[3*n+1]];
			Vector3 v2 = vertices[triangles[3*n+2]];
			//midpoints[n] = ((v0+v1+v2)/3).normalized;
			newVertices[n + nV] = ((v0+v1+v2)/3);//.normalized;
		}
		return newVertices;	
	}
	
	private List<Vector2> GetEdges()
	{
		List<Vector2> edges = new List<Vector2>();
		// go through every triangle and check to see if edges are included
	//	Debug.Log("num Triangles: " + triangles.Length.ToString());
	//	Debug.Log("num Vertices: " + vertices.Length.ToString());	
		Vector2 e = new Vector2();
		for (int t=0; t<nmbrTris; t++)
		{
			e.x = triangles[3*t+0]; 
			e.y = triangles[3*t+1];
			if (edges.Contains(e) || edges.Contains(new Vector2(e.y, e.x)))
			{}
			else
			{
				edges.Add(e);
			}
			e.x = triangles[3*t+1]; 
			e.y = triangles[3*t+2];
			if (edges.Contains(e) || edges.Contains(new Vector2(e.y, e.x)))
			{}
			else
			{
				edges.Add(e);
			}
			e.x = triangles[3*t+0]; 
			e.y = triangles[3*t+2];
			if (edges.Contains(e) || edges.Contains(new Vector2(e.y, e.x)))
			{}
			else
			{
				edges.Add(e);
			}
		}
	//	edges = edges.Distinct();
		
//		Debug.Log(edges.Count.ToString() + "edges found");
		return edges.ToList();
	}
	
	List<List<int>> GetTrianglesAlongEdge()
	{
		List<List<int>> tfe = new List<List<int>>();

		// for each edge, find the two triangles that share it
		for (int e=0; e<edges.Count; e++)//
		{		
			//go through every triangle and see if it includes edge e
			List<int> trisForEdge = new List<int>();
			for (int t=0; t<nmbrTris; t++)
			{
				// edge vertice
				int ve1 = (int)edges[e][0];
				int ve2 = (int)edges[e][1];
				// triangle vertices
				int vt1 = triangles[3*t+0]; 
				int vt2 = triangles[3*t+1]; 
				int vt3 = triangles[3*t+2]; 
			    
				bool ve1inTri = (ve1==vt1 | ve1==vt2 | ve1==vt3);
				bool ve2inTri = (ve2==vt1 | ve2==vt2 | ve2==vt3);
				bool edgeInTri = ve1inTri & ve2inTri;

				if (edgeInTri)
				{
					trisForEdge.Add(t);
				}	
			//	if (trisForEdge.Count==2)
				//{
				//	break;
				//}
			}
			tfe.Add(trisForEdge);
		}	
		return tfe;
	}
	
}
	
	
