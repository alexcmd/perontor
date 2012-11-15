using UnityEngine;
using System.Collections;

public class CamControl : MonoBehaviour
{
	
	/* 
	 * as the name suggests, this controls the main camera
	 * flaled at by John Harland
	 * */
	
	float zoomSens = 1.0f; // zoom sensitivity
	float rotSpeed = 0.1f; // rotation speed
	public float rotSpeedStep = 0.1f;
	public bool enableSwoop = false;
	float zPos = -9f;
	bool changedAngles = false;
	GameObject planet;
	
	// links to vertical and horizontal rotation anchors
	Transform anchor;
	float currentElevation = 0.0f;
	float endElevation;
	float currentAzimuth = 0.0f;
	float translateAmount = 0;
	public float transitionDuration = 1.5f;
	public float maxScrollSpeed = 2.0f;
	float vertScrollSpeed = 0.5f;
	public float startSwooping = 2.0f;
	public float maxZoom = 1.60f;
	public float maxElevation = 45.0f;
	
	
	// scroll boundaries: when the mouse is near the edge of the screen we rotate	
	int scrollL, scrollR, scrollU, scrollD;
	int scrollLimitH = 2; // % of screen where horizontal scrolling takes place
	int scrollLimitV = 5; // % of screen where vertical scrolling takes place
	
	void Start ()
	{
		// link stuff
		planet = GameObject.FindGameObjectWithTag ("Planet");	
		//anchorV = GameObject.Find("CamAnchorV").transform;
		anchor = GameObject.Find ("CamAnchor").transform;
		
		// sort out boundaries for mouse scroll/rotate		
		scrollL = scrollLimitH * (Screen.width / 100);		
		scrollR = Screen.width - scrollL;
		scrollU = scrollLimitV * Screen.height / 100;
		scrollD = Screen.height - scrollU;	
		
		// Set to default positions
		anchor.transform.position = new Vector3 (0, 0, 0);
		anchor.transform.rotation = Quaternion.identity;
		anchor.Rotate (currentElevation, currentAzimuth, 0);
		anchor.Translate (0, translateAmount, zPos);
	}
	
	IEnumerator InterpolateAngles (float az, float el)
	{
		float startAz = currentAzimuth;
		float startEl = currentElevation;
		float tempAz, tempEl;
		
		float interp = 0.0f;
		
		while (interp < 1.0f) {
			
			interp = interp + (Time.deltaTime * (Time.timeScale / transitionDuration));
			if (interp > 1.0f) {
				interp = 1.0f;
			}
			//Debug.Log("i"+ interp);
			anchor.transform.position = new Vector3 (0, 0, 0);
			tempAz = Mathf.LerpAngle (startAz, az, interp);
			tempEl = Mathf.LerpAngle (startEl, el, interp);
			anchor.transform.rotation = Quaternion.identity;
			anchor.Rotate (tempEl, tempAz, 0);
			anchor.Translate (0, 0, zPos);
			yield return new WaitForEndOfFrame();
		}
		
		return true;
	}
	
	public void CentreCamera (Tile t)
	{
		/*
		 * this function is called at the start of each turn.
		 * We should probably add a "centre camera" button too so the player 
		 * can centre the camera
		 * */
		
		// first, reset camera rotation.
		// ideally, we wouldn't have to do this, maybe?
		anchor.transform.rotation = Quaternion.identity;
		//anchorV.transform.rotation = Quaternion.identity;
		
//		Debug.Log (t.Azimuth () + ", " + t.Elevation ());
		
		//float az = 180 + t.Azimuth();
		float az = t.Azimuth ();
		
		// adjust elevation angle so we do not end up to close to the North
		// or South poles 
		float el = (t.Elevation () + 90) % 360;
		el = Mathf.Min (175, el);
		el = Mathf.Max (15, el);
		el = el - 90;	
	
		StartCoroutine (InterpolateAngles (az, el));
		currentAzimuth = az;
		currentElevation = el;
		//Debug.Log("CentreCamera " + currentAzimuth + ", " + currentElevation);
	}
	
	void FixedUpdate ()
	{		
		/* this listens for mouse scroll/zoom, rotate keys etc and acts accordingly. 
		 * it would be nice to allow for orbit speed to accelerate a little to give 
		 * smoother behaviour
		 * */
		changedAngles = false;
		CheckForZoom ();		
		CheckForScrolling ();
		// now rotate
		
		float fandAngle = 0;
		if (changedAngles) {	
			
			currentAzimuth = currentAzimuth % 360.0f;
			
			// Calculate the zoom swoop..ness
			translateAmount = 0;
			endElevation = currentElevation;
			if (enableSwoop) {
				if (Mathf.Abs (zPos) < startSwooping) {
					Debug.Log ("doing zoom swoop");
					endElevation = currentElevation;
				
					// a nasty bodge to get the position 
					// of the camera before the
					// zoom swoop transform had been applied
					anchor.transform.position = new Vector3 (0, 0, 0);
					anchor.transform.rotation = Quaternion.identity;
					anchor.Rotate (currentElevation, currentAzimuth, 0);
					anchor.Translate (0, 0, zPos);
		
					RaycastHit bumpedSomething;
					Debug.Log (anchor.position);
					bool collide = Physics.Linecast (anchor.position, Vector3.zero, out bumpedSomething);
					float distance = 0;
					if (collide) {
						Debug.Log ("Line hit something (" + bumpedSomething.collider.ToString () + ") " + bumpedSomething.distance + " units away from origin");
						distance = bumpedSomething.distance;
					}
					
					// Form a right angle triangle, with the right-angle at the original position of the camera
					// ------- new cam. pos
					// |¬    /
					// |    /
					// |   /
					// |  /
					// | /
					// |/
					// thing looked at (angle here 45 degrees)
					float interpolant = (startSwooping - Mathf.Abs (zPos)) / (startSwooping - maxZoom);
				
					Debug.Log (startSwooping + " - " + Mathf.Abs (zPos) + " = " + (startSwooping + zPos));
				
					fandAngle = Mathf.Lerp (0, 45, interpolant);
					Debug.Log (-zPos + " Interpolant " + interpolant + " gives angle " + fandAngle);
					
					endElevation = currentElevation - fandAngle;
				
					Debug.Log ("Post-flange angle: " + endElevation);
				
					translateAmount = Mathf.Tan (fandAngle * Mathf.Deg2Rad) * distance;
					changedAngles = true;
					
					Debug.Log ("Transform by " + Mathf.Tan (fandAngle * Mathf.Deg2Rad) + " * " + distance + " = " + translateAmount);
				}		
			}
	
			anchor.transform.position = new Vector3 (0, 0, 0);
			anchor.transform.rotation = Quaternion.identity;
			anchor.Rotate (endElevation, currentAzimuth, 0);
			anchor.Translate (0, translateAmount, zPos);	
					
		}
		
	}
	
