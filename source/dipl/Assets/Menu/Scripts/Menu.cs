using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {
	
	public Texture backgroundTexture;

	private float guiPlacementX1 = 0.25f;
	private float guiPlacementY1 = 0.3f;
	private float guiPlacementX2 = 0.25f;
	private float guiPlacementY2 = 0.5f;
	private float guiPlacementX3 = 0.25f;
	private float guiPlacementY3 = 0.7f;

	void OnGUI(){
		//Display Background Texture
		GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), backgroundTexture);
		
		//Display Buttons
		/* if (GUI.Button (new Rect(Screen.width * guiPlacementX1, Screen.height * guiPlacementY1,Screen.width * .5f,Screen.height * .1f), "Resume"))
		{
			//------
		} */

		if (GUI.Button (new Rect(Screen.width * guiPlacementX2, Screen.height * guiPlacementY2,Screen.width * .5f,Screen.height * .1f), "New Game"))
		{
			Application.LoadLevel("dipl");
		}
		
		if (GUI.Button (new Rect(Screen.width * guiPlacementX3, Screen.height * guiPlacementY3,Screen.width * .5f,Screen.height * .1f), "Exit"))
		{
			Application.Quit();
		}
	}
	
}
