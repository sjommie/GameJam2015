using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	public int maxHealth;
	public float deadDelayTime = 3f;
    public float startDelayTime = 5f;
	public GUIText deadText;
	public GUIText gameOverText;

	CameraController cameraController;
	public int numberOfPlayers = 3;

	float deadTime;
    float startTime;
	bool isPaused;
	public bool gameOver;
    bool waitForStart;
    public bool IsStarted =false;

	void Awake() {
       	resetDeadText ();
		gameOverText.text = "Game Over! Press 'R' for restart!";
		gameOverText.enabled = false;

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

        waitForStart = true;
        startTime = Time.time + startDelayTime;
        
	}
	
	// Update is called once per frame
	void Update () {
        if (waitForStart)
        {
            cameraController.pauseMovement();
            if (startTime < Time.time)
            {
                cameraController.resumeMovement();
                waitForStart = false;
                IsStarted = true;
            }
        }
        else if (gameOver)
        {
            resetDeadText();
            gameOverText.enabled = true;


            if (Input.GetKeyDown(KeyCode.R))
            {
                Application.LoadLevel(Application.loadedLevel);
            }
        }

        // Resume camera after delay
        else if (isPaused && deadTime + deadDelayTime < Time.time)
        {
            Debug.Log("Player dead...");
            cameraController.resumeMovement();
            isPaused = false;
            resetDeadText();
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
		numberOfPlayers--;

		if (numberOfPlayers == 1) {
			setGameOver();
		}
	}

	void setGameOver() {
		gameOver = true;
	}
}
