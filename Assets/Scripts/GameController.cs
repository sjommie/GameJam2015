using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	public int maxHealth;
	public float deadDelayTime = 3f;
	public GUIText deadText;

	CameraController cameraController;

	float deadTime;
	bool isPaused;

	void Awake() {
		resetDeadText ();

	}

	void resetDeadText() {
		deadText.text = "Died!";
		deadText.enabled = false;
	}

	// Use this for initialization
	void Start () {
		GameObject cameraControllerObject = GameObject.FindWithTag ("MainCamera");
		if (cameraControllerObject != null) {
			cameraController = cameraControllerObject.GetComponent <CameraController>();
		} else {
			Debug.LogWarning ("Cannot find 'CameraController' script");
		}

	}
	
	// Update is called once per frame
	void Update () {
		 
		// Resume camera after delay
		if (isPaused && deadTime + deadDelayTime < Time.time) {
			Debug.Log ("Player dead...");
			cameraController.resumeMovement();
			isPaused = false;
			resetDeadText ();
		}

	}

	public void setDead(GameObject player) {
		deadText.text = player.tag + " " + deadText.text;
		deadText.enabled = true;

		// Pause the camera movement
		cameraController.pauseMovement ();
		isPaused = true;
		deadTime = Time.time;

		GameObject.Destroy (player);
	}
}
