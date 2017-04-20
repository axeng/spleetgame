using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
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

		public Map(MapConstructor constructor, string name, MapType type) : this(name, type)
        {
			constructor.Construct();
        }
		
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
		public void AddElement(string[] buts, string element, ExecType type)
		{
			List<Button> buttonsElement = new List<Button>();

			foreach(string but in buts)
			{
				buttonsElement.Add(new Button(null, GameObject.FindWithTag(but + "_true"), GameObject.FindWithTag(but + "_false")));
			}

			switch(type)
			{
				case ExecType.DOOR:
                    this.AddExec(new Door(null, GameObject.FindWithTag(element), GameObject.FindWithTag(element + "_left"), GameObject.FindWithTag(element + "_right")), buttonsElement);
					break;

				case ExecType.WINDOW:
					this.AddExec(new Window(null, GameObject.FindWithTag(element)), buttonsElement);
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
                    //MapTest.Add(new Block(BlockType.Icorridor, new Vector3(0,0,0), 0));
                    MapTest.Add(new Block(BlockType.Icorridor, new Vector3(24, 0, 0), 0));
                    MapTest.Add(new Block(BlockType.Icorridor, new Vector3(48, 0, 0), 0));
                    MapTest.Add(new Block(BlockType.Xcorridor, new Vector3(72, 0, 0), 0));
                    MapTest.Add(new Block(BlockType.Door3D, new Vector3(6, 2, -6), 90));
                    MapConstructor test1 = new MapConstructor(MapTest);

                    return new Map(test1, "oklolsalut", MapType.TEST);
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
