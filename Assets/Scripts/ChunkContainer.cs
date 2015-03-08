using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChunkContainer : MonoBehaviour {

	public HashSet<Vector2> map = new HashSet<Vector2>();
	public Transform chunk;

	private int chunkSize;
	private float chunkSizeInUnits;
	public int chunkRenderDistance = 3;

	private Camera cam;
	private float camSize;

	LevelImporter levelImporter;
	List<List<string>> levels; 
	
	void Awake(){
		chunkSize = 16;
		chunkSizeInUnits = ChunkGenerator.tileSize * chunkSize;

		// Start level importer
		Debug.Log ("Starting world generator...");
		levelImporter = ScriptableObject.CreateInstance<LevelImporter> ();
		levelImporter.import ();
		
		Debug.Log ("Getting levels from importer...");
		levels = levelImporter.getLevels ();
		
		Debug.Log ("Nr. of levels imported: " + levels.Count);
	}

	// Use this for initialization
	void Start () {
		// Set camera
		cam = Camera.main;
		Debug.Log ("Starting chunk generation...");

		generateChunks ();
	}

	public bool isAvailable(Vector2 loc){
		return !map.Contains (loc);
	}

	void addIfAvailable(Vector2 loc){
		if (isAvailable(loc) == true){
			spawnChunk(loc);
			map.Add (loc);
		}
	}

	void spawnChunk(Vector2 loc){
		Vector3 location = new Vector3 ((loc.x * chunkSize) - chunkSize/2, (loc.y * chunkSize) - chunkSize/2, 0);
		Transform cloneChunk = (Transform) Instantiate (chunk, location, new Quaternion());

		cloneChunk.GetComponent<ChunkGenerator> ().populateChunk(levels);
	}

	// Update is called once per frame
	void Update () {
		generateChunks ();
	}

	void generateChunks() {
		float x = Mathf.Round (cam.transform.position.x / chunkSizeInUnits);
		float y = Mathf.Round (cam.transform.position.y / chunkSizeInUnits);
		
		for (int xOffset = -chunkRenderDistance; xOffset < chunkRenderDistance; xOffset++){
			for (int yOffset = -chunkRenderDistance; yOffset < chunkRenderDistance; yOffset++){
				addIfAvailable (new Vector2 (x + xOffset, y + yOffset));
			}
		}
	} 

}
