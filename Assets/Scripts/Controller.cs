using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Chit))]

public class Controller : MonoBehaviour {
	
	/* 
	 * Controller Class, used to characters and settlements
	 * settlements are characters as we assume each settlement has a governor/lord, etc.
	 * 
	 * range is the activation/command range and determines the radius of control
	 * commandRating determines how many chits this character can order
	 * remainingCR tracks how many order we have left each turn
	 * cType = commander/city/scout etc
	 */
	
	public int range = 2;
	public int commandRating = 2;
	
	// used to keep track of how many units we've moved
	public int remainingCR = 0;	
	
	// only really used when loading.
	public int tileID;
}
