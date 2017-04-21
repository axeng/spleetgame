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
				if (tag)
					buttonsElement.Add(new Button(null, GameObject.FindWithTag(but + "_true"), GameObject.FindWithTag(but + "_false")));
				else
					buttonsElement.Add(new Button(null, GameObject.Find(but + "_true"), GameObject.Find(but + "_false")));
			}

			switch(type)
			{
				case ExecType.DOOR:
                    this.AddExec(new Door(null, GameObject.Find(element), GameObject.Find(element + "_left"), GameObject.Find(element + "_right")), buttonsElement);
					break;

				case ExecType.WINDOW:
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
                    MapTest.Add(new Block(BlockType.Door3D, new Vector3(6, 2, -6), 90));
                    MapTest.Add(new Block(BlockType.Lcorridor, new Vector3(60, 0, -60), 180));
                    MapTest.Add(new Block(BlockType.Enigme1, new Vector3(91, 23, -71), 180));
                    MapConstructor test1 = new MapConstructor(MapTest);

                    return new Map(test1, "oklolsalut", MapType.TEST);
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
