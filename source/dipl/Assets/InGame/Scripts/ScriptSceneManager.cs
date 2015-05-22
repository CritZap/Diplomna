using UnityEngine;


public class ScriptSceneManager : MonoBehaviour 
{
	public int aircraftLanded = 0;
	public int aircraftLost  = 0;
	public float gameTimer = 150f;

	void FixedUpdate () 
	{
		gameTimer -= Time.deltaTime;
	}

	void OnGUI () 
	{

		if (gameTimer <= 0) 
		{
			var style = new GUIStyle("label");
			style.fontSize = 50;
			GUI.Label(new Rect((Screen.width/2)-75, (Screen.height/2)-75, 200, 200), "GAME OVER", style);
			Time.timeScale = 0;

			if(GUI.Button (new Rect((Screen.width/2)-75, (Screen.height/2)+50, 165, 50), "Back to menu"))
			{
				Application.LoadLevel("Menu");
			}

		}

		GUI.Box(new Rect(10, 10, 100, 30), "Landed : "+ aircraftLanded);
		GUI.Box(new Rect(10, 45, 100, 30), "Lost X : "+ aircraftLost);
		
		GUI.Box(new Rect((Screen.width/2)-75, 10, 165, 30), "Time Left : "+ gameTimer);

		if (gameTimer > 0) 
		{
			if (GUI.Button (new Rect ((Screen.width/2)+150, 10, 30, 30), "||")) 
			{
				if (Time.timeScale == 1) 
				{
					Time.timeScale = 0;
				}
				else 
				{
					Time.timeScale = 1;
				}
			}
		}

	}

}
