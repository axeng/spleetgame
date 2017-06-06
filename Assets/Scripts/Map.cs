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
		public static List<string> mapsName = 
			new List<string>() { 
					"mctest",
					"map1", 
					"plat1", 
					"poulet", 
					"mult", 
					"Mapmulti", 
					"TestPressionPlate", 
					"Stp3",
					"Stp2", 
					"Stp1", 
					"Stp0", 
					"enigme"
				};

		public static Dictionary<string, Map> dicoMaps;

		private List<ToAddElement> toAddElement;
    
		//level name
    	private string name;
		private MapType type;

		private List<Checkpoint> checkpoints;
		private List<Executable> executables;

		private List<Executor> buttons;
		private List<Executor> pressionPlates;

		private List<Vector3> spawnPoint;

		private List<MapConstructor> constructors;

		private List<string> hintsList;

		public Map(List<MapConstructor> constructors, string name, MapType type, Vector3 spawnPoint) : this(constructors, name, type, new List<Vector3>(new Vector3[] {spawnPoint})){}
        public Map(List<MapConstructor> constructors, string name, MapType type, List<Vector3> spawnPoint) : this(name, type, spawnPoint)
        {
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

			this.buttons = new List<Executor>();
			this.pressionPlates = new List<Executor>();

			this.spawnPoint = spawnPoint;

			this.constructors = new List<MapConstructor>();
			this.hintsList = new List<string>();

			this.toAddElement = new List<ToAddElement>();
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

		public List<Executor> GetButtons()
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
			AddElement(buts, element, type, ExecutorType.BUTTON, tag);
		}
		public void AddElement(string[] buts, string element, ExecType type, ExecutorType executorType, bool tag = true)
		{
			toAddElement.Add(new ToAddElement(buts, element, type, executorType, tag));
		}
		public void OldAddElement(ToAddElement e)
		{
			string[] buts = e.buts;
			string element = e.element;
			ExecType type = e.type;
			ExecutorType executorType = e.executorType;
			bool tag = e.tag;
			
			List<Executor> executorsElement = new List<Executor>();

			foreach(string but in buts)
			{
				if (tag)
				{
					if (executorType == ExecutorType.BUTTON)
						executorsElement.Add(new Button(null, GameObject.FindWithTag(but + "_true"), GameObject.FindWithTag(but + "_false"), tag));
					else if (executorType == ExecutorType.PRESSIONPLATE)
						executorsElement.Add(new PressionPlate(null, GameObject.FindWithTag(but + "_true"), GameObject.FindWithTag(but + "_false"), tag));
				}
				else
				{
					if (executorType == ExecutorType.BUTTON)
						executorsElement.Add(new Button(null, GameObject.Find(but + "_true"), GameObject.Find(but + "_false"), tag));
					else if (executorType == ExecutorType.PRESSIONPLATE)
						executorsElement.Add(new PressionPlate(null, GameObject.Find(but + "_true"), GameObject.Find(but + "_false"), tag));
				}
			}

			switch(type)
			{
				case ExecType.DOOR:
					if (tag)
						this.AddExec(new Door(null, GameObject.FindWithTag(element), GameObject.FindWithTag(element + "_left"), GameObject.FindWithTag(element + "_right")), executorsElement);
					else
                    	this.AddExec(new Door(null, GameObject.Find(element), GameObject.Find(element + "_left"), GameObject.Find(element + "_right")), executorsElement);
					break;

				case ExecType.WINDOW:
					if (tag)
						this.AddExec(new Window(null, GameObject.FindWithTag(element)), executorsElement);
					else
						this.AddExec(new Window(null, GameObject.Find(element)), executorsElement);
					break;
			}
		}

		//add an executable in the map
		public void AddExec(Executable exec, List<Executor> executors)
		{
			foreach (Executor b in executors)
			{
				b.SetExecutable(exec);
			}
			
			exec.SetExecutors(executors);

			this.executables.Add(exec);
			this.buttons.AddRange(executors.FindAll(p => p.type == ExecutorType.BUTTON));
			this.pressionPlates.AddRange(executors.FindAll(p => p.type == ExecutorType.PRESSIONPLATE));
		}

		//call each time a laser hit an object
		public void Shoot(GameObject gameObject)
		{
			List<Executor> b = new List<Executor>();
			b.AddRange(buttons.FindAll(p => p.IsThisGameObject(gameObject)));

			foreach(Executor e in b)
			{
				e.Push();
				e.Exec();
			}
		}
		
		public void WalkOn(GameObject gameObject)
		{
			List<Executor> b = new List<Executor>();
			b.AddRange(pressionPlates.FindAll(p => p.IsThisGameObject(gameObject)));

			foreach(Executor e in b)
			{
				if (!e.IsActivate())
				{
					e.Push();
					e.Exec();
				}
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
		
		public bool PopOneHint()
		{
			if (hintsList.Count <= 0)
			{
				Game.game.PopupMessage("You don't have hints available", 1);
				return false;
			}

			if (Game.game.isPopup)
				return false;
			Game.game.PopupMessage(hintsList[0], 2);
			hintsList.RemoveAt(0);
			return true;
		}

		public void Construct()
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
						constructor.blocks.Add(new Block("Player", spawnPoint[0], 0));
					}
				}
	
				constructor.Construct();
				i++;
			}

			foreach (ToAddElement e in toAddElement)
				OldAddElement(e);
		}

		public static Map GetMap(string name)
		{
			if (dicoMaps.ContainsKey(name))
				return dicoMaps[name];
			else
				return null;
		}

		public static Map OldGetMap(string name)
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
                    MapTest.Add(new Block(BlockType.Door3D, new Vector3(77, 2.25f, -54), 90, "door_3"));
                    MapTest.Add(new Block(BlockType.Lcorridor, new Vector3(60, 0, -60), 180));		
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
                    MapTest.Add(new Block("windows3D_col", new Vector3(111.9092f, 8.5f, -89.37477f), new Vector3(0f, 180f, 0), "window_1"));
                    MapTest.Add(new Block("FloorNeonAlea_col", new Vector3(83.1f,0.1244316f,-60.02477f), new Vector3(0f, 180f, 0)));
					MapTest.Add(new Block("FloorNeonAlea_col", new Vector3(130.9f,0.1244316f,-60.02477f), new Vector3(0f, 180f, 0)));
					MapTest.Add(new Block("Icorridor3x6_col", new Vector3(111.81f,6.5f,-103f), new Vector3(0f, 90f, 0)));
					MapTest.Add(new Block("Icorridor_col", new Vector3(59.72915f,0.1244316f,-59.92477f), new Vector3(0f, 180f, 0)));
                    MapTest.Add(new Block("Mur", new Vector3(171.63f, 6.1f, -53.8f), new Vector3(0f, 90f, 0)));
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
					MapTest.Add(new Block("Door3D_col", new Vector3(146.7791f,2.25f,-53.90477f), new Vector3(0f, 270f, 0), "door_2"));
					MapTest.Add(new Block("Icorridor_col", new Vector3(154.6692f,0.1244316f,-59.79901f), new Vector3(0f, 180f, 0)));
                    MapTest.Add(new Block("Teleportation portal", new Vector3(162.54f, 0.6f, -54.18f), new Vector3(0f, 0f, 0), "tp_Stp%l"));
                    MapConstructor test1 = new MapConstructor(MapTest);

                    Map map1 = new Map(test1, name, MapType.TEST, new Vector3(2,2,-6));
                    map1.AddElement(new string[] { "button_3", "button_5", "button_2" }, "window_1", ExecType.WINDOW, false);
					map1.AddElement(new string[] { "button_1" }, "window_2", ExecType.WINDOW, false);
					map1.AddElement(new string[] { "button_4" }, "door_2", ExecType.DOOR, false);
                    map1.AddElement(new string[] { "button_6" }, "door_3", ExecType.DOOR, false);
                    map1.hintsList.Add("Tirer sur un bouton l'active.");
                    return map1;
                    break;

                case "plat1":
                    List<Block> Map = new List<Block>();
                    Map.Add(new Block("Button", new Vector3(-146, 15, -7), 90, "button_1"));
                    Map.Add(new Block("plat/Mur_col", new Vector3(-134, 6, -6), 90));
                    Map.Add(new Block("plat/Mur_col", new Vector3(-127, 18, -12), 0));
                    Map.Add(new Block("plat/Mur_col", new Vector3(-129, 19, 0), 180));
                    Map.Add(new Block("plat/Mur_col", new Vector3(-147, 18, -6), 90));
                    Map.Add(new Block("plat/Mur_col", new Vector3(-31.04f, 6.24f, -50f),0));
                    Map.Add(new Block("plat/Plateforme_col", new Vector3(-130, 6, -7), 0));
                    Map.Add(new Block("plat/trap_pilier_col", new Vector3(-98, -2, -7), 0));
                    Map.Add(new Block("plat/trap_pilier_col", new Vector3(-107, 0, -4), 0));
                    Map.Add(new Block("plat/trap_pilier_col", new Vector3(-121, 3, -4), 0));
                    Map.Add(new Block("plat/Icorridor_plat_col", new Vector3(-94, 0, 0), 0));
                    Map.Add(new Block("plat/Icorridor_plat_col", new Vector3(-25, 0, -36), 90));
                    Map.Add(new Block("plat/Icorridor_plat_col", new Vector3(-139, 12, 0), 0));
                    Map.Add(new Block("plat/Icorridor_plat_col", new Vector3(0, 0, 0), 0));
                    Map.Add(new Block("plat/Icorridor_plat_col", new Vector3(-117, 0, 0), 0));
                    Map.Add(new Block("plat/Icorridor_plat_col", new Vector3(-24.09f, 0f, -49.6f), 90));
                    Map.Add(new Block("plat/Tcorridor_plat_col", new Vector3(-48, 0, 0), 0));
                    Map.Add(new Block("plat/Door3D_plat_col", new Vector3(4, 2, -6), 90));
                    Map.Add(new Block("plat/Door3D_plat_col", new Vector3(-30.25f, 2, -41), 0, "door_1"));
                    Map.Add(new Block("plat/corridoirscile", new Vector3(-74, -84.3f, -8), 0));
                    Map.Add(new Block("Teleportation portal", new Vector3(-30.08f, 0.5f, -45.12f), new Vector3(0.0f, 0.0f, 0.0f), "tp_Stp%l"));
                    MapConstructor test2 = new MapConstructor(Map);
					test2.AddObject("Subtitles/NarrationSystem");
					test2.AddObject("Subtitles/Subtitle_gun");
					test2.AddObject("Subtitles/Subtitle_button");
					test2.AddObject("Subtitles/Subtitle_introduction");
					test2.AddObject("Subtitles/NarrationSystem");
					
                    Map plat1 = new Map(test2, name, MapType.TEST, new Vector3(0, 2.33f, -6.2f));
					plat1.AddElement(new string[] { "button_1" }, "door_1", ExecType.DOOR, false);
					//plat1.hintsList.Add("");
					return plat1;
                    break;

                case "poulet":
                    List<Block> bl_poulet = new List<Block>();
                    bl_poulet.Add(new Block("plat/Scile_col", new Vector3(21.13661f, 26.3f, -13.1f), new Vector3(90f, 0f, 0)));
                    bl_poulet.Add(new Block("plat/Mur_col", new Vector3(-16.7f, 31.53f, -6f), new Vector3(0f, 90f, 0)));
                    bl_poulet.Add(new Block("plat/Icorridor_plat_col", new Vector3(0f, 25f, 0f), new Vector3(0f, 0f, 0)));
                    bl_poulet.Add(new Block("plat/Icorridor_plat_col", new Vector3(24f, 25f, 0f), new Vector3(0f, 0f, 0)));
                    bl_poulet.Add(new Block("plat/Icorridor_plat_col", new Vector3(143.6f, 25f, 0f), new Vector3(0f, 0f, 0)));
                    bl_poulet.Add(new Block("plat/Icorridor_plat_col", new Vector3(167.6f, 25f, 0f), new Vector3(0f, 0f, 0)));
                    bl_poulet.Add(new Block("plat/Icorridor_plat_col", new Vector3(119.7f, 25f, 0f), new Vector3(0f, 0f, 0)));
                    bl_poulet.Add(new Block("plat/Scile_col", new Vector3(25.85f, 26.4f, -2.8f), new Vector3(90f, 180f, 0)));
                    bl_poulet.Add(new Block("plat/Scile_col", new Vector3(21.13661f, 36.3f, -10f), new Vector3(0f, 180f, 180)));
                    bl_poulet.Add(new Block("plat/Scile_col", new Vector3(25.85f, 26.4f, 2.8f), new Vector3(90f, 180f, 0)));
                    bl_poulet.Add(new Block("plat/Scile_col", new Vector3(23.3f, 21.2f, -2f), new Vector3(0f, 0f, 0)));
                    bl_poulet.Add(new Block("plat/trap_pilier_col", new Vector3(98.48444f, 27.5f, -10.72713f), new Vector3(0f, 0f, 0)));
                    bl_poulet.Add(new Block("plat/trap_pilier_col", new Vector3(98.84058f, 27.61481f, -1.3f), new Vector3(0f, 0f, 0)));
                    bl_poulet.Add(new Block("plat/trap_pilier_col", new Vector3(78.68836f, 27.5f, -8.712777f), new Vector3(0f, 0f, 0)));
                    bl_poulet.Add(new Block("plat/trap_pilier_col", new Vector3(52.88676f, 28.55f, -5.586227f), new Vector3(0f, 0f, 0)));
                    bl_poulet.Add(new Block("plat/Icorridor_and_pik-plateforme", new Vector3(48.1f, 24.9f, -0.3000002f), new Vector3(0f, 0f, 0)));
                    bl_poulet.Add(new Block("plat/Icorridor_and_pik-plateforme", new Vector3(72.05f, 24.9f, -0.3000002f), new Vector3(0f, 0f, 0)));
                    bl_poulet.Add(new Block("plat/Icorridor_and_pik-plateforme", new Vector3(95.9f, 24.9f, -0.3000002f), new Vector3(0f, 0f, 0)));
                    bl_poulet.Add(new Block("plat/trap_pilier_col", new Vector3(136.7386f, 27.5f, -5.680916f), new Vector3(0f, 0f, 0)));
                    bl_poulet.Add(new Block("plat/trap_pilier_col", new Vector3(126.0856f, 27.5f, -2.626113f), new Vector3(0f, 0f, 0)));
                    bl_poulet.Add(new Block("plat/trap_pilier_col", new Vector3(116.3842f, 27.5f, -8.191012f), new Vector3(0f, 0f, 0)));
                    bl_poulet.Add(new Block("plat/trap_pilier_col", new Vector3(106.5616f, 27.5f, -6.080332f), new Vector3(0f, 0f, 0)));
                    bl_poulet.Add(new Block("plat/Icorridor_plat_col", new Vector3(191.7f, 25f, 0f), new Vector3(0f, 0f, 0)));
                    bl_poulet.Add(new Block("plat/Icorridor_plat_col", new Vector3(215.2f, 25f, 0f), new Vector3(0f, 0f, 0)));
                    bl_poulet.Add(new Block("plat/Icorridor_plat_col", new Vector3(262.5f, 25f, 0f), new Vector3(0f, 0f, 0)));
                    bl_poulet.Add(new Block("plat/Icorridor_plat_col", new Vector3(239f, 25f, 0f), new Vector3(0f, 0f, 0)));
                    bl_poulet.Add(new Block("plat/Scile_col", new Vector3(158.94f, 21.63f, -2.2f), new Vector3(0f, 0f, 0)));
                    bl_poulet.Add(new Block("plat/Scile_col", new Vector3(158.94f, 23.13f, -2.2f), new Vector3(0f, 0f, 0)));
                    bl_poulet.Add(new Block("plat/trap_pilier_col", new Vector3(161.14f, 28.5f, -7.8f), new Vector3(0f, 0f, 0)));
                    bl_poulet.Add(new Block("plat/Scile_col", new Vector3(212.9f, 23.13f, -2.2f), new Vector3(0f, 0f, 0)));
                    bl_poulet.Add(new Block("plat/Scile_col", new Vector3(212.9f, 21.63f, -2.2f), new Vector3(0f, 0f, 0)));
                    bl_poulet.Add(new Block("plat/Scile_col", new Vector3(212.9f, 20.13f, -2.2f), new Vector3(0f, 0f, 0)));
                    bl_poulet.Add(new Block("plat/Scile_col", new Vector3(215.17f, 20.13f, -9.75f), new Vector3(0f, 180f, 0)));
                    bl_poulet.Add(new Block("plat/Scile_col", new Vector3(215.17f, 21.7f, -9.75f), new Vector3(0f, 180f, 0)));
                    bl_poulet.Add(new Block("plat/Scile_col", new Vector3(215.17f, 23.25f, -9.75f), new Vector3(0f, 180f, 0)));
                    bl_poulet.Add(new Block("plat/trap_pilier_col", new Vector3(213.8f, 28.61f, -5.933362f), new Vector3(0f, 0f, 0)));
                    bl_poulet.Add(new Block("plat/trap_pilier_col", new Vector3(223.7f, 28.61f, -5.933362f), new Vector3(0f, 0f, 0)));
                    bl_poulet.Add(new Block("plat/Scile_col", new Vector3(246.6f, 26.59f, -14.5f), new Vector3(90f, 0f, 0)));
                    bl_poulet.Add(new Block("plat/Scile_col", new Vector3(246.6f, 26.7f, -10.5f), new Vector3(90f, 0f, 0)));
                    bl_poulet.Add(new Block("plat/trap_pilier_col", new Vector3(203.4226f, 28.61f, -5.933362f), new Vector3(0f, 0f, 0)));
                    bl_poulet.Add(new Block("plat/Scile_col", new Vector3(158.94f, 20.13f, -2.2f), new Vector3(0f, 0f, 0)));
                    bl_poulet.Add(new Block("plat/trap_pilier_col", new Vector3(150.54f, 28.51f, -7.9f), new Vector3(0f, 0f, 0)));
                    bl_poulet.Add(new Block("plat/trap_pilier_col", new Vector3(172.6f, 28.5f, -7.8f), new Vector3(0f, 0f, 0)));
                    bl_poulet.Add(new Block("DOP", new Vector3(265.3571f, 19.47f, -5.94838f), new Vector3(0f, 0f, 0)));
                    bl_poulet.Add(new Block("plat/Icorridor_plat_col", new Vector3(262.61f, 13.5f, -0.0999999f), new Vector3(0f, 0f, 0)));
                    bl_poulet.Add(new Block("plat/Plateforme_col", new Vector3(265.4117f, 13.33f, -4.977777f), new Vector3(0f, 0f, 0)));
                    bl_poulet.Add(new Block("plat/Mur_col", new Vector3(262.73f, 18f, -5.95f), new Vector3(0f, 90f, 0)));
                    bl_poulet.Add(new Block("plat/Icorridor_plat_col", new Vector3(286.47f, 13.5f, -0.0999999f), new Vector3(0f, 0f, 0)));
                    bl_poulet.Add(new Block("plat/Mur_col", new Vector3(274.54f, 31.6f, -0.1799998f), new Vector3(0f, 180f, 0)));
                    bl_poulet.Add(new Block("plat/Mur_col", new Vector3(274.54f, 31.6f, -12f), new Vector3(0f, 0f, 0)));
                    bl_poulet.Add(new Block("plat/Mur_col", new Vector3(286.4f, 31.6f, -0.1799998f), new Vector3(0f, 180f, 0)));
                    bl_poulet.Add(new Block("plat/Mur_col", new Vector3(286.4f, 31.6f, -12f), new Vector3(0f, 0f, 0)));
                    bl_poulet.Add(new Block("plat/Mur_col", new Vector3(516.9f, 20.3f, -61.1f), new Vector3(0f, 0f, 0)));
                    bl_poulet.Add(new Block("plat/Icorridor_plat_col", new Vector3(522.9f, 13.9f, -59.7f), new Vector3(0f, 90f, 0)));
                    bl_poulet.Add(new Block("plat/Icorridor_and_pik-plateforme", new Vector3(523f, 13.8f, -35.8f), new Vector3(0f, 90f, 0)));
                    bl_poulet.Add(new Block("plat/Lcorridor_plat_col", new Vector3(498.8f, 13.6f, 0f), new Vector3(0f, 0f, 0)));
                    bl_poulet.Add(new Block("plat/trap_pilier_col", new Vector3(472.5105f, 17f, -9.275017f), new Vector3(0f, 0f, 0)));
                    bl_poulet.Add(new Block("plat/trap_pilier_col", new Vector3(461.7889f, 17f, -3.697206f), new Vector3(0f, 0f, 0)));
                    bl_poulet.Add(new Block("plat/trap_pilier_col", new Vector3(442.7877f, 17f, -1.87019f), new Vector3(0f, 0f, 0)));
                    bl_poulet.Add(new Block("plat/trap_pilier_col", new Vector3(383.1f, 17.5f, -5.34f), new Vector3(90f, 0f, 0)));
                    bl_poulet.Add(new Block("plat/Icorridor_plat_col", new Vector3(310.31f, 13.5f, -0.0999999f), new Vector3(0f, 0f, 0)));
                    bl_poulet.Add(new Block("plat/trap_pilier_col", new Vector3(383.1f, 17.5f, -9.64f), new Vector3(270f, 0f, 0)));
                    bl_poulet.Add(new Block("plat/Icorridor_plat_col", new Vector3(358.2f, 13.5f, -0.0999999f), new Vector3(0f, 0f, 0)));
                    bl_poulet.Add(new Block("plat/Icorridor_plat_col", new Vector3(334.3f, 13.5f, -0.0999999f), new Vector3(0f, 0f, 0)));
                    bl_poulet.Add(new Block("plat/Icorridor_plat_col", new Vector3(381.4f, 13.5f, -0.0999999f), new Vector3(0f, 0f, 0)));
                    bl_poulet.Add(new Block("plat/Icorridor_plat_col", new Vector3(405.3f, 13.5f, -0.0999999f), new Vector3(0f, 0f, 0)));
                    bl_poulet.Add(new Block("plat/trap_pilier_col", new Vector3(318.8f, 16.2f, -9.64f), new Vector3(270f, 0f, 0)));
                    bl_poulet.Add(new Block("plat/trap_pilier_col", new Vector3(347.2f, 17.5f, -9.64f), new Vector3(270f, 0f, 0)));
                    bl_poulet.Add(new Block("plat/trap_pilier_col", new Vector3(347.2f, 17.5f, -5.34f), new Vector3(90f, 0f, 0)));
                    bl_poulet.Add(new Block("plat/Icorridor_plat_col", new Vector3(428.5f, 13.5f, -0.0999999f), new Vector3(0f, 0f, 0)));
                    bl_poulet.Add(new Block("plat/Icorridor_plat_col", new Vector3(499.1f, 13.5f, -0.0999999f), new Vector3(0f, 0f, 0)));
                    bl_poulet.Add(new Block("plat/Icorridor_plat_col", new Vector3(475.9f, 13.5f, -0.0999999f), new Vector3(0f, 0f, 0)));
                    bl_poulet.Add(new Block("plat/Icorridor_plat_col", new Vector3(452f, 13.5f, -0.0999999f), new Vector3(0f, 0f, 0)));
                    bl_poulet.Add(new Block("plat/trap_pilier_col", new Vector3(490.1f, 17.5f, -9.64f), new Vector3(270f, 0f, 0)));
                    bl_poulet.Add(new Block("plat/trap_pilier_col", new Vector3(490.1f, 17.5f, -5.34f), new Vector3(90f, 0f, 0)));
                    bl_poulet.Add(new Block("plat/trap_pilier_col", new Vector3(451.0126f, 17f, -8.379164f), new Vector3(0f, 0f, 0)));
                    bl_poulet.Add(new Block("plat/trap_pilier_col", new Vector3(435.1435f, 17f, -3.859549f), new Vector3(0f, 0f, 0)));
                    bl_poulet.Add(new Block("plat/trap_pilier_col", new Vector3(422.1265f, 17f, -9.678993f), new Vector3(0f, 0f, 0)));
                    bl_poulet.Add(new Block("plat/trap_pilier_col", new Vector3(417.7932f, 17f, -0.7200003f), new Vector3(0f, 0f, 0)));
                    bl_poulet.Add(new Block("plat/trap_pilier_col", new Vector3(406.7797f, 17f, -7.832581f), new Vector3(0f, 0f, 0)));
                    bl_poulet.Add(new Block("plat/trap_pilier_col", new Vector3(318.8f, 16.2f, -5.34f), new Vector3(90f, 0f, 0)));
                    bl_poulet.Add(new Block("ADR", new Vector3(290.1726f, 16.44f, -5.957782f), new Vector3(0f, 0f, 0)));
                    bl_poulet.Add(new Block("plat/Icorridor_plat_col", new Vector3(286.47f, 13.5f, -0.0999999f), new Vector3(0f, 0f, 0)));
                    bl_poulet.Add(new Block("Teleportation portal", new Vector3(516.8f, 14.844f, -54.07f), new Vector3(0f, 0f, 0), "tp_Stp%l"));
                    MapConstructor mc_poulet = new MapConstructor(bl_poulet);
                    Map poulet = new Map(mc_poulet, name, MapType.OTHER, new Vector3(-12.685F, 26.5F, -5.74F));
                    poulet.GetCheckpoints().Add(new Checkpoint( new Vector2(268, -12), new Vector2(293, 0), new Vector3(274.5F, 15, -6)));
                    poulet.hintsList.Add("Fais attention au timing des scies pour passer.");
                    poulet.hintsList.Add("Dépêche toi avec l'adrénaline tu n'as que  15 secondes.");
                    return poulet;

                case "enigme":
                    List<Block> bl_enigme = new List<Block>();
                    bl_enigme.Add(new Block("Icorridor_col", new Vector3(143.1f, 0f, -23.6f), new Vector3(0f, 0f, 0f)));
                    bl_enigme.Add(new Block("Lcorridor2_col", new Vector3(95.10001f, 0f, -0.0999999f), new Vector3(0f, 0f, 0f)));
                    bl_enigme.Add(new Block("windows3D_col", new Vector3(84.22f, 8.599998f, -40.96204f), new Vector3(0f, 0f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(77.36f, 6.299999f, -12.06f), new Vector3(0f, 180f, 0f)));
                    bl_enigme.Add(new Block("Door3D_col", new Vector3(76.85001f, 2.607101f, -6.21f), new Vector3(0f, 90f, 0f), "door_1"));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(77.60001f, 6.5f, 5.8f), new Vector3(0f, 180f, 0f)));
                    bl_enigme.Add(new Block("Icorridor2_col", new Vector3(83.04001f, 0.0997467f, -23.9f), new Vector3(0f, 270f, 0f)));
                    bl_enigme.Add(new Block("Mur", new Vector3(-16.45999f, 6.619999f, -5.51623f), new Vector3(0f, 90f, 0f)));
                    bl_enigme.Add(new Block("Icorridor_col", new Vector3(23.5f, 0f, 0f), new Vector3(0f, 0f, 0f)));
                    bl_enigme.Add(new Block("Icorridor_col", new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f)));
                    bl_enigme.Add(new Block("Icorridor_col", new Vector3(47.2f, 0f, 0f), new Vector3(0f, 0f, 0f)));
                    bl_enigme.Add(new Block("Icorridor_col", new Vector3(71.10001f, 0f, 0f), new Vector3(0f, 0f, 0f)));
                    bl_enigme.Add(new Block("Icorridor2_col", new Vector3(107.08f, 0f, -35.9f), new Vector3(0f, 180f, 0f)));
                    bl_enigme.Add(new Block("windows3D_col", new Vector3(96.07001f, 8.599998f, -40.96204f), new Vector3(0f, 0f, 0f)));
                    bl_enigme.Add(new Block("sol", new Vector3(119.09f, 0f, -23.7f), new Vector3(0f, 0f, 0f)));
                    bl_enigme.Add(new Block("windows3D_col", new Vector3(119.4f, 8.599998f, -40.96204f), new Vector3(0f, 0f, 0f)));
                    bl_enigme.Add(new Block("windows3D_col", new Vector3(107.5f, 8.599998f, -40.96204f), new Vector3(0f, 0f, 0f), "window_1"));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(124.99f, 6.689999f, -40.92f), new Vector3(0f, 0f, 0f)));
                    bl_enigme.Add(new Block("Mur", new Vector3(122.3f, 6.408253f, -60.88087f), new Vector3(0f, 0f, 0f)));
                    bl_enigme.Add(new Block("Icorridor_col", new Vector3(124.7f, -0.6088562f, -59.62f), new Vector3(0f, 90f, 0f)));
                    bl_enigme.Add(new Block("Icorridor_col", new Vector3(103.1f, -0.6088562f, -59.62f), new Vector3(0f, 90f, 0f)));
                    bl_enigme.Add(new Block("Mur", new Vector3(97.89999f, 6.408253f, -60.88087f), new Vector3(0f, 0f, 0f)));
                    bl_enigme.Add(new Block("Mur", new Vector3(81.7f, 6.408253f, -60.88087f), new Vector3(0f, 0f, 0f)));
                    bl_enigme.Add(new Block("Icorridor_col", new Vector3(90.39999f, -0.6088562f, -59.62f), new Vector3(0f, 90f, 0f)));
                    bl_enigme.Add(new Block("Mur", new Vector3(111.5829f, 6.400002f, -11.6f), new Vector3(0f, 180f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(99.58f, 6.43f, -11.5f), new Vector3(0f, 270f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(93.48001f, 6.369999f, -9.4f), new Vector3(0f, 90f, 0f)));
                    bl_enigme.Add(new Block("Mur", new Vector3(111.5829f, 6.400002f, -10.00649f), new Vector3(0f, 0f, 0f)));
                    bl_enigme.Add(new Block("Button", new Vector3(107.5185f, 11.92f, -59.73f), new Vector3(0f, 0f, 0f), "button_1"));
                    bl_enigme.Add(new Block("Mur", new Vector3(108.4197f, 6.408253f, -60.88087f), new Vector3(0f, 0f, 0f)));
                    bl_enigme.Add(new Block("Icorridor_col", new Vector3(113.7f, -0.6088562f, -59.62f), new Vector3(0f, 90f, 0f)));
                    bl_enigme.Add(new Block("Door3D_col", new Vector3(125.34f, 2.700001f, -29.24f), new Vector3(0f, 90f, 0f), "door_2"));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(124.99f, 6.689999f, -24.15f), new Vector3(0f, 0f, 0f)));
                    bl_enigme.Add(new Block("PressionPlate", new Vector3(117.897f, 0.7999992f, -2.424569f), new Vector3(0f, 0f, 0f),"pressionplate_2"));
                    bl_enigme.Add(new Block("PressionPlate", new Vector3(29.3752f, 0.6699982f, -6.209277f), new Vector3(0f, 0f, 0f), "pressionplate_1"));
                    bl_enigme.Add(new Block("Icorridor_col", new Vector3(167.1081f, 0f, -23.6f), new Vector3(0f, 0f, 0f)));
                    bl_enigme.Add(new Block("Icorridor_col", new Vector3(190.8081f, 0f, -23.6f), new Vector3(0f, 0f, 0f)));
                    bl_enigme.Add(new Block("Icorridor_col", new Vector3(286.44f, 0f, -23.64f), new Vector3(0f, 0f, 0f)));
                    bl_enigme.Add(new Block("Mur", new Vector3(249.859f, 5.927101f, -76.7f), new Vector3(0f, 90f, 0f)));
                    bl_enigme.Add(new Block("Lcorridor2_col", new Vector3(262.3f, 0f, -59.4f), new Vector3(0f, 90f, 0f)));
                    bl_enigme.Add(new Block("Icorridor2_col", new Vector3(238.34f, 0f, -47.8f), new Vector3(0f, 0f, 0f)));
                    bl_enigme.Add(new Block("Icorridor2_col", new Vector3(238.7f, 0f, 0f), new Vector3(0f, 0f, 0f)));
                    bl_enigme.Add(new Block("Lcorridor2_col", new Vector3(238.2f, 0f, 24.1f), new Vector3(0f, 0f, 0f)));
                    bl_enigme.Add(new Block("Icorridor2_col", new Vector3(226.3f, 0f, -0.0999999f), new Vector3(0f, 270f, 0f)));
                    bl_enigme.Add(new Block("Mur", new Vector3(219.8286f, 5.197102f, 14.3f), new Vector3(0f, 0f, 0f)));
                    bl_enigme.Add(new Block("Mur", new Vector3(209.4f, 6.3871f, -9.82f), new Vector3(0f, 90f, 0f)));
                    bl_enigme.Add(new Block("Mur", new Vector3(232.2619f, 6.5f, -21.6f), new Vector3(0f, 0f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(197.4f, 6.437099f, -21.8f), new Vector3(0f, 90f, 0f)));
                    bl_enigme.Add(new Block("Icorridor2_col", new Vector3(226.79f, 0f, -11.75f), new Vector3(0f, 180f, 0f)));
                    bl_enigme.Add(new Block("Lcorridor2_col", new Vector3(202.79f, 0f, 0.3000002f), new Vector3(0f, 270f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(197.3f, 6.459999f, -17.6f), new Vector3(0f, 180f, 0f)));
                    bl_enigme.Add(new Block("sol", new Vector3(214.9f, 0f, -23.77f), new Vector3(0f, 0f, 0f)));
                    bl_enigme.Add(new Block("sol", new Vector3(238.9f, 0f, -23.77f), new Vector3(0f, 0f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(203.37f, 6.43f, -24.2f), new Vector3(0f, 270f, 0f)));
                    bl_enigme.Add(new Block("Mur", new Vector3(232.2619f, 6.5f, -23.6f), new Vector3(0f, 180f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(220.29f, 6.5f, -24.2f), new Vector3(0f, 270f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(214.29f, 6.5f, -21.8f), new Vector3(0f, 90f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(203.3381f, 6.43f, -21.8f), new Vector3(0f, 90f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(209.3381f, 6.43f, -24.2f), new Vector3(0f, 270f, 0f)));
                    bl_enigme.Add(new Block("Mur", new Vector3(207.64f, 6.389999f, -9.82f), new Vector3(0f, 270f, 0f)));
                    bl_enigme.Add(new Block("sol", new Vector3(238.9f, 0f, 0.1999998f), new Vector3(0f, 0f, 0f)));
                    bl_enigme.Add(new Block("Mur", new Vector3(219.8286f, 5.169998f, 12.1f), new Vector3(0f, 180f, 0f)));
                    bl_enigme.Add(new Block("Mur", new Vector3(215.3f, 6.389999f, -9.82f), new Vector3(0f, 270f, 0f)));
                    bl_enigme.Add(new Block("Mur", new Vector3(217f, 6.389999f, -9.82f), new Vector3(0f, 90f, 0f)));
                    bl_enigme.Add(new Block("Mur", new Vector3(232.6f, 6.389999f, -9.82f), new Vector3(0f, 90f, 0f)));
                    bl_enigme.Add(new Block("Mur", new Vector3(230.9f, 6.389999f, -9.82f), new Vector3(0f, 270f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(230.8f, 6.389999f, 2.3f), new Vector3(0f, 0f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(233f, 6.389999f, 8.6f), new Vector3(0f, 180f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(233f, 6.389999f, 14f), new Vector3(0f, 180f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(230.8f, 6.389999f, 7.699999f), new Vector3(0f, 0f, 0f)));
                    bl_enigme.Add(new Block("sol", new Vector3(262.5f, 0f, -23.77f), new Vector3(0f, 0f, 0f)));
                    bl_enigme.Add(new Block("Mur", new Vector3(256.1f, 6.5f, -21.6f), new Vector3(0f, 0f, 0f)));
                    bl_enigme.Add(new Block("Mur", new Vector3(256.8f, 6.5f, -23.6f), new Vector3(0f, 180f, 0f)));
                    bl_enigme.Add(new Block("Mur", new Vector3(250f, 6.389999f, 17.7f), new Vector3(0f, 270f, 0f)));
                    bl_enigme.Add(new Block("Mur", new Vector3(251.7f, 6.389999f, 17.7f), new Vector3(0f, 90f, 0f)));
                    bl_enigme.Add(new Block("Mur", new Vector3(251.7f, 6.389999f, -11.1f), new Vector3(0f, 90f, 0f)));
                    bl_enigme.Add(new Block("Mur", new Vector3(250f, 6.389999f, -11.1f), new Vector3(0f, 270f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(244.0681f, 6.389999f, 0.5999999f), new Vector3(0f, 90f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(250.0681f, 6.389999f, -1.8f), new Vector3(0f, 270f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(244.0681f, 6.389999f, -1.8f), new Vector3(0f, 270f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(238.0681f, 6.389999f, 0.5999999f), new Vector3(0f, 90f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(231.9f, 6.389999f, -11.6f), new Vector3(0f, 90f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(237.9f, 6.5f, -14f), new Vector3(0f, 270f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(243.9f, 6.5f, -14f), new Vector3(0f, 270f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(237.9f, 6.389999f, -11.6f), new Vector3(0f, 90f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(251.7f, 6.389999f, 6f), new Vector3(0f, 270f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(245.7f, 6.389999f, 8.4f), new Vector3(0f, 90f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(240.1f, 6.389999f, 8.4f), new Vector3(0f, 90f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(240.8f, 6.389999f, 11.9f), new Vector3(0f, 0f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(246.1f, 6.389999f, 6f), new Vector3(0f, 270f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(243f, 6.389999f, 18.2f), new Vector3(0f, 180f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(243f, 6.389999f, 12.8f), new Vector3(0f, 180f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(240.8f, 6.389999f, 6.5f), new Vector3(0f, 0f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(240.8f, 6.389999f, 17.4f), new Vector3(0f, 0f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(243f, 6.389999f, 23.7f), new Vector3(0f, 180f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(257.1f, 6.389999f, 6f), new Vector3(0f, 270f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(251.1f, 6.389999f, 8.4f), new Vector3(0f, 90f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(256.7f, 6.389999f, 8.4f), new Vector3(0f, 90f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(262.7f, 6.389999f, 6f), new Vector3(0f, 270f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(261.9f, 6.389999f, 23.7f), new Vector3(0f, 180f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(259.7f, 6.389999f, 17.4f), new Vector3(0f, 0f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(259.7f, 6.389999f, 6.5f), new Vector3(0f, 0f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(261.9f, 6.389999f, 12.8f), new Vector3(0f, 180f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(261.9f, 6.389999f, 18.2f), new Vector3(0f, 180f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(259.7f, 6.389999f, 11.9f), new Vector3(0f, 0f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(266.8f, 6.5f, -22.7f), new Vector3(0f, 0f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(217.4f, 6.389999f, 0.5999999f), new Vector3(0f, 90f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(223.4f, 6.389999f, -1.8f), new Vector3(0f, 270f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(229.4f, 6.5f, -14.4f), new Vector3(0f, 270f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(223.4f, 6.389999f, -12f), new Vector3(0f, 90f, 0f)));
                    bl_enigme.Add(new Block("Mur", new Vector3(208.8f, 6.5f, -36f), new Vector3(0f, 0f, 0f)));
                    bl_enigme.Add(new Block("Mur", new Vector3(208.8f, 6.5f, -38f), new Vector3(0f, 180f, 0f)));
                    bl_enigme.Add(new Block("Mur", new Vector3(232f, 6.5f, -37.6f), new Vector3(0f, 180f, 0f)));
                    bl_enigme.Add(new Block("Mur", new Vector3(232f, 6.5f, -35.6f), new Vector3(0f, 0f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(262.38f, 6.5f, -35.8f), new Vector3(0f, 90f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(268.38f, 6.5f, -38.2f), new Vector3(0f, 270f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(262.57f, 6.5f, -38.2f), new Vector3(0f, 270f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(256.57f, 6.5f, -35.8f), new Vector3(0f, 90f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(244f, 6.5f, -35.8f), new Vector3(0f, 90f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(250f, 6.5f, -38.2f), new Vector3(0f, 270f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(267.69f, 6.5f, -44.19f), new Vector3(0f, 0f, 0f)));
                    bl_enigme.Add(new Block("Icorridor2_col", new Vector3(227f, 0f, -59.6f), new Vector3(0f, 180f, 0f)));
                    bl_enigme.Add(new Block("sol", new Vector3(238.9f, 0f, -47.6f), new Vector3(0f, 0f, 0f)));
                    bl_enigme.Add(new Block("Icorridor2_col", new Vector3(238.34f, 0f, -59.5f), new Vector3(0f, 90f, 0f)));
                    bl_enigme.Add(new Block("Lcorridor2_col", new Vector3(226.5f, 0f, -83.1f), new Vector3(0f, 180f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(249.76f, 6.5f, -35.99f), new Vector3(0f, 180f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(247.46f, 6.5f, -41.99f), new Vector3(0f, 0f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(247.46f, 5.700001f, -47.68f), new Vector3(0f, 0f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(249.76f, 5.700001f, -41.68f), new Vector3(0f, 180f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(249.76f, 5.700001f, -47.3f), new Vector3(0f, 180f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(247.46f, 5.700001f, -53.3f), new Vector3(0f, 0f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(255.65f, 5.700001f, -51.03f), new Vector3(0f, 90f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(261.65f, 5.700001f, -53.43f), new Vector3(0f, 270f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(255.65f, 5.700001f, -53.43f), new Vector3(0f, 270f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(249.65f, 5.700001f, -51.03f), new Vector3(0f, 90f, 0f)));
                    bl_enigme.Add(new Block("Mur", new Vector3(248.2f, 5.900002f, -76.7f), new Vector3(0f, 270f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(249.65f, 5.700001f, -64.8f), new Vector3(0f, 90f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(255.65f, 5.700001f, -67.2f), new Vector3(0f, 270f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(261.65f, 5.700001f, -67.2f), new Vector3(0f, 270f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(255.65f, 5.700001f, -64.8f), new Vector3(0f, 90f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(262f, 5.700001f, -76.5f), new Vector3(0f, 90f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(268f, 5.700001f, -78.9f), new Vector3(0f, 270f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(262f, 5.700001f, -78.9f), new Vector3(0f, 270f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(256f, 5.700001f, -76.5f), new Vector3(0f, 90f, 0f)));
                    bl_enigme.Add(new Block("Mur", new Vector3(237f, 5.900002f, -57.6f), new Vector3(0f, 270f, 0f)));
                    bl_enigme.Add(new Block("Mur", new Vector3(238.6f, 5.900002f, -57.6f), new Vector3(0f, 90f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(238.8f, 5.700001f, -75.25f), new Vector3(0f, 180f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(236.5f, 5.700001f, -81.25f), new Vector3(0f, 0f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(236.5f, 5.700001f, -75.55f), new Vector3(0f, 0f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(238.8f, 5.700001f, -69.55f), new Vector3(0f, 180f, 0f)));
                    bl_enigme.Add(new Block("Mur", new Vector3(227.2f, 5.700001f, -56.6f), new Vector3(0f, 0f, 0f)));
                    bl_enigme.Add(new Block("Mur", new Vector3(227.2f, 5.700001f, -58.6f), new Vector3(0f, 180f, 0f)));
                    bl_enigme.Add(new Block("Mur", new Vector3(227.2f, 5.700001f, -71.8f), new Vector3(0f, 180f, 0f)));
                    bl_enigme.Add(new Block("Mur", new Vector3(227.2f, 5.700001f, -69.8f), new Vector3(0f, 0f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(231.9f, 5.700001f, -45.8f), new Vector3(0f, 90f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(237.9f, 5.700001f, -48.2f), new Vector3(0f, 270f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(231.9f, 5.700001f, -48.2f), new Vector3(0f, 270f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(225.9f, 5.700001f, -45.8f), new Vector3(0f, 90f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(225.9f, 5.700001f, -78.1f), new Vector3(0f, 90f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(231.9f, 5.700001f, -80.5f), new Vector3(0f, 270f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(237.9f, 5.700001f, -80.5f), new Vector3(0f, 270f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(231.9f, 5.700001f, -78.1f), new Vector3(0f, 90f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(218.5f, 5.700001f, -49.2f), new Vector3(0f, 0f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(220.8f, 5.700001f, -43.2f), new Vector3(0f, 180f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(220.8f, 6.5f, -37.6f), new Vector3(0f, 180f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(218.5f, 6.5f, -43.6f), new Vector3(0f, 0f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(218.5f, 5.700001f, -83.4f), new Vector3(0f, 0f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(220.8f, 5.700001f, -77.4f), new Vector3(0f, 180f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(220.8f, 5.700001f, -83f), new Vector3(0f, 180f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(218.5f, 5.700001f, -89f), new Vector3(0f, 0f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(218.7f, 5.700001f, -61.5f), new Vector3(0f, 180f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(216.4f, 5.700001f, -67.5f), new Vector3(0f, 0f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(196.8f, 5.700001f, -56.1f), new Vector3(0f, 90f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(202.8f, 5.700001f, -58.5f), new Vector3(0f, 270f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(208.8f, 5.700001f, -58.5f), new Vector3(0f, 270f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(202.8f, 5.700001f, -56.1f), new Vector3(0f, 90f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(202.8f, 5.700001f, -69.6f), new Vector3(0f, 90f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(208.8f, 5.700001f, -72f), new Vector3(0f, 270f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(202.8f, 5.700001f, -72f), new Vector3(0f, 270f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(196.8f, 5.700001f, -69.6f), new Vector3(0f, 90f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(206.1f, 5.700001f, -83.6f), new Vector3(0f, 0f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(208.4f, 5.700001f, -77.6f), new Vector3(0f, 180f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(208.4f, 5.700001f, -72f), new Vector3(0f, 180f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(206.1f, 5.700001f, -78f), new Vector3(0f, 0f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(206.1f, 5.700001f, -50.6f), new Vector3(0f, 0f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(208.4f, 5.700001f, -44.6f), new Vector3(0f, 180f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(208.4f, 5.700001f, -50.2f), new Vector3(0f, 180f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(206.1f, 5.700001f, -56.2f), new Vector3(0f, 0f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(197.4f, 6.5f, -36.9f), new Vector3(0f, 180f, 0f)));
                    bl_enigme.Add(new Block("Teleportation portal", new Vector3(283.6161f, 0.5299988f, -30.01839f), new Vector3(0f, 0f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(268.4081f, 6.389999f, -10.90916f), new Vector3(0f, 270f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(262.4081f, 6.389999f, -8.509156f), new Vector3(0f, 90f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(252.0081f, 6.389999f, -2.009156f), new Vector3(0f, 90f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(258.0081f, 6.389999f, -4.409156f), new Vector3(0f, 270f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(263.6082f, 6.389999f, -4.409156f), new Vector3(0f, 270f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(257.6082f, 6.389999f, -2.009156f), new Vector3(0f, 90f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(256.6082f, 6.389999f, -8.309156f), new Vector3(0f, 90f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(262.6082f, 6.389999f, -10.70916f), new Vector3(0f, 270f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(257.6082f, 6.389999f, -14.40916f), new Vector3(0f, 90f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(263.6082f, 6.389999f, -16.80916f), new Vector3(0f, 270f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(258.0081f, 6.389999f, -16.80916f), new Vector3(0f, 270f, 0f)));
                    bl_enigme.Add(new Block("Icorridor3x6_col", new Vector3(252.0081f, 6.389999f, -14.40916f), new Vector3(0f, 90f, 0f)));
                    bl_enigme.Add(new Block("Button", new Vector3(201.7533f, 8.700001f, -73.82f), new Vector3(0f, 0f, 0f), "button_2"));
                    //bl_enigme.Add(new Block("Button", new Vector3(201.8547f, 6.75f, -55.2f), new Vector3(0f, 0f, 0f), "button_3"));
                    //bl_enigme.Add(new Block("Button", new Vector3(235.22f, 8.889999f, -62.57f), new Vector3(0f, 90f, 0f), "button_4"));
                    bl_enigme.Add(new Block("Button", new Vector3(219.63f, 10.57f, -64.55f), new Vector3(0f, 90f, 0f), "button_5"));
                    bl_enigme.Add(new Block("Button", new Vector3(265.79f, 9.5f, -82.42f), new Vector3(0f, 90f, 0f), "button_6"));
                    bl_enigme.Add(new Block("Button", new Vector3(234.183f, 9.650002f, -17.16518f), new Vector3(0f, 90f, 0f), "button_7"));
                    //bl_enigme.Add(new Block("Button", new Vector3(256.24f, 7.330002f, 9.285f), new Vector3(0f, 0f, 0f), "button_8"));
                    //bl_enigme.Add(new Block("Button", new Vector3(229.61f, 9.456463f, -19.00024f), new Vector3(0f, 90f, 0f), "button_9"));
                    //bl_enigme.Add(new Block("Button", new Vector3(204.267f, 9.484932f, -20.238f), new Vector3(0f, 0f, 0f), "button_10"));
                    bl_enigme.Add(new Block("Door3D_col", new Vector3(276.7964f, 2.540001f, -29.60845f), new Vector3(0f, 90f, 0f), "door_3"));
                    bl_enigme.Add(new Block("Mur", new Vector3(290.47f, 6.489998f, -29.24608f), new Vector3(0f, 270f, 0f)));


                    MapConstructor mc_enigme = new MapConstructor(bl_enigme);
                    Map enigme = new Map(mc_enigme, name, MapType.OTHER, new Vector3(0, 5, -4.7f));
                    enigme.AddElement(new string[] { "button_1" }, "door_2", ExecType.DOOR, false);
                    enigme.AddElement(new string[] { "button_2", "button_5", "button_6", "button_7"}, "door_3", ExecType.DOOR, false);
                    enigme.AddElement(new string[] { "pressionplate_1" }, "door_1", ExecType.DOOR, ExecutorType.PRESSIONPLATE,false);
                    enigme.AddElement(new string[] { "pressionplate_2" }, "window_1", ExecType.WINDOW, ExecutorType.PRESSIONPLATE, false);

                    enigme.hintsList.Add("Les plaques de pressions se désactivent avec le temps.");
                    enigme.hintsList.Add("Il y a 4 boutons à activer dans le labyrinthe.");
                    enigme.hintsList.Add("Un a gauche, trois à droite.");

                    return enigme;


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
					
				case "TestPressionPlate":
					List<Block> bl_TestPressionPlate = new List<Block>();
					bl_TestPressionPlate.Add(new Block("PressionPlate", new Vector3(154.8005f,0.22f,117.5f), new Vector3(0f, 0f, 0), "pressionplate_1"));
					bl_TestPressionPlate.Add(new Block("Door3D_col", new Vector3(155.3491f,2.017251f,130.8972f), new Vector3(0f, 0f, 0), "door_1"));
					MapConstructor mc_TestPressionPlate = new MapConstructor(bl_TestPressionPlate);
					Map TestPressionPlate = new Map(mc_TestPressionPlate, name, MapType.OTHER, new Vector3(152,1,108));
					TestPressionPlate.AddElement(new string[] { "pressionplate_1" }, "door_1", ExecType.DOOR, ExecutorType.PRESSIONPLATE, false);
					return TestPressionPlate;
					break;

                case "Stp3":
                    List<Block> bl_Stp3 = new List<Block>();
                    bl_Stp3.Add(new Block("Teleportation_portal_inactif", new Vector3(11.05383f, 4.386359f, 0f), new Vector3(0f, 0f, 0f)));
                    bl_Stp3.Add(new Block("Teleportation_portal_inactif", new Vector3(-11.05383f, 4.386359f, 0f), new Vector3(0f, 0f, 0f)));
                    bl_Stp3.Add(new Block("Teleportation portal", new Vector3(0f, 4.386359f, 11.05383f), new Vector3(0f, 0f, 0f), "tp_plat1"));
                    bl_Stp3.Add(new Block("Teleportation_portal_inactif", new Vector3(0f, 4.386359f, -11.05383f), new Vector3(0f, 0f, 0f)));
                    bl_Stp3.Add(new Block("Salle_de_tp_sans_tp_3_rouge_col", new Vector3(0f, 4.126359f, 0f), new Vector3(0f, 0f, 0f)));
                    bl_Stp3.Add(new Block("TerrainCOOL", new Vector3(-242.2479f, -0.8736415f, -261.6377f), new Vector3(0f, 0f, 0f)));
                    bl_Stp3.Add(new Block("ADR2", new Vector3(14.41275f, 6.23f, -0.1199999f), new Vector3(325f, 0f, 0f)));
                    bl_Stp3.Add(new Block("Button", new Vector3(0.07999992f, 7.099842f, -14.25f), new Vector3(0f, 0f, 0f)));
                    bl_Stp3.Add(new Block("Scile", new Vector3(-0.01089096f, 5.39f, 13.12704f), new Vector3(0f, 0f, 0f)));
                    MapConstructor mc_Stp3 = new MapConstructor(bl_Stp3);
                    Map Stp3 = new Map(mc_Stp3, name, MapType.OTHER, new Vector3(0, 10, 0));
                    return Stp3;

                case "Stp2":
                    List<Block> bl_Stp2 = new List<Block>();
                    bl_Stp2.Add(new Block("Teleportation portal", new Vector3(0f, 4.386359f, -11.05383f), new Vector3(0f, 0f, 0f), "tp_map1"));
                    bl_Stp2.Add(new Block("Teleportation portal", new Vector3(0f, 4.386359f, 11.05383f), new Vector3(0f, 0f, 0f), "tp_plat1"));
                    bl_Stp2.Add(new Block("Teleportation_portal_inactif", new Vector3(-11.05383f, 4.386359f, 0f), new Vector3(0f, 0f, 0f)));
                    bl_Stp2.Add(new Block("Teleportation_portal_inactif", new Vector3(11.05383f, 4.386359f, 0f), new Vector3(0f, 0f, 0f)));
                    bl_Stp2.Add(new Block("Salle_de_tp_sans_tp_2_rouge_col", new Vector3(0f, 4.126359f, 0f), new Vector3(0f, 0f, 0f)));
                    bl_Stp2.Add(new Block("TerrainCOOL", new Vector3(-242.2479f, -0.8736415f, -261.6377f), new Vector3(0f, 0f, 0f)));
                    bl_Stp2.Add(new Block("ADR2", new Vector3(14.41275f, 6.23f, -0.1199999f), new Vector3(325f, 0f, 0f)));
                    bl_Stp2.Add(new Block("Button", new Vector3(0.07999992f, 7.099842f, -14.25f), new Vector3(0f, 0f, 0f)));
                    bl_Stp2.Add(new Block("Scile", new Vector3(-0.01089096f, 5.39f, 13.12704f), new Vector3(0f, 0f, 0f)));
                    MapConstructor mc_Stp2 = new MapConstructor(bl_Stp2);
                    Map Stp2 = new Map(mc_Stp2, name, MapType.OTHER, new Vector3(0, 10, 0));
                    return Stp2;

                case "Stp1":
                    List<Block> bl_Stp1 = new List<Block>();
                    bl_Stp1.Add(new Block("TerrainCOOL", new Vector3(-242.2479f, -0.8736415f, -261.6377f), new Vector3(0f, 0f, 0f)));
                    bl_Stp1.Add(new Block("Teleportation portal", new Vector3(0f, 4.386359f, -11.05383f), new Vector3(0f, 0f, 0f), "tp_map1"));
                    bl_Stp1.Add(new Block("Teleportation portal", new Vector3(11.05383f, 4.386359f, 0f), new Vector3(0f, 0f, 0f), "tp_poulet"));
                    bl_Stp1.Add(new Block("Teleportation portal", new Vector3(0f, 4.386359f, 11.05383f), new Vector3(0f, 0f, 0f), "tp_plat1"));
                    bl_Stp1.Add(new Block("Teleportation_portal_inactif", new Vector3(-11.05383f, 4.386359f, 0f), new Vector3(0f, 0f, 0f)));
                    bl_Stp1.Add(new Block("Salle_de_tp_sans_tp_1_rouge_col", new Vector3(0f, 4.126359f, 0f), new Vector3(0f, 0f, 0f)));
                    bl_Stp1.Add(new Block("ADR2", new Vector3(14.41275f, 6.23f, -0.1199999f), new Vector3(325f, 0f, 0f)));
                    bl_Stp1.Add(new Block("Button", new Vector3(0.07999992f, 7.099842f, -14.25f), new Vector3(0f, 0f, 0f)));
                    bl_Stp1.Add(new Block("Scile", new Vector3(-0.01089096f, 5.39f, 13.12704f), new Vector3(0f, 0f, 0f)));
                    MapConstructor mc_Stp1 = new MapConstructor(bl_Stp1);
                    Map Stp1 = new Map(mc_Stp1, name, MapType.OTHER, new Vector3(0, 10, 0));
                    return Stp1;


                case "Stp0":
                    List<Block> bl_Stp0 = new List<Block>();
                    bl_Stp0.Add(new Block("Teleportation portal", new Vector3(0f, 4.386359f, -11.05383f), new Vector3(0f, 0f, 0f),"tp_map1"));
                    bl_Stp0.Add(new Block("Teleportation portal", new Vector3(-11.05383f, 4.386359f, 0f), new Vector3(0f, 0f, 0f), "tp_enigme"));
                    bl_Stp0.Add(new Block("Teleportation portal", new Vector3(11.05383f, 4.386359f, 0f), new Vector3(0f, 0f, 0f), "tp_poulet"));
                    bl_Stp0.Add(new Block("Teleportation portal", new Vector3(0f, 4.386359f, 11.05383f), new Vector3(0f, 0f, 0f), "tp_plat1"));
                    bl_Stp0.Add(new Block("Salle_de_tp_sans_tp_col", new Vector3(0f, 4.126359f, 0f), new Vector3(0f, 0f, 0f)));
                    bl_Stp0.Add(new Block("ADR2", new Vector3(14.41275f, 6.23f, -0.1199999f), new Vector3(325f, 0f, 0f)));
                    bl_Stp0.Add(new Block("Button", new Vector3(0.07999992f, 7.099842f, -14.25f), new Vector3(0f, 0f, 0f)));
                    bl_Stp0.Add(new Block("TerrainCOOL", new Vector3(-242.2479f, -0.8736415f, -261.6377f), new Vector3(0f, 0f, 0f)));
                    bl_Stp0.Add(new Block("Scile", new Vector3(-0.01089096f, 5.39f, 13.12704f), new Vector3(0f, 0f, 0f)));
                    MapConstructor mc_Stp0 = new MapConstructor(bl_Stp0);
                    Map Stp0 = new Map(mc_Stp0, name, MapType.OTHER, new Vector3(0, 10, 0));
                    return Stp0;


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
