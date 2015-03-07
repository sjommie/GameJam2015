using UnityEngine;
using System.Collections;

public class BulletScript : MonoBehaviour
{
    public float fireDelay = 0.1f;
    [SerializeField]
    private float speed = 20;
    public float Speed { get { return speed; } set { speed = value; rigidbody2D.velocity = new Vector3(speed, Random.Range(-0.2f, 0.2f)); } }
    //public bool GoingRight;
    // Use this for initialization
    Camera cam;

    IEnumerator Start()
    {
        cam = Camera.main;
        ParticleEmitter emitter = GetComponentInChildren<ParticleEmitter>();
        emitter.emit = true;
        yield return new WaitForSeconds(0.2f);
        emitter.emit = false;
    }

    // Update is called once per frame
    void Update()
    {
        if ((new Vector2(cam.transform.position.x, cam.transform.position.y) - rigidbody2D.position).magnitude > 100)
            Destroy(gameObject);
    }


    // Gets called when the object goes out of the screen
    void OnBecameInvisible()
    {
        // Destroy the bullet 
        Destroy(gameObject);
    }

    private bool facingRight = false;
    public bool FacingRight { get { return facingRight; } set { facingRight = value; Speed = Mathf.Abs(Speed) * (facingRight ? 1 : -1); GetComponentInChildren<ParticleEmitter>().localVelocity = new Vector3(10f * (facingRight ? 1 : -1), 0f); } }
    [SerializeField]
    public float KnockBackForce = 500f;
}
