using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnitySampleAssets._2D;

public class GameController : MonoBehaviour {

	public int maxHealth;
	public float deadDelayTime = 3f;
    public float startDelayTime = 5f;
	public GUIText deadText;
	public GUIText infoText;
    public GameObject CountDownPrefab;
    private GameObject countDownObject;
    public GameObject PlayerPrefab;
    private List<GameObject> Players = new List<GameObject>();

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
		infoText.text = "Game Over! Press 'R' for restart!";
		infoText.enabled = false;

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
        numberOfPlayers = 0;
        infoText.text = "Select number of players (2, 3 or 4)";
        infoText.enabled = true;
        startTime = Time.time + startDelayTime;
        
	}
	
	// Update is called once per frame
	void Update () {
        if (waitForStart)
        {
            cameraController.pauseMovement();

            if (Input.GetKey(KeyCode.Alpha2))
                numberOfPlayers = 2;
            else if (Input.GetKey(KeyCode.Alpha3))
                numberOfPlayers = 3;
            else if (Input.GetKey(KeyCode.Alpha4))
                numberOfPlayers = 4;

            // Number of players selected, create countdown and players, remove text
            if (countDownObject == null && numberOfPlayers > 0)
            {
                countDownObject = (GameObject)Instantiate(CountDownPrefab);

                var playerColors = new Color[] { Color.green, Color.red, Color.blue, Color.yellow };
                // Create players
                for (int i = 0; i < numberOfPlayers; i++)
                {                     
                    var newPlayer = (GameObject)Instantiate(PlayerPrefab); 
                    newPlayer.GetComponent<Platformer2DUserControl>().PlayerID = "P" + (i + 1).ToString();
                    newPlayer.GetComponent<PlatformerCharacter2D>().fuelText = GameObject.Find("Player" + (i + 1).ToString() + " Fuel").guiText;
                    newPlayer.GetComponent<PlatformerCharacter2D>().healthText = GameObject.Find("Player" + (i + 1).ToString() + " Health").guiText;
                    newPlayer.GetComponentInChildren<SpriteRenderer>().color = playerColors[i];
                    Players.Add(newPlayer);
                }
                // Clear unneeded text
                for (int i = numberOfPlayers; i < 4; i++)
                {
                    GameObject.Find("Player" + (i + 1).ToString() + " Fuel").guiText.text = "";
                    GameObject.Find("Player" + (i + 1).ToString() + " Health").guiText.text = "";
                }
                infoText.text = "Game Over! Press 'R' for restart!";
                infoText.enabled = false;
            }

            if (countDownObject != null && !countDownObject.GetComponent<CountDown>().Countdown)
            {
                cameraController.resumeMovement();
                waitForStart = false;
                IsStarted = true;
            }
        }
        else if (gameOver)
        {
            resetDeadText();
            infoText.enabled = true;


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
