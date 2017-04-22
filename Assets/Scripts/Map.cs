using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Script
{
    class Map
    {
		//level name
    	private string name;
		private MapType type;

		private List<Checkpoint> checkpoints;
		private List<Executable> executables;

		private List<Button> buttons;

		public Map(List<MapConstructor> constructors, string name, MapType type) : this(name, type)
        {
			foreach(MapConstructor constructor in constructors)
				constructor.Construct();
        }
        
		public Map(MapConstructor constructor, string name, MapType type) 
			: this(new List<MapConstructor>(new MapConstructor[] {constructor}), name, type){}
		
		public Map(string name, MapType type)
        {
        	this.name = name;
			this.type = type;

			this.checkpoints = new List<Checkpoint>();


			this.executables = new List<Executable>();

			this.buttons = new List<Button>();
        }       

		public List<Checkpoint> GetCheckpoints()
		{
			return this.checkpoints;
		}

		public void SetExecutables(List<Executable> executables)
		{
			this.executables = executables;
		}
		public List<Executable> GetExecutables()
		{
			return this.executables;
		}

		public List<Button> GetButtons()
		{
			return this.buttons;
		}

		//add an element
		//AddElement(new string[] { "button_1" }, "door_1", ExecType.DOOR);
		public void AddElement(string[] buts, string element, ExecType type, bool tag = true)
		{
			List<Button> buttonsElement = new List<Button>();

			foreach(string but in buts)
			{
				Debug.Log(but);
				if (tag)
					buttonsElement.Add(new Button(null, GameObject.FindWithTag(but + "_true"), GameObject.FindWithTag(but + "_false")));
				else
					buttonsElement.Add(new Button(null, GameObject.Find(but + "_true"), GameObject.Find(but + "_false")));
			}

			switch(type)
			{
				case ExecType.DOOR:
					if (tag)
						this.AddExec(new Door(null, GameObject.FindWithTag(element), GameObject.FindWithTag(element + "_left"), GameObject.FindWithTag(element + "_right")), buttonsElement);
					else
                    	this.AddExec(new Door(null, GameObject.Find(element), GameObject.Find(element + "_left"), GameObject.Find(element + "_right")), buttonsElement);
					break;

				case ExecType.WINDOW:
					if (tag)
						this.AddExec(new Window(null, GameObject.FindWithTag(element)), buttonsElement);
					else
						this.AddExec(new Window(null, GameObject.Find(element)), buttonsElement);
					break;
			}
		}

		//add an executable in the map
		public void AddExec(Executable exec, List<Button> buttons)
		{
			foreach(Button b in buttons)
				b.SetExecutable(exec);
			
			exec.SetButtons(buttons);

			this.executables.Add(exec);
			this.buttons.AddRange(buttons);
		}

		//call each time a laser hit an object
		public void Shoot(GameObject gameObject)
		{
			Debug.Log("Shoot : "+gameObject.name+" - nb : "+buttons.Count);
			Button b = null;
			for (int i = 0; b == null && i < buttons.Count; i++)
			{
				Debug.Log(buttons[i].GetActive().name+" | "+buttons[i].GetNonActive().name);
				if (buttons[i].IsThisGameObject(gameObject))
					b = buttons[i];
			}

			if (b != null)
			{
				b.Push();
				b.Exec();
			}
		}

        public static Map GetMap(string name)
        {
            switch (name)
            {
                case "mctest":
                    List<Block> testB = new List<Block>();
                    testB.Add(new Block(BlockType.Icorridor, new Vector3(0, 0, 0), 0));
                    testB.Add(new Block(BlockType.Icorridor, new Vector3(24, 0, 0), 0));
                    MapConstructor test = new MapConstructor(testB);

                    return new Map(test, "oklolsalut", MapType.TEST);
                    break;

                case "map1":
                    List<Block> MapTest = new List<Block>();
                    MapTest.Add(new Block(BlockType.Icorridor, new Vector3(0,0,0), 0));
                    MapTest.Add(new Block(BlockType.Icorridor, new Vector3(24, 0, 0), 0));
                    MapTest.Add(new Block(BlockType.Lcorridor, new Vector3(24, 0, 0), 0));
                    MapTest.Add(new Block(BlockType.Icorridor, new Vector3(36, 0, -24), -90));
                    MapTest.Add(new Block(BlockType.Door3D, new Vector3(6, 2, -6), 90, "door_1"));
                    MapTest.Add(new Block(BlockType.Lcorridor, new Vector3(60, 0, -60), 180));
                    //MapTest.Add(new Block(BlockType.Enigme1, new Vector3(91, 23, -71), 180));			
					MapTest.Add(new Block("Icorridor3x6_col", new Vector3(117.05f,0.1300011f,-59.97f), new Vector3(0f, 88.00001f, 0)));
					MapTest.Add(new Block("FloorNeonAlea_col", new Vector3(107f,0.1244316f,-60.02477f), new Vector3(0f, 180f, 0)));
					MapTest.Add(new Block("Lcorridor2_col", new Vector3(106.1292f,0.1244316f,-84.1f), new Vector3(0f, 180f, 0)));
					MapTest.Add(new Block("FloorNeonAlea_col", new Vector3(106.1292f,0.1244316f,-84.02477f), new Vector3(0f, 180f, 0)));
					MapTest.Add(new Block("Lcorridor2_col", new Vector3(83.1f,0.1244316f,-48.1f), new Vector3(0f, 270f, 0)));
					MapTest.Add(new Block("Lcorridor2_col", new Vector3(118f,0.1244316f,-24.2f), new Vector3(0f, 0f, 0)));
					MapTest.Add(new Block("Icorridor3x6_col", new Vector3(105.5f,0.6415367f,-74f), new Vector3(0f, 88.00001f, 0)));
					MapTest.Add(new Block("Icorridor3x6_col", new Vector3(98.84f,0.1400013f,-60.18f), new Vector3(0f, 88.00001f, 0)));
					MapTest.Add(new Block("Icorridor2_col", new Vector3(107.0792f,0.1244316f,-48.01477f), new Vector3(0f, 270f, 0)));
					MapTest.Add(new Block("Icorridor_col", new Vector3(106.2292f,0.1244316f,-96.12477f), new Vector3(0f, 270f, 0)));
					MapTest.Add(new Block("Icorridor3x6_col", new Vector3(111f,0.6415367f,-73.8f), new Vector3(0f, 88.00001f, 0)));
					MapTest.Add(new Block("windows3D_col", new Vector3(111.9092f,8.923815f,-89.37477f), new Vector3(0f, 180f, 0), "window_1"));
					MapTest.Add(new Block("FloorNeonAlea_col", new Vector3(83.1f,0.1244316f,-60.02477f), new Vector3(0f, 180f, 0)));
					MapTest.Add(new Block("FloorNeonAlea_col", new Vector3(130.9f,0.1244316f,-60.02477f), new Vector3(0f, 180f, 0)));
					MapTest.Add(new Block("Icorridor3x6_col", new Vector3(106.5992f,0.2138081f,-60.05003f), new Vector3(0f, 180f, 0)));
					MapTest.Add(new Block("Icorridor_col", new Vector3(59.72915f,0.1244316f,-59.92477f), new Vector3(0f, 180f, 0)));
					MapTest.Add(new Block("Icorridor3x6_col", new Vector3(117.8292f,0.3138084f,-48.24477f), new Vector3(0f, 0f, 0)));
					MapTest.Add(new Block("Icorridor3x6_col", new Vector3(117.8292f,0.3138084f,-65.76f), new Vector3(0f, 0f, 0)));
					MapTest.Add(new Block("Icorridor3x6_col", new Vector3(107.0792f,0.04380798f,-42.04477f), new Vector3(0f, 180f, 0)));
					MapTest.Add(new Block("windows3D_col", new Vector3(147.4f,8.923815f,-77.5f), new Vector3(0f, 270f, 0), "window_2"));
					MapTest.Add(new Block("Icorridor_col", new Vector3(153.9f,0.1244316f,-83.2f), new Vector3(0f, 180f, 0)));
					MapTest.Add(new Block("Icorridor3x6_col", new Vector3(117.8292f,0.3138084f,-71.9f), new Vector3(0f, 0f, 0)));
					MapTest.Add(new Block("Icorridor3x6_col", new Vector3(117.8292f,0.3138084f,-89.74f), new Vector3(0f, 0f, 0)));
					MapTest.Add(new Block("Icorridor3x6_col", new Vector3(123.3f,0.3138084f,-83.4f), new Vector3(0f, 0f, 0)));
					MapTest.Add(new Block("Icorridor3x6_col", new Vector3(123.3f,0.3138084f,-77.3f), new Vector3(0f, 0f, 0)));
					MapTest.Add(new Block("Button", new Vector3(113.0092f,10.11381f,-101.2148f), new Vector3(0f, 180f, 0), "button_1"));
					MapTest.Add(new Block("Button", new Vector3(146.5f,5.550602f,-37.2305f), new Vector3(0f, 270f, 0), "button_2"));
					MapTest.Add(new Block("Button", new Vector3(79.53915f,9.683808f,-43.08684f), new Vector3(0f, 270f, 0), "button_3"));
					MapTest.Add(new Block("Button", new Vector3(151.2691f,10.02381f,-76.16476f), new Vector3(0f, 270f, 0), "button_4"));
					MapTest.Add(new Block("Button", new Vector3(109.9093f,3.955154f,-19.51477f), new Vector3(0f, 180f, 0), "button_5"));
					MapTest.Add(new Block("Icorridor2_col", new Vector3(142.1392f,0.1244316f,-60.02477f), new Vector3(0f, 90f, 0)));
					MapTest.Add(new Block("Door3D_col", new Vector3(146.7791f,2.87381f,-53.90477f), new Vector3(0f, 270f, 0), "door_2"));
					MapTest.Add(new Block("Icorridor_col", new Vector3(154.6692f,0.1244316f,-59.79901f), new Vector3(0f, 180f, 0)));
					
                    MapConstructor test1 = new MapConstructor(MapTest);

                    Map map1 = new Map(test1, "oklolsalut", MapType.TEST);
                    map1.AddElement(new string[] { "button_3", "button_5", "button_2" }, "window_1", ExecType.WINDOW, false);
					map1.AddElement(new string[] { "button_1" }, "window_2", ExecType.WINDOW, false);
					map1.AddElement(new string[] { "button_4" }, "door_2", ExecType.DOOR, false);
					return map1;
                    break;

                case "plat1":
                    List<Block> Map = new List<Block>();
                    Map.Add(new Block("Button", new Vector3(-146, 15, -7), 90, "button_1"));
                    Map.Add(new Block("plat/Mur_col", new Vector3(-134, 6, -6), 90));
                    Map.Add(new Block("plat/Mur_col", new Vector3(-127, 18, -12), 0));
                    Map.Add(new Block("plat/Mur_col", new Vector3(-129, 19, 0), 180));
                    Map.Add(new Block("plat/Mur_col", new Vector3(-147, 18, -6), 90));
                    Map.Add(new Block("plat/Plateforme_col", new Vector3(-130, 6, -7), 0));
                    Map.Add(new Block("plat/trap_pilier_col", new Vector3(-98, -2, -7), 0));
                    Map.Add(new Block("plat/trap_pilier_col", new Vector3(-107, 0, -4), 0));
                    Map.Add(new Block("plat/trap_pilier_col", new Vector3(-121, 3, -4), 0));
                    Map.Add(new Block("plat/Icorridor_plat_col", new Vector3(-94, 0, 0), 0));
                    Map.Add(new Block("plat/Icorridor_plat_col", new Vector3(-25, 0, -36), 90));
                    Map.Add(new Block("plat/Icorridor_plat_col", new Vector3(-139, 12, 0), 0));
                    Map.Add(new Block("plat/Icorridor_plat_col", new Vector3(0, 0, 0), 0));
                    Map.Add(new Block("plat/Icorridor_plat_col", new Vector3(-117, 0, 0), 0));
                    Map.Add(new Block("plat/Tcorridor_plat_col", new Vector3(-48, 0, 0), 0));
                    Map.Add(new Block("plat/Door3D_plat_col", new Vector3(4, 2, -6), 90));
                    Map.Add(new Block("plat/Door3D_plat_col", new Vector3(-31, 2, -41), 0, "door_1"));
                    Map.Add(new Block("plat/corridoirscile", new Vector3(-74, -84, -8), 0));
                    MapConstructor test2 = new MapConstructor(Map);
					
                    Map plat1 = new Map(test2, "oklolsalut", MapType.TEST);
					plat1.AddElement(new string[] { "button_1" }, "door_1", ExecType.DOOR, false);
					return plat1;
                    break;

                default:	
                    return null;
                    break;
            }
        }

    }
    

	enum MapType
	{
		SPEED,
		SMART,
		WAIT,
		TEST,
		OTHER
	}
}
