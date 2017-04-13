using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

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

		public Map(string name, MapType type)
        {
        	this.name = name;
			this.type = type;

			this.checkpoints = new List<Checkpoint>();
			//this.checkpoints.Add(new Checkpoint(new Vector2(0,0), new Vector2(1,1), new Vector2(0.5,0.5)));

			this.executables = new List<Executable>();
			/*
			 * GameObject ex;
			 * List<Button> butts;
			 * this.executables.Add(ex, new Window(butts, ex));
			*/

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
