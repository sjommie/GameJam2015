using UnityEngine;
using System.Collections;

public class CountDown : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        RunCountdown();
	}

    public bool Countdown = true;
    private bool CountDownDone = false;
    float CountdownStart = 0;

    void RunCountdown()
    {
        if (CountDownDone)
            return;

        if (Countdown)
        {
            if (CountdownStart == 0)
            {
                this.transform.Find("Counter").audio.Play();
                CountdownStart = Time.time;
            }
            if (Time.time - CountdownStart < 5)
            {
                var timeleft = (int)(CountdownStart + 5 - Time.time);
                if (timeleft == 0)
                {
                    this.transform.Find("Text").guiText.text = "Play";
                    this.transform.Find("Play").audio.Play();
                }
                else
                    this.transform.Find("Text").guiText.text = timeleft.ToString();
            }   
        }
        if (Time.time - CountdownStart > 5)
        {
            Countdown = false;
        }
        if (Time.time - CountdownStart > 6)
        {
            this.transform.Find("Text").guiText.text = "";
            CountDownDone = true;
        }

    }
}
