using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Assets.Script;
using UnityEngine;
using UnityEngine.SceneManagement;

class Game : MonoBehaviour
{
	
	public static Map map;
	public static List<Player> players;
	public static bool multi;

	private bool pauseGui = false;
	public bool pause = false;
	public bool multiplayer = false;

	public string startMap = "plat1";

	// Use this for initialization
	void Start()
	{

		players = new List<Player>();
		multi = multiplayer;
		
		Scene scene = SceneManager.GetActiveScene();

		switch (scene.name)
		{
			case "Level1":
				map = new Map("Level1", MapType.TEST, new Vector3(0,0,0));

				map.AddElement(new string[] { "button_1" }, "door_1", ExecType.DOOR);
				map.AddElement(new string[] { "button_2", "button_3" }, "window_1", ExecType.WINDOW);
				map.AddElement(new string[] { "button_4" }, "door_2", ExecType.DOOR);
				map.AddElement(new string[] { "button_5" }, "door_3", ExecType.DOOR);

				map.GetCheckpoints().Add(new Checkpoint(new Vector2(8.664959f, 138.28f), new Vector2(29.03885f, 185.8612f), new Vector3(19.224444f, 21.89996f, 132.9029f)));
				break;

			case "DemoScene":
				map = new Map("DemoScene", MapType.TEST, new Vector3(0,0,0));

				map.AddElement(new string[] { "button_1", "button_2", "button_3" }, "door_1", ExecType.DOOR);
				map.AddElement(new string[] { "button_4" }, "window_1", ExecType.WINDOW);
				break;
		}

        map = Map.GetMap(startMap);

		/*foreach (GameObject p in GameObject.FindGameObjectsWithTag("Player"))
			players.Add(new Player(players.Count, p, map));*/
	}

	// Update is called once per frame
	void Update()
	{
		foreach (Player p in players) 
			p.Move();

		/*
		//some key for s1
		if (Input.GetKeyDown(KeyCode.T))
		{
			SceneManager.LoadScene(2);
		}
		else if (Input.GetKeyDown(KeyCode.K))
		{
			SceneManager.LoadScene(1);
		}
		else if (Input.GetKeyDown(KeyCode.H))
		{
			players["Player1"].Tp(new Vector3(19.224444f, 21.89996f, 132.9029f));
		}*/

		if (Input.GetKeyDown(KeyCode.O))
		{
			map.GetButtons()[0].Push();
			map.GetButtons()[0].Exec();
		}

		if (Input.GetKeyDown(KeyCode.M))
		{
			GameObject.FindWithTag("MapGUI").transform.GetChild(0).gameObject.SetActive(true);
		}


		if (Input.GetKeyDown(KeyCode.P))
		{
			pause = !pause;
			pauseGui = true;
		}
		else
		{
			pause = GameObject.FindWithTag("PauseGUI").transform.GetChild(0).gameObject.activeSelf || GameObject.FindWithTag("MapGUI").transform.GetChild(0).gameObject.activeSelf;
			pauseGui = GameObject.FindWithTag("PauseGUI").transform.GetChild(0).gameObject.activeSelf;
		}

		if (pause)
			Time.timeScale = 0.0f;
		else
			Time.timeScale = 1.0f;

		if (pauseGui)
			GameObject.FindWithTag("PauseGUI").transform.GetChild(0).gameObject.SetActive(pause);
	}
	
	public void Save()
	{
		/*if (!Directory.Exists("saves"))
		{
			Directory.CreateDirectory("saves");
		}*/
		string toSave = "";

		toSave += "MAP:" + map.GetName() + "\n";

		//FIXME multiplayer
		Vector3 check = players[0].GetCurrentCheckpoint();
		toSave += "LASTCHECK:" + check.x + "," + check.y + "," + check.z + "\n";

		try
		{
			File.WriteAllText("save.txt", toSave);
		}
		catch(Exception e)
		{
			Debug.LogWarning("Error when trying to save : "+e.GetType());
		}
	}
	
	public void LoadMap(string name)
	{
		GameObject.FindWithTag("MapGUI").transform.GetChild(0).gameObject.SetActive(false);
		players.Clear();
		map.DestroyObjects();
		map = Map.GetMap(name);
	}
	
	public static int AddPlayer(GameObject obj)
	{
		int i = players.Count;
		players.Add(new Player(i, obj, map));
		return i;
	}
}
