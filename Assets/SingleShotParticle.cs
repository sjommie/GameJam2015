using UnityEngine;
using System.Collections;

public class SingleShotParticle : MonoBehaviour {

    float startTime = 0;
	// Use this for initialization
    void Start()
    {
        startTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
        if (Time.time - startTime > 0.2f)
        {
            ParticleEmitter emitter = GetComponentInChildren<ParticleEmitter>();
            emitter.emit = false;
        }
	}
}
