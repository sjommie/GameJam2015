using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChunkGenerator : MonoBehaviour {

	public Transform tileFloor;
	public Transform tileWood;
	public Transform tileGrass;
	public Transform tileIce;
	public Transform tileLava;
	public Transform tileForest;
	public Transform tileBounce;
	
	public float tileSize = 0.32f;

	// Use this for initialization
	void Start () {
	}

	public void populateChunk(List<List<string>> levels){
		List<string> level = levels [Random.Range (0, levels.Count)];

		for (int y = 0; y < 16; y++){
			for (int x = 0; x < 16; x++){

				char tileType = level[y][x];

				Vector2 pos = new Vector2(transform.position.x + x, transform.position.y - y);

				// build the chunk
				switch (tileType)
				{
				case '.':
					// air
					break;
				case 'w':
					// wood
					break;
				case 'i':
					placeTile(pos, tileIce);
					break;
				case 'l':
					// lava
					placeTile(pos, tileLava);
					break;
				case 'F':
					// forest
					break;
				case 'b':
					placeTile(pos, tileBounce);
					break;
				case 'g':
					// floor
					placeTile(pos, tileGrass);
					break;
				default:
					break;
				}
			}
		}
	}

	void placeTile(Vector2 loc, Transform tile){
		Vector3 location = new Vector3 (loc.x * tileSize, loc.y * tileSize, 0);
		Instantiate (tile, location, new Quaternion());
	}

	// Update is called once per frame
	void Update () {
	
	}
}
