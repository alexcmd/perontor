using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter),typeof(MeshRenderer))]

public class ActiveRegionHoop : MonoBehaviour
{
	GameObject theHoop;
	Tile centreTile;
	MeshRenderer meshR;
	Planet planet;
	public Vector3[] perimeter;
	int pointsInLine;
	float radius;
	float tubeRadius = 0.01f;
	public bool hoopActive = false;
	public int segments = 32;
	public int tubes = 12;
	
	private static float Pi = 3.14159f;
	private float hoopTransformCounter = 0f;
	public float hoopBobMagnitude = 0.5f;
	public float hoopBobRate = 0.1f;

	// Use this for initialization
	void Start ()
	{
		theHoop = (GameObject)GameObject.Find("ActiveRegionHoop");
		meshR = theHoop.GetComponent<MeshRenderer>();
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		// animate hoop moving up and down... still to implement!
		if (hoopActive) {
			theHoop.transform.position = Vector3.zero;
			theHoop.transform.rotation = Quaternion.identity;		
			theHoop.transform.Rotate(centreTile.Elevation(), centreTile.Azimuth(), 0f);
			
			float sineWave = hoopBobMagnitude * Mathf.Sin((float)hoopTransformCounter);
			float hoopTranslateHeight = -centreTile.altitude + sineWave;
			
			theHoop.transform.Translate(0,0, hoopTranslateHeight);
			hoopTransformCounter += hoopBobRate;
			if (hoopTransformCounter > 1000.0f)
			{
				hoopTransformCounter = 0.0f;
			}
		}
	}
	
	public void setPlanet(Planet p)
	{
		planet = p;
		Debug.Log ("Setting Planet to: " + p);
	}
	
	public void SetPerimiterTiles (List<int> activeTiles, Tile cTile)
	{		
		centreTile = cTile;
		Vector3 centre = centreTile.midpoint;

		int frontier = activeTiles [0] + 1;
		activeTiles.Remove (0);
		pointsInLine = activeTiles.Count - frontier + 1;
		
		// get perimiter points, and we will need them ordered :(
		perimeter = new Vector3[pointsInLine];
		
		
		
		for (int i=0; i<pointsInLine-1; i++) {
			int tileSought = activeTiles [i + frontier];
			
			Debug.Log("Getting tiles and shit: (" + i + "|" + tileSought + ") " + planet.tiles[tileSought].midpoint.normalized);
			perimeter [i] = (0.1f + centre.magnitude) * planet.tiles [tileSought].midpoint.normalized;
			planet.tiles [activeTiles [i + frontier]].ActivateTile ();
			radius += Vector3.Distance (centre, perimeter [i]);
		}
		
		radius /= (pointsInLine - 1);
		
		Debug.Log ("Radius of active area: " + radius);
		
		drawActiveTorus (radius);
		
		perimeter = ReorderVertices(perimeter, centre);	
		hoopActive = true;
	}
	
