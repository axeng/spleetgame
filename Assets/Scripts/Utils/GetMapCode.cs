using System.Collections;
using System.Collections.Generic;
#if (UNITY_EDITOR)
using UnityEditor;
#endif
using UnityEngine;

public class GetMapCode : MonoBehaviour {

	// Use this for initialization
	void Start () {
	#if (UNITY_EDITOR)
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
					+ "f), new Vector3(" 
						+ go.transform.rotation.eulerAngles.x
						+"f, "+go.transform.rotation.eulerAngles.y
						+"f, "+go.transform.rotation.eulerAngles.z+"f)));\n";
		}
		
		result += "MapConstructor mc_" + nameT + " = new MapConstructor(bl_" + nameT + ");\n";
		result += "Map " + nameT + " = new Map(mc_" + nameT + ", name, MapType.OTHER, new Vector3(0,0,0));\n";
		result += "return " + nameT + ";\n";
		
		Debug.Log(result);
	#endif
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
}
