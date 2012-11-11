using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Terraform : MonoBehaviour {
	
	float scale = 0.9f;

	Vector3[] baseVertices;
	private Perlin noise;
		
	public Vector3[] CreateTerrain(float seaLevel)
	{
		noise = new Perlin();
		Mesh mesh = transform.GetComponent<MeshFilter>().mesh;
	
		if (baseVertices == null) // not sure about the scope of this if. potentional bug
		{
			baseVertices = mesh.vertices;
		}
		
		Vector3[] vertices = new Vector3[baseVertices.Length];	

		for (var i=0;i<vertices.Length;i++)
		{
			Vector3 vertex = baseVertices[i];
				
			vertex.x += noise.Noise(vertex.x, vertex.y, vertex.z) * scale;
			vertex.y += noise.Noise(vertex.x, vertex.y, vertex.z) * scale;
			vertex.z += noise.Noise(vertex.x, vertex.y, vertex.z) * scale;
			
			// flatten out sea
			if (vertex.magnitude < seaLevel)
			{
				vertex = seaLevel * vertex.normalized;
			}		
			vertices[i] = vertex;		

		}			
		return vertices;
	}
}
