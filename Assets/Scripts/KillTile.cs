using UnityEngine;
using System.Collections;

public class KillTile : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D other){
		if (other.tag == "Player") {
			// TODO: Add Game Controller.triggerPickup
			//GameController.setDead (other);
		}
	}

}
