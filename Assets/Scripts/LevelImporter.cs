using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelImporter : ScriptableObject {
	
	List< List< string>> levels = new List<List<string>>();

	// Use this for initialization
	public void import () {
		Debug.Log ("Reading chunk levels from levels.txt");

		string fileData = System.IO.File.ReadAllText ("Assets/_local/levels.txt");
		string[] lines = fileData.Split("\n"[0]);

		List<string> level = new List<string>();
		foreach (string line in lines) {
			if (line[0].Equals('>')){
				Debug.Log ("Processed one level...");
				levels.Add(level);
				level = new List<string>();
				// this is a separator line between two levels, skip it
				continue;
			}

			level.Add (line);
		}
	}

	public List< List< string>> getLevels(){
		return levels;
	}

}
