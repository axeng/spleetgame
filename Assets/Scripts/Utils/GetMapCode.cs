using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GetMapCode : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GameObject obj = gameObject;
		string nameT = obj.name.Replace(" ", "");
		string result = "";
		result += "List<Block> bl_" + nameT + " = new List<Block>();\n";

		for (int i = 0; i < obj.transform.childCount; i++)
		{
			GameObject go = obj.transform.GetChild(i).gameObject;
			result += "bl_" + nameT
				+ ".Add(new Block(\""
					+ PrefabUtility.GetPrefabParent(go).name.Split(' ')[0]
					+ "\", new Vector3("
						+ go.transform.position.x
						+ "f," + go.transform.position.y
						+ "f," + go.transform.position.z
					+ "f)," + go.transform.rotation.eulerAngles.y+"f));\n";
		}
		
		result += "MapConstructor mc_" + nameT + " = new MapConstructor(bl_" + nameT + ");\n";
		result += "Map " + nameT + " = new Map(mc_" + nameT + ", \"" + nameT + "\", MapType.OTHER);\n";
		result += "return " + nameT + ";\n";
		
		Debug.Log(result);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
}
