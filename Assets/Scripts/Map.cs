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

		private List<Vector3> spawnPoint;

		private List<MapConstructor> constructors;

		public Map(List<MapConstructor> constructors, string name, MapType type, Vector3 spawnPoint) : this(constructors, name, type, new List<Vector3>(new Vector3[] {spawnPoint})){}
        public Map(List<MapConstructor> constructors, string name, MapType type, List<Vector3> spawnPoint) : this(name, type, spawnPoint)
        {
			int i = 0;
			foreach (MapConstructor constructor in constructors)
			{

				if (i == 0)	
				{
					if (Game.multi)
					{
						constructor.AddObject("Network/Network Manager");
						foreach(Vector3 loc in spawnPoint)
						{
							constructor.blocks.Add(new Block("Network/SpawnPoint", loc, 0));
						}
					}
					else
					{
						constructor.blocks.Add(new Block("FPSController", spawnPoint[0], 0));
					}
				}
	
				constructor.Construct();
				i++;
			}

			this.constructors = constructors;
        }
        
        
		public Map(MapConstructor constructor, string name, MapType type, Vector3 spawnPoint) : this(constructor, name, type, new List<Vector3>(new Vector3[] {spawnPoint})) {}
		public Map(MapConstructor constructor, string name, MapType type, List<Vector3> spawnPoint) : this(new List<MapConstructor>(new MapConstructor[] {constructor}), name, type, spawnPoint){}
		
		public Map(string name, MapType type, Vector3 spawnPoint) : this(name, type, new List<Vector3>(new Vector3[] {spawnPoint})) {}
		public Map(string name, MapType type, List<Vector3> spawnPoint)
        {
        	this.name = name;
			this.type = type;

			this.checkpoints = new List<Checkpoint>();


			this.executables = new List<Executable>();

			this.buttons = new List<Button>();

			this.spawnPoint = spawnPoint;

			this.constructors = new List<MapConstructor>();
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
		
		public string GetName()
		{
			return this.name;
		}

		//add an element
		//AddElement(new string[] { "button_1" }, "door_1", ExecType.DOOR);
		public void AddElement(string[] buts, string element, ExecType type, bool tag = true)
		{
			List<Button> buttonsElement = new List<Button>();

			foreach(string but in buts)
			{
				if (tag)
					buttonsElement.Add(new Button(null, GameObject.FindWithTag(but + "_true"), GameObject.FindWithTag(but + "_false"), tag));
				else
					buttonsElement.Add(new Button(null, GameObject.Find(but + "_true"), GameObject.Find(but + "_false"), tag));
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
			Button b = null;
			for (int i = 0; b == null && i < buttons.Count; i++)
			{
				if (buttons[i].IsThisGameObject(gameObject))
					b = buttons[i];
			}

			if (b != null)
			{
				b.Push();
				b.Exec();
			}
		}
		
		public void DestroyObjects()
		{
			foreach (MapConstructor constructor in constructors)
				constructor.DeConstruct();
				
			this.checkpoints.Clear();
			this.executables.Clear();
			this.buttons.Clear();
			this.spawnPoint.Clear();
			this.constructors.Clear();
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

                    return new Map(test, name, MapType.TEST, new Vector3(0,0,0));
                    break;

                case "map1":
                    List<Block> MapTest = new List<Block>();
                    MapTest.Add(new Block(BlockType.Icorridor, new Vector3(0,0,0), 0));
                    MapTest.Add(new Block(BlockType.Icorridor, new Vector3(24, 0, 0), 0));
                    MapTest.Add(new Block(BlockType.Lcorridor, new Vector3(24, 0, 0), 0));
                    MapTest.Add(new Block(BlockType.Icorridor, new Vector3(36, 0, -24), -90));
                    MapTest.Add(new Block(BlockType.Door3D, new Vector3(-17, 2, -6), 90, "door_1"));
                    MapTest.Add(new Block(BlockType.Door3D, new Vector3(77, 2, -54), 90, "door_3"));
                    MapTest.Add(new Block(BlockType.Lcorridor, new Vector3(60, 0, -60), 180));
                    //MapTest.Add(new Block(BlockType.Enigme1, new Vector3(91, 23, -71), 180));			
					MapTest.Add(new Block("Icorridor3x6_col", new Vector3(147.16f,6.5f,-48.244f), new Vector3(0f, 0f, 0)));
					MapTest.Add(new Block("FloorNeonAlea_col", new Vector3(107f,0.1244316f,-60.02477f), new Vector3(0f, 180f, 0)));
					MapTest.Add(new Block("Lcorridor2_col", new Vector3(106.1292f,0.1244316f,-84.1f), new Vector3(0f, 180f, 0)));
					MapTest.Add(new Block("FloorNeonAlea_col", new Vector3(106.1292f,0.1244316f,-84.02477f), new Vector3(0f, 180f, 0)));
					MapTest.Add(new Block("Lcorridor2_col", new Vector3(83.1f,0.1244316f,-48.1f), new Vector3(0f, 270f, 0)));
					MapTest.Add(new Block("Lcorridor2_col", new Vector3(118f,0.1244316f,-24.2f), new Vector3(0f, 0f, 0)));
					MapTest.Add(new Block("Icorridor3x6_col", new Vector3(147.16f,6.5f,-65.6f), new Vector3(0f, 0f, 0)));
					MapTest.Add(new Block("Icorridor3x6_col", new Vector3(147.16f,6.5f,-71.7f), new Vector3(0f, 0f, 0)));
					MapTest.Add(new Block("Icorridor2_col", new Vector3(107.0792f,0.1244316f,-48.01477f), new Vector3(0f, 270f, 0)));
					MapTest.Add(new Block("Icorridor_col", new Vector3(106.2292f,0.1244316f,-96.12477f), new Vector3(0f, 270f, 0)));
					MapTest.Add(new Block("Icorridor3x6_col", new Vector3(147.16f,6.5f,-89.54f), new Vector3(0f, 0f, 0)));
                    MapTest.Add(new Block("windows3D_col", new Vector3(111.9092f, 8.5f, -89.37477f), new Vector3(0f, 180f, 0), "window_1")); MapTest.Add(new Block("FloorNeonAlea_col", new Vector3(83.1f,0.1244316f,-60.02477f), new Vector3(0f, 180f, 0)));
					MapTest.Add(new Block("FloorNeonAlea_col", new Vector3(130.9f,0.1244316f,-60.02477f), new Vector3(0f, 180f, 0)));
					MapTest.Add(new Block("Icorridor3x6_col", new Vector3(111.81f,6.5f,-103f), new Vector3(0f, 90f, 0)));
					MapTest.Add(new Block("Icorridor_col", new Vector3(59.72915f,0.1244316f,-59.92477f), new Vector3(0f, 180f, 0)));
					MapTest.Add(new Block("Icorridor3x6_col", new Vector3(105.71f,6.5f,-103.8f), new Vector3(0f, 90f, 0)));
					MapTest.Add(new Block("Icorridor3x6_col", new Vector3(99.5f,6.5f,-90f), new Vector3(0f, 90f, 0)));
					MapTest.Add(new Block("Icorridor3x6_col", new Vector3(76.8f,6.5f,-60f), new Vector3(0f, 180f, 0)));
					MapTest.Add(new Block("windows3D_col", new Vector3(147.4f,8.923815f,-77.5f), new Vector3(0f, 270f, 0), "window_2"));
					MapTest.Add(new Block("Icorridor_col", new Vector3(153.9f,0.1244316f,-83.2f), new Vector3(0f, 180f, 0)));
					MapTest.Add(new Block("Icorridor3x6_col", new Vector3(77.35f,6.5f,-42f), new Vector3(0f, 180f, 0)));
					MapTest.Add(new Block("Icorridor3x6_col", new Vector3(118.1f,6.5f,-90f), new Vector3(0f, 90f, 0)));
					MapTest.Add(new Block("Icorridor3x6_col", new Vector3(152.91f,6.5f,-77.2f), new Vector3(0f, 0f, 0)));
					MapTest.Add(new Block("Icorridor3x6_col", new Vector3(152.91f,6.5f,-83.02f), new Vector3(0f, 0f, 0)));
					MapTest.Add(new Block("Button", new Vector3(113.0092f,10.11381f,-101.2148f), new Vector3(0f, 180f, 0), "button_1"));
					MapTest.Add(new Block("Button", new Vector3(146.5f,5.550602f,-37.2305f), new Vector3(0f, 270f, 0), "button_2"));
					MapTest.Add(new Block("Button", new Vector3(79.2f,9.683808f,-43.08684f), new Vector3(0f, 270f, 0), "button_3"));
					MapTest.Add(new Block("Button", new Vector3(151.2691f,10.02381f,-76.16476f), new Vector3(0f, 270f, 0), "button_4"));
					MapTest.Add(new Block("Button", new Vector3(109.9093f,3.955154f,-19.51477f), new Vector3(0f, 180f, 0), "button_5"));
                    MapTest.Add(new Block("Button", new Vector3(76.53f, 2.91f, -57.12f), new Vector3(0f, 90f, 0), "button_6"));
                    MapTest.Add(new Block("Icorridor2_col", new Vector3(142.1392f,0.1244316f,-60.02477f), new Vector3(0f, 90f, 0)));
					MapTest.Add(new Block("Door3D_col", new Vector3(146.7791f,2.87381f,-53.90477f), new Vector3(0f, 270f, 0), "door_2"));
					MapTest.Add(new Block("Icorridor_col", new Vector3(154.6692f,0.1244316f,-59.79901f), new Vector3(0f, 180f, 0)));
					
                    MapConstructor test1 = new MapConstructor(MapTest);

                    Map map1 = new Map(test1, name, MapType.TEST, new Vector3(2,2,-6));
                    map1.AddElement(new string[] { "button_3", "button_5", "button_2" }, "window_1", ExecType.WINDOW, false);
					map1.AddElement(new string[] { "button_1" }, "window_2", ExecType.WINDOW, false);
					map1.AddElement(new string[] { "button_4" }, "door_2", ExecType.DOOR, false);
                    map1.AddElement(new string[] { "button_6" }, "door_3", ExecType.DOOR, false);
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
                    Map.Add(new Block("plat/corridoirscile", new Vector3(-74, -84.3f, -8), 0));
                    MapConstructor test2 = new MapConstructor(Map);
					test2.AddObject("Subtitles/NarrationSystem");
					test2.AddObject("Subtitles/Subtitle_gun");
					test2.AddObject("Subtitles/Subtitle_button");
					test2.AddObject("Subtitles/Subtitle_introduction");
					test2.AddObject("Subtitles/NarrationSystem");
					
                    Map plat1 = new Map(test2, name, MapType.TEST, new Vector3(0, 2.33f, -6.2f));
					plat1.AddElement(new string[] { "button_1" }, "door_1", ExecType.DOOR, false);
					return plat1;
                    break;


                case "mult":
                    List<Block> mult = new List<Block>();
                    mult.Add(new Block("Button", new Vector3(-30.97742f, 9.839996f, 63.16f), new Vector3(0f, 0f, 0), "button_2"));
                    mult.Add(new Block("Button", new Vector3(-49.308f, 16.88f, 7.010864f), new Vector3(90f, 0f, 0), "button_4"));
                    mult.Add(new Block("Button", new Vector3(-63.28f, 21.37077f, -33.07411f), new Vector3(0f, 270f, 0), "button_3"));
                    mult.Add(new Block("Button", new Vector3(-24.53563f, 12.91f, -35.85709f), new Vector3(90f, 0f, 0), "button_1"));
                    mult.Add(new Block("Plateforme_col", new Vector3(-24.38943f, -3.800003f, 27.94241f), new Vector3(0f, 0f, 0)));
                    mult.Add(new Block("Mur", new Vector3(5.152161f, 17.9935f, 18f), new Vector3(0f, 270f, 0)));
                    mult.Add(new Block("Mur", new Vector3(-6f, 18.51711f, 24.1f), new Vector3(0f, 180f, 0)));
                    mult.Add(new Block("Mur", new Vector3(-6.186554f, 18.51711f, 11.55272f), new Vector3(0f, 0f, 0)));
                    mult.Add(new Block("Mur", new Vector3(-17.8f, 18.51711f, 40.6f), new Vector3(0f, 270f, 0)));
                    mult.Add(new Block("Mur", new Vector3(-53.10001f, 18.51711f, 52.61f), new Vector3(0f, 180f, 0)));
                    mult.Add(new Block("Mur", new Vector3(-65.3f, 18.35001f, -5.979996f), new Vector3(0f, 90f, 0)));
                    mult.Add(new Block("Icorridor2_col", new Vector3(-36.10001f, 0f, 11.10001f), new Vector3(0f, 180f, 0)));
                    mult.Add(new Block("Lcorridor2_col", new Vector3(-60f, 0f, 23f), new Vector3(0f, 270f, 0)));
                    mult.Add(new Block("Icorridor3x6_col", new Vector3(-23.98f, 7.309998f, 64.56f), new Vector3(0f, 270f, 0)));
                    mult.Add(new Block("Icorridor3x6_col", new Vector3(-30f, 7.139999f, 65.11f), new Vector3(0f, 270f, 0)));
                    mult.Add(new Block("Icorridor_col", new Vector3(-24.8f, 0.5f, 59.5f), new Vector3(0f, 90f, 0)));
                    mult.Add(new Block("Icorridor3x6_col", new Vector3(-18.75999f, 6.490005f, 52.65f), new Vector3(0f, 270f, 0)));
                    mult.Add(new Block("windows3D_col", new Vector3(-30.59f, 8.529999f, 52.93f), new Vector3(0f, 0f, 0),"window_1"));
                    mult.Add(new Block("Mur", new Vector3(-19.00999f, 18.55f, -30.72f), new Vector3(0f, 270f, 0)));
                    mult.Add(new Block("Door3D_col", new Vector3(-41f, 14.21001f, -30.11f), new Vector3(0f, 90f, 0), "door_1"));
                    mult.Add(new Block("Icorridor3x6_col", new Vector3(-42.45999f, 18.24001f, -41.5f), new Vector3(0f, 0f, 0)));
                    mult.Add(new Block("Icorridor3x6_col", new Vector3(-42.60001f, 18.22f, -24.63f), new Vector3(0f, 0f, 0)));
                    mult.Add(new Block("Mur", new Vector3(-53.49001f, 18.19f, -20.8f), new Vector3(0f, 180f, 0)));
                    mult.Add(new Block("Lcorridor2_col", new Vector3(-23.92f, 0f, -12f), new Vector3(0f, 90f, 0)));
                    mult.Add(new Block("Plateforme_col", new Vector3(-27.63103f, -4.300003f, -19.16389f), new Vector3(0f, 0f, 0)));
                    mult.Add(new Block("Icorridor2_col", new Vector3(-48f, 0f, 47.9f), new Vector3(0f, 0f, 0)));
                    mult.Add(new Block("FloorNeonAlea_col", new Vector3(-24.03f, 0f, 0.08171082f), new Vector3(0f, 0f, 0)));
                    mult.Add(new Block("Icorridor3x6_col", new Vector3(-18.02f, 6.399994f, 5.869995f), new Vector3(0f, 0f, 0)));
                    mult.Add(new Block("Door3D_col", new Vector3(5.059998f, 2.410004f, -6.039993f), new Vector3(0f, 90f, 0)));
                    mult.Add(new Block("Icorridor_col", new Vector3(0f, 0f, 23.91555f), new Vector3(0f, 0f, 0)));
                    mult.Add(new Block("Icorridor_col", new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0)));
                    mult.Add(new Block("Door3D_col", new Vector3(5.059998f, 2.410004f, 18.2f), new Vector3(0f, 90f, 0)));
                    mult.Add(new Block("Icorridor3x6_col", new Vector3(-18.02f, 6.399994f, -0.07000732f), new Vector3(0f, 0f, 0)));
                    mult.Add(new Block("Icorridor3x6_col", new Vector3(-18.02f, 6.399994f, -18.31f), new Vector3(0f, 0f, 0)));
                    mult.Add(new Block("Icorridor3x6_col", new Vector3(-18.02f, 6.399994f, 24f), new Vector3(0f, 0f, 0)));
                    mult.Add(new Block("FloorNeonAlea_col", new Vector3(-24.03f, 0f, 23.9f), new Vector3(0f, 0f, 0)));
                    mult.Add(new Block("Plateforme_col", new Vector3(-34.2f, 0.7700043f, -29.10001f), new Vector3(0f, 0f, 0)));
                    mult.Add(new Block("Plateforme_col", new Vector3(-24.60001f, 4.5f, -34.2f), new Vector3(0f, 0f, 0)));
                    mult.Add(new Block("Lcorridor2_col", new Vector3(-35f, 12f, -35.89999f), new Vector3(0f, 180f, 0)));
                    mult.Add(new Block("Icorridor3x6_col", new Vector3(-40.7f, 18.22f, -18.47f), new Vector3(0f, 180f, 0)));
                    mult.Add(new Block("Icorridor3x6_col", new Vector3(-40.54001f, 18.24001f, -35.84f), new Vector3(0f, 180f, 0)));
                    mult.Add(new Block("Mur", new Vector3(-53.49001f, 18.19f, -18.7f), new Vector3(0f, 0f, 0)));
                    mult.Add(new Block("Icorridor3x6_col", new Vector3(-18.02f, 18.39999f, -18.31f), new Vector3(0f, 0f, 0)));
                    mult.Add(new Block("Mur", new Vector3(-30.50999f, 18.55f, -42.11f), new Vector3(0f, 0f, 0)));
                    mult.Add(new Block("Icorridor3x6_col", new Vector3(-36.31f, 6.490005f, 52.65f), new Vector3(0f, 270f, 0)));
                    mult.Add(new Block("Icorridor2_col", new Vector3(-36.10001f, 0f, -12.2f), new Vector3(0f, 180f, 0)));
                    mult.Add(new Block("Mur", new Vector3(-65.3f, 18.35001f, 18.02f), new Vector3(0f, 90f, 0)));
                    mult.Add(new Block("Mur", new Vector3(-65.3f, 18.35001f, 42.02f), new Vector3(0f, 90f, 0)));
                    mult.Add(new Block("Mur", new Vector3(-29f, 18.51711f, 52.61f), new Vector3(0f, 180f, 0)));
                    mult.Add(new Block("Icorridor3x6_col", new Vector3(-18.02f, 18.39999f, 24f), new Vector3(0f, 0f, 0)));
                    mult.Add(new Block("Icorridor3x6_col", new Vector3(-18.02f, 18.39999f, -0.07000732f), new Vector3(0f, 0f, 0)));
                    mult.Add(new Block("Icorridor3x6_col", new Vector3(-18.02f, 18.39999f, 5.869995f), new Vector3(0f, 0f, 0)));
                    mult.Add(new Block("Mur", new Vector3(-6f, 18.51711f, 0.3000031f), new Vector3(0f, 180f, 0)));
                    mult.Add(new Block("Mur", new Vector3(-6.186554f, 18.51711f, -12.2f), new Vector3(0f, 0f, 0)));
                    mult.Add(new Block("Mur", new Vector3(5.152161f, 17.9935f, -6.039993f), new Vector3(0f, 270f, 0)));
                    mult.Add(new Block("Plateforme_col", new Vector3(-39.39999f, 0.6000061f, 36.3f), new Vector3(0f, 0f, 0)));
                    mult.Add(new Block("Plateforme_col", new Vector3(-54.5f, 8f, 27.2f), new Vector3(0f, 0f, 0)));
                    mult.Add(new Block("Plateforme_col", new Vector3(-52.89999f, 5.5f, 45.1f), new Vector3(0f, 0f, 0)));
                    mult.Add(new Block("Plateforme_col", new Vector3(-49.3f, 8.600006f, 8.600006f), new Vector3(0f, 0f, 0)));
                    mult.Add(new Block("Icorridor2_col", new Vector3(-35.75999f, 0f, -35.69f), new Vector3(0f, 180f, 0)));
                    mult.Add(new Block("Icorridor3x6_col", new Vector3(-53.5f, 6.589996f, -64.01001f), new Vector3(0f, 90f, 0)));
                    mult.Add(new Block("Icorridor_col", new Vector3(-46.89999f, 0f, -60f), new Vector3(0f, 90f, 0)));
                    mult.Add(new Block("Icorridor3x6_col", new Vector3(-65.14f, 5.630005f, -41.2f), new Vector3(0f, 90f, 0)));
                    mult.Add(new Block("Door3D_col", new Vector3(-52.83856f, 2.5f, -41.38f), new Vector3(0f, 0f, 0), "door_2"));
                    mult.Add(new Block("Icorridor3x6_col", new Vector3(-47.39f, 5.630005f, -41.14999f), new Vector3(0f, 90f, 0)));
                    mult.Add(new Block("Icorridor3x6_col", new Vector3(-59.28f, 6.589996f, -64.01001f), new Vector3(0f, 90f, 0)));


                    MapConstructor mc_level2players = new MapConstructor(mult);
                    Map level2players = new Map(mc_level2players, name, MapType.OTHER, new List<Vector3>(new Vector3[]{new Vector3(2.2f, 2.33f, -6.705f), new Vector3(2.2f, 2.33f, 18.5f)}));
                    level2players.AddElement(new string[] { "button_1"}, "window_1", ExecType.WINDOW, false);
                    level2players.AddElement(new string[] { "button_2" }, "door_1", ExecType.DOOR, false);
                    level2players.AddElement(new string[] { "button_3", "button_4" }, "door_2", ExecType.DOOR, false);
                    return level2players;
				
				case "Mapmulti":
					List<Block> bl_Mapmulti = new List<Block>();
					bl_Mapmulti.Add(new Block("plat/Lcorridor_plat_col", new Vector3(28.14f,0f,23.98f), new Vector3(0f, 180f, 0)));
					bl_Mapmulti.Add(new Block("plat/Icorridor_plat_col", new Vector3(-7.5f,0f,0f), new Vector3(0f, 90f, 0)));
					bl_Mapmulti.Add(new Block("plat/Icorridor_plat_col", new Vector3(-7.5f,0f,24f), new Vector3(0f, 90f, 0)));
					bl_Mapmulti.Add(new Block("plat/Tcorridor_plat_col", new Vector3(-19f,0f,36f), new Vector3(0f, 270f, 0)));
					bl_Mapmulti.Add(new Block("plat/Icorridor_plat_col", new Vector3(-7f,0f,96f), new Vector3(0f, 90f, 0)));
					bl_Mapmulti.Add(new Block("plat/Icorridor_plat_col", new Vector3(-7f,0f,72f), new Vector3(0f, 90f, 0)));
					bl_Mapmulti.Add(new Block("plat/Lcorridor_plat_col", new Vector3(40.1f,0f,47.9f), new Vector3(0f, 90f, 0)));
					bl_Mapmulti.Add(new Block("plat/Lcorridor_plat_col", new Vector3(-7f,0f,59.6f), new Vector3(0f, 0f, 0)));
					bl_Mapmulti.Add(new Block("plat/Lcorridor_plat_col", new Vector3(-18.9f,0f,107.8f), new Vector3(0f, 270f, 0)));
					bl_Mapmulti.Add(new Block("plat/Icorridor_plat_col", new Vector3(62.2f,0f,47.7f), new Vector3(0f, 90f, 0)));
					bl_Mapmulti.Add(new Block("plat/Icorridor_plat_col", new Vector3(39.8f,0f,47.7f), new Vector3(0f, 90f, 0)));
					bl_Mapmulti.Add(new Block("plat/Lcorridor_plat_col", new Vector3(40.9f,0f,143f), new Vector3(0f, 90f, 0)));
					bl_Mapmulti.Add(new Block("plat/Icorridor_and_pik-plateforme", new Vector3(17.1f,0f,131.3f), new Vector3(0f, 0f, 0)));
					bl_Mapmulti.Add(new Block("plat/Icorridor_plat_col", new Vector3(40.8f,0f,142.9f), new Vector3(0f, 90f, 0)));
					bl_Mapmulti.Add(new Block("plat/Mur_col", new Vector3(34.43f,4.34f,160.62f), new Vector3(0f, 180f, 0)));
					bl_Mapmulti.Add(new Block("plat/Mur_col", new Vector3(35.05f,16.4368f,182.4f), new Vector3(0f, 180f, 0)));
					bl_Mapmulti.Add(new Block("plat/Lcorridor_plat_col", new Vector3(38f,0f,106f), new Vector3(0f, 0f, 0)));
					bl_Mapmulti.Add(new Block("plat/Lcorridor_plat_col", new Vector3(28.7f,0f,82.5f), new Vector3(0f, 270f, 0)));
					bl_Mapmulti.Add(new Block("plat/Icorridor_plat_col", new Vector3(62.2f,0f,70.9f), new Vector3(0f, 90f, 0)));
					bl_Mapmulti.Add(new Block("plat/Scile_col", new Vector3(-17.38001f,-2f,89f), new Vector3(0f, 270f, 0)));
					bl_Mapmulti.Add(new Block("plat/Door3D_plat_col", new Vector3(56.11f,2.26f,84.84608f), new Vector3(0f, 0f, 0), "door_1"));
					bl_Mapmulti.Add(new Block("plat/Icorridor_and_pik-plateforme", new Vector3(40.1f,0f,71.7f), new Vector3(0f, 90f, 0)));
					bl_Mapmulti.Add(new Block("plat/Door3D_plat_col", new Vector3(3.420918f,2.4f,53.94f), new Vector3(0f, 90f, 0), "door_2"));
					bl_Mapmulti.Add(new Block("plat/Icorridor_and_pik-plateforme", new Vector3(83.7f,0f,71.7f), new Vector3(0f, 90f, 0)));
					bl_Mapmulti.Add(new Block("plat/Icorridor_plat_col", new Vector3(83.4f,0f,47.7f), new Vector3(0f, 90f, 0)));
					bl_Mapmulti.Add(new Block("plat/Lcorridor_plat_col", new Vector3(83.7f,0f,47.9f), new Vector3(0f, 90f, 0)));
					bl_Mapmulti.Add(new Block("plat/Door3D_plat_col", new Vector3(65.62286f,2.3f,30.46f), new Vector3(0f, 90f, 0), "door_3"));
					bl_Mapmulti.Add(new Block("plat/Icorridor_plat_col", new Vector3(40.9f,10f,166.6f), new Vector3(0f, 90f, 0)));
					bl_Mapmulti.Add(new Block("plat/Lcorridor_plat_col", new Vector3(73.9f,0f,24.5f), new Vector3(0f, 180f, 0)));
					bl_Mapmulti.Add(new Block("plat/Icorridor_plat_col", new Vector3(83.11f,0f,143.8f), new Vector3(0f, 90f, 0)));
					bl_Mapmulti.Add(new Block("plat/Mur_col", new Vector3(76.41833f,6.39f,161.016f), new Vector3(0f, 180f, 0)));
					bl_Mapmulti.Add(new Block("plat/Icorridor_plat_col", new Vector3(83.15989f,-0.161499f,95.89633f), new Vector3(0f, 90f, 0)));
					bl_Mapmulti.Add(new Block("plat/Mur_col", new Vector3(-13.86f,5.68f,-15.3f), new Vector3(0f, 0f, 0)));
					bl_Mapmulti.Add(new Block("plat/Icorridor_plat_col", new Vector3(-7.5f,0f,-12f), new Vector3(0f, 90f, 0)));
					bl_Mapmulti.Add(new Block("plat/trap_pilier_col", new Vector3(33.52055f,-1.97f,144.27f), new Vector3(0f, 0f, 0)));
					bl_Mapmulti.Add(new Block("plat/trap_pilier_col", new Vector3(78.1f,4.120001f,50.3f), new Vector3(0f, 0f, 0)));
					bl_Mapmulti.Add(new Block("plat/Scile_col", new Vector3(73.2f,-2f,49.2f), new Vector3(0f, 270f, 0)));
					bl_Mapmulti.Add(new Block("plat/Scile_col", new Vector3(82f,-2.5f,54.2f), new Vector3(0f, 90f, 0)));
					bl_Mapmulti.Add(new Block("plat/trap_pilier_col", new Vector3(35.6f,0f,59.5017f), new Vector3(0f, 0f, 0)));
					bl_Mapmulti.Add(new Block("plat/Scile_col", new Vector3(29.4f,-3.5f,54.1f), new Vector3(0f, 270f, 0)));
					bl_Mapmulti.Add(new Block("plat/trap_pilier_col", new Vector3(58.26322f,0f,59.5017f), new Vector3(0f, 0f, 0)));
					bl_Mapmulti.Add(new Block("Button", new Vector3(51.2f,5.496685f,60.57917f), new Vector3(0f, 90f, 0), "button_1"));
					bl_Mapmulti.Add(new Block("plat/Scile_col", new Vector3(52.05f,-3.5f,54.1f), new Vector3(0f, 270f, 0)));
					bl_Mapmulti.Add(new Block("Button", new Vector3(34.01f,13.49f,181.4f), new Vector3(0f, 0f, 0), "button_2"));
					bl_Mapmulti.Add(new Block("plat/trap_pilier_col", new Vector3(36.60698f,0.9f,149.86f), new Vector3(0f, 0f, 0)));
					bl_Mapmulti.Add(new Block("plat/trap_pilier_col", new Vector3(33.52055f,3.332898f,156.2393f), new Vector3(0f, 0f, 0)));
					bl_Mapmulti.Add(new Block("plat/trap_pilier_col", new Vector3(-12.50207f,4.120001f,90.14619f), new Vector3(0f, 0f, 0)));
					bl_Mapmulti.Add(new Block("plat/Scile_col", new Vector3(-8.6f,-2.5f,94f), new Vector3(0f, 90f, 0)));
					bl_Mapmulti.Add(new Block("plat/Scile_col", new Vector3(72.85621f,-3.202118f,108.5214f), new Vector3(0f, 270f, 0)));
					bl_Mapmulti.Add(new Block("plat/Icorridor_plat_col", new Vector3(83.15989f,-0.161499f,119.9214f), new Vector3(0f, 90f, 0)));
					bl_Mapmulti.Add(new Block("plat/Scile_col", new Vector3(70.25622f,1.157883f,113.2214f), new Vector3(90f, 90f, 0)));
					bl_Mapmulti.Add(new Block("plat/Scile_col", new Vector3(81.52622f,-3.252121f,113.2214f), new Vector3(0f, 90f, 0)));
					bl_Mapmulti.Add(new Block("Button", new Vector3(52.5f, 2.6f, 85.3f), new Vector3(0f, 0f, 0), "button_3"));
					MapConstructor mc_Mapmulti = new MapConstructor(bl_Mapmulti);
					Map Mapmulti = new Map(mc_Mapmulti, name, MapType.OTHER, new List<Vector3>(new Vector3[]{new Vector3(77.07f, 2.0f, 150.0f), new Vector3(-13.4f, 2.0f, -7.6f)}));
					Mapmulti.AddElement(new string[] { "button_1"}, "door_2", ExecType.DOOR, false);
                    Mapmulti.AddElement(new string[] { "button_2" }, "door_3", ExecType.DOOR, false);
                    Mapmulti.AddElement(new string[] { "button_3" }, "door_1", ExecType.DOOR, false);
					return Mapmulti;
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