	void CheckForScrolling ()
	{
		float scrollRot = 0;	
		
		/*if (Input.GetAxis("Horizontal")<0)		{
			anchorH.transform.RotateAround(Vector3.up, rotSpeed);
		}
		else if (Input.GetAxis("Horizontal")>0)		{
			anchorH.transform.RotateAround(Vector3.up, -rotSpeed);
		}*/
		
		// do some fiddling with angles to avoid the 360=0degrees crossover
		int rotshift = (int)(anchor.localEulerAngles.x + 90) % 360;
		
		//if ((Input.GetAxis("Vertical")<0)&(rotshift>15))		{
		//	anchorV.transform.RotateAroundLocal(Vector3.left, rotSpeed);
		//
		//}
		//else if ((Input.GetAxis("Vertical")>0)&(rotshift<175))		{
		//	anchorV.transform.RotateAroundLocal(Vector3.left, -rotSpeed);
		//}
		
		// check for mouse scrolling		
		if ((Input.mousePosition [0] < scrollL ) || (Input.GetAxis("Horizontal")<0)) {

//			Debug.Log ("Scroll left " + Input.mousePosition[0] + " | " + rotSpeed);
			if (rotSpeed < maxScrollSpeed) {
				rotSpeed = rotSpeed + rotSpeedStep;
			}
			currentAzimuth = currentAzimuth + rotSpeed;
			changedAngles = true;
		} else if ((Input.mousePosition [0] > scrollR) || (Input.GetAxis("Horizontal")>0)) {
//			Debug.Log ("Scroll right " + Input.mousePosition[0] + " | " + rotSpeed);
			if (rotSpeed > -maxScrollSpeed) {
				rotSpeed = rotSpeed - rotSpeedStep;
			}
			currentAzimuth = currentAzimuth + rotSpeed;
			changedAngles = true;
		} else {
			rotSpeed = 0;
		}
		

		// now detect vertical scrolling
		// but only if the cursor's in the window
		//if ((Input.mousePosition [1] > 0 && Input.mousePosition [1] < Screen.height) || ) {
			if (Input.mousePosition [1] > scrollD || (Input.GetAxis("Vertical")>0)) {
				if (currentElevation < maxElevation) {
					currentElevation = currentElevation + vertScrollSpeed;	
					changedAngles = true;
				}
				}
				
			 else if ((Input.mousePosition [1] < scrollU) || (Input.GetAxis("Vertical")<0)) {
				if (currentElevation > -maxElevation) {
					currentElevation = currentElevation - vertScrollSpeed;
					changedAngles = true;
				}
			
		}
			
	}
	
	void CheckForZoom ()
	{
		float scrollness = Input.GetAxis ("Mouse ScrollWheel");
		if ((scrollness > 0) & (zPos < -maxZoom)) {
			//transform.Translate(new Vector3(0, 0, zoomSens * Input.GetAxis("Mouse ScrollWheel")));
			float step = (zoomSens * scrollness);
			zPos = zPos + step;
			changedAngles = true;
			
			if (zPos > -maxZoom) {
				zPos = -maxZoom;
			}
		} else if ((scrollness < 0) & (zPos > -50f)) {
			//transform.Translate(new Vector3(0, 0, zoomSens * Input.GetAxis("Mouse ScrollWheel")));
			float step = (zoomSens * Input.GetAxis ("Mouse ScrollWheel"));
			zPos = zPos + step;
			changedAngles = true;
		}
	}	
	
	public void OutlineTile(Tile tile)
	{
		/*
		 * This is meant to draw an outline round the currently selected tile
		 * It doesn't work properly, and I can't be bothered going back to work 
		 * out which vertices in the tile's mesh give the "top face"
		 * 
		 * Will fix at a later date
		 */
		Vector3[] pts = tile.GetTileCorners();
		LineRenderer outline = transform.parent.GetComponent<LineRenderer>();
		int npts = pts.Length;
		
		outline.SetVertexCount(npts+1);
		
		for (int p = 0; p<npts; p++)
		{
			outline.SetPosition(p, pts[p]);
		}
		outline.SetPosition(npts, pts[0]);
	}
}