	private void drawActiveTorus (float segmentRadius)
	{
		Vector3 centre = centreTile.midpoint;
		
		// Total vertices
		int totalVertices = segments * tubes;

		// Total primitives
		int totalPrimitives = totalVertices * 2;

		// Total indices
		int totalIndices = totalPrimitives * 3;

		// Init vertexList and indexList
		ArrayList verticesList = new ArrayList ();
		ArrayList indicesList = new ArrayList ();

		// Save these locally as floats
		float numSegments = segments;
		float numTubes = tubes;

		// Calculate size of segment and tube
		float segmentSize = 2 * Pi / numSegments;
		float tubeSize = 2 * Pi / numTubes;

		// Create floats for our xyz coordinates
		float x = 0;
		float y = 0;
		float z = 0;

		// Init temp lists with tubes and segments
		ArrayList segmentList = new ArrayList ();
		ArrayList tubeList = new ArrayList ();

		// Loop through number of tubes
		for (int i = 0; i < numSegments; i++) {
			tubeList = new ArrayList ();

			for (int j = 0; j < numTubes; j++) {
				// Calculate X, Y, Z coordinates.
				x = (segmentRadius + tubeRadius * Mathf.Cos (j * tubeSize)) * Mathf.Cos (i * segmentSize);
				y = (segmentRadius + tubeRadius * Mathf.Cos (j * tubeSize)) * Mathf.Sin (i * segmentSize);
				z = tubeRadius * Mathf.Sin (j * tubeSize);

				/*// Add the vertex to the tubeList
				tubeList.Add (new Vector3 (x, z, y));

				// Add the vertex to global vertex list
				verticesList.Add (new Vector3 (x, z, y));*/
					
				// Add the vertex to the tubeList
				tubeList.Add (new Vector3 (x, y, z));

				// Add the vertex to global vertex list
				verticesList.Add (new Vector3 (x, y, z));
			}

			// Add the filled tubeList to the segmentList
			segmentList.Add (tubeList);
		}

		// Loop through the segments
		for (int i = 0; i < segmentList.Count; i++) {
			// Find next (or first) segment offset
			int n = (i + 1) % segmentList.Count;

			// Find current and next segments
			ArrayList currentTube = (ArrayList)segmentList [i];
			ArrayList nextTube = (ArrayList)segmentList [n];

			// Loop through the vertices in the tube
			for (int j = 0; j < currentTube.Count; j++) {
				// Find next (or first) vertex offset
				int m = (j + 1) % currentTube.Count;

				// Find the 4 vertices that make up a quad
				Vector3 v1 = (Vector3)currentTube [j];
				Vector3 v2 = (Vector3)currentTube [m];
				Vector3 v3 = (Vector3)nextTube [m];
				Vector3 v4 = (Vector3)nextTube [j];

				// Draw the first triangle
				indicesList.Add ((int)verticesList.IndexOf (v1));
				indicesList.Add ((int)verticesList.IndexOf (v2));
				indicesList.Add ((int)verticesList.IndexOf (v3));

				// Finish the quad
				indicesList.Add ((int)verticesList.IndexOf (v3));
				indicesList.Add ((int)verticesList.IndexOf (v4));
				indicesList.Add ((int)verticesList.IndexOf (v1));
			}
		}
            
		Mesh mesh = new Mesh ();

		Vector3[] vertices = new Vector3[totalVertices];
		verticesList.CopyTo (vertices);
		
		
		int[] triangles = new int[totalIndices];
		indicesList.CopyTo (triangles);
		mesh.vertices = vertices;
		mesh.triangles = triangles;

		mesh.RecalculateBounds ();
		mesh.RecalculateNormals();
		mesh.Optimize ();
		MeshFilter mFilter = GetComponent (typeof(MeshFilter)) as MeshFilter;
		Debug.Log("mFilter is " + mFilter);
		mFilter.mesh = mesh;
			
	}
	
	
	private Vector3[] ReorderVertices (Vector3[] hexVertices, Vector3 m)
	{
		// now we need to wind the vertices in the correct direction
		// first get a rotation so the midpoint is at (0 0 1);
		Quaternion q = Quaternion.FromToRotation (m, Vector3.forward * m.magnitude);
		Vector2[] rotatedV = new Vector2[hexVertices.Length];
		float[] angles = new float[hexVertices.Length];
		for (int i=0; i<hexVertices.Length; i++) {
			rotatedV [i] = new Vector2 ((q * hexVertices [i]).x, (q * hexVertices [i]).y);
			angles [i] = Mathf.Atan2 (rotatedV [i].y, rotatedV [i].x);//*180/Mathf.PI;
			if (angles [i] < 0) {
				angles [i] += 2 * Mathf.PI;
			}
		}
		
		int[] idx = new int[hexVertices.Length];	
		for (int i=0; i<hexVertices.Length; i++) {
			idx [i] = i;
		}
		System.Array.Sort (angles, idx);	
		Vector3[] orderedV = new Vector3[hexVertices.Length];		
		for (int v=0; v<(idx.Length); v++) {
			orderedV [v] = hexVertices [idx [v]];
		}

		return orderedV;
		
		
	}
	
}
