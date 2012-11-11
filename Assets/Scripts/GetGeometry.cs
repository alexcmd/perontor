using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

static public class GetGeomtry {
	
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
	
		
}
