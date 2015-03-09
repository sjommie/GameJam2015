using UnityEngine;

namespace UnitySampleAssets._2D
{
	
	public class PlatformerCharacter2D : MonoBehaviour
	{
		private bool facingRight = true; // For determining which way the player is currently facing.
		
		[SerializeField]
		private float maxSpeed = 1f; // The fastest the player can travel in the x axis.
		[SerializeField]
		private float jumpForce = 5f; // Amount of force added when the player jumps.	
		
		[SerializeField]
		private float maxYSpeed = 10f; // The fastest the player can travel in the x axis.
		[SerializeField]
		private float jetForce = 0.04f; // Amount of force added when the player jumps.	
		[SerializeField]
		private float jetRefuel = 0.04f; // Amount of force added when the player jumps.	
		[SerializeField]
		private float jetDrain = 0.1f; // Amount of force added when the player jumps.	
		
		
		[Range(0, 1)]
		[SerializeField]
		private float crouchSpeed = .36f;
		// Amount of maxSpeed applied to crouching movement. 1 = 100%
		
		[SerializeField]
		private bool airControl = false; // Whether or not a player can steer while jumping;
		[SerializeField]
		private LayerMask whatIsGround; // A mask determining what is ground to the character
		
		private Platformer2DUserControl control;
		
		private Transform groundCheck; // A position marking where to check if the player is grounded.
		private float groundedRadius = .2f; // Radius of the overlap circle to determine if grounded
		private bool grounded = false; // Whether or not the player is grounded.
		private Transform ceilingCheck; // A position marking where to check for ceilings
		private float ceilingRadius = .01f; // Radius of the overlap circle to determine if the player can stand up
		private Animator anim; // Reference to the player's animator component.
		
		Transform playerGraphics;		// Reference to the graphics so we can change direction
		
		public bool IsDead = false;
		float deathTime = 1;
		float timeOfDying = -1000;
		public float JetFuel = 163;
		public float MaxJetFuel = 163;
		private bool jetting;

		GameController gameController;
		int playerHealth;

		public GUIText healthText;
		public GUIText fuelText;

		
		private void Awake()
		{
			// Setting up references.
			control = GetComponent<Platformer2DUserControl>();
			groundCheck = transform.Find("GroundCheck");
			ceilingCheck = transform.Find("CeilingCheck");
			anim = GetComponent<Animator>();
			playerGraphics = transform.FindChild("Graphics");
			if (playerGraphics == null)
			{
				Debug.LogError("KANKER NEE!! THERE IS NO GRPAHICS OBJECT11");
			}
		}
		
		private Camera cam;
		private Plane[] planes;
		void Start()
		{
			cam = Camera.main;
			
			Debug.Log ("Referencing GameController...");
			// Reference to GameController
			GameObject gameControllerObject = GameObject.FindWithTag ("GameController");
			if (gameControllerObject != null) {
				gameController = gameControllerObject.GetComponent <GameController>();
			} else {
				Debug.LogWarning ("Cannot find 'GameController' script");
			}
			
			playerHealth = gameController.maxHealth;
			healthText.text = playerHealth.ToString();
            fuelText.text = JetFuel.ToString("F1");
            rigidbody2D.gravityScale = 0;
		}
		
		public GameObject DeathParticlePrefab;
		public GameObject SpawnParticlePrefab;
		public GameObject BloodParticlePrefab;
		
		private void Update()
		{
            if (gameController.IsStarted)
                rigidbody2D.gravityScale = 3;

            if (gameController.gameOver )
                rigidbody2D.gravityScale = 0;

			if (IsDead)
			{
				if (Time.time - deathTime > timeOfDying)
					ResetPlayer();
				return;
			}
			
			var IDied = false;
			planes = GeometryUtility.CalculateFrustumPlanes(cam);
			if (!GeometryUtility.TestPlanesAABB(planes, new Bounds(rigidbody2D.position, new Vector3(0.1f, 0.1f, 0.1f))))
			{
				IDied = true;
				playerHealth--;
				healthText.text = playerHealth.ToString();
			}

            fuelText.text = JetFuel.ToString("F1");

			// This player died
			if (playerHealth == 0) {
				gameController.setDead(gameObject);			
			}
			
			if (IDied)
			{
				Instantiate(DeathParticlePrefab, transform.position, Quaternion.Euler(new Vector3(0, 0, 0)));
				Instantiate(SpawnParticlePrefab, new Vector3(cam.transform.position.x, cam.transform.position.y, -2), Quaternion.Euler(new Vector3(0, 0, 0)));
				
				rigidbody2D.position = rigidbody2D.position - new Vector2(1000,1000);
				timeOfDying = Time.time;
				IsDead = true;
			}
		}
		
		private void ResetPlayer()
		{
			rigidbody2D.position = new Vector2(cam.transform.position.x, cam.transform.position.y);
			rigidbody2D.velocity = new Vector2(0, 0);
			JetFuel = MaxJetFuel;
			hitTime = -1000;
			timeOfDying = -1000;
			IsDead = false;
		}
		
		float hitTime = -1000;
		float jetDisableTime = 2;
		public void OnCollisionEnter2D(Collision2D coll)
		{
			if (coll.gameObject.name.StartsWith("Bullet") && !coll.gameObject.name.EndsWith(control.PlayerID))
			{
				if (Mathf.Abs(coll.gameObject.rigidbody2D.velocity.magnitude) > 10.0f)
				{
					Instantiate(BloodParticlePrefab, transform.position, Quaternion.Euler(new Vector3(0, 0, 0)));
					hitTime = Time.time;
					//JetFuel = 0;
				}
			}
		}
		
		
		
		private void FixedUpdate()
		{
			if (IsDead)
				return;
			
			// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
			grounded = Physics2D.OverlapCircle(groundCheck.position, groundedRadius, whatIsGround);
			//Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"),
			//                     LayerMask.NameToLayer("OneWayPlatform"),
			//                     !grounded || rigidbody2D.velocity.y > 0
			//                    );
			
			anim.SetBool("Ground", grounded);
			
			// Set the vertical animation
			anim.SetFloat("vSpeed", rigidbody2D.velocity.y);
		}
		
		private bool doubleJump;
		public void Move(float move, bool crouch, bool jump)
		{
			if (IsDead)
				return;
			
			// If crouching, check to see if the character can stand up
			if (!crouch && anim.GetBool("Crouch"))
			{
				// If the character has a ceiling preventing them from standing up, keep them crouching
				if (Physics2D.OverlapCircle(ceilingCheck.position, ceilingRadius, whatIsGround))
					crouch = true;
			}
			
			// Set whether or not the character is crouching in the animator
			anim.SetBool("Crouch", crouch);
			
			//only control the player if grounded or airControl is turned on
			if (grounded || airControl)
			{
				// Reduce the speed if crouching by the crouchSpeed multiplier
				move = (crouch ? move * crouchSpeed : move) * (jetting ? 1.3f : 1);
				
				// The Speed animator parameter is set to the absolute value of the horizontal input.
				anim.SetFloat("Speed", Mathf.Abs(move));
				
				// Move the character
				rigidbody2D.velocity = new Vector2(move * maxSpeed, rigidbody2D.velocity.y);
				
				// If the input is moving the player right and the player is facing left...
				if (move > 0 && !facingRight)
					// ... flip the player.
					Flip();
				// Otherwise if the input is moving the player left and the player is facing right...
				else if (move < 0 && facingRight)
					// ... flip the player.
					Flip();
			}
			
			// If the player should jump...
            if (grounded && jump && anim.GetBool("Ground") && gameController.IsStarted)
			{
				// Add a vertical force to the player.
				grounded = false;
				anim.SetBool("Ground", false);
				rigidbody2D.AddForce(new Vector2(0f, jumpForce));
			}
			
			// Keeping space down = jetting!
			if (jump && JetFuel > 0 && (Time.time - hitTime > jetDisableTime) && gameController.IsStarted)
			{
				jetting = true;
				if (!audio.isPlaying)
					audio.Play();
				
				foreach (Transform child in transform)
				{
					if (child.name.Equals("Jetpack"))
					{
						foreach (var item in child.GetComponentsInChildren<ParticleEmitter>())
						{
							item.emit = true;
						}
						break;
					}
				}
				
				
				JetFuel -= jetDrain;
				JetFuel = Mathf.Max(JetFuel, 0);
				rigidbody2D.AddForce(new Vector2(0f, jetForce));
			}
			else
			{
				jetting = false;
				foreach (Transform child in transform)
				{
					if (child.name.Equals("Jetpack"))
					{
						foreach (var item in child.GetComponentsInChildren<ParticleEmitter>())
						{
							item.emit = false;
						}
						break;
					}
				}
				audio.Stop();
			}
			if (grounded)
			{
				JetFuel += jetRefuel;
				JetFuel = Mathf.Min(JetFuel, MaxJetFuel);
			}
			
			if (rigidbody2D.velocity.y > maxYSpeed)
				rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, maxYSpeed);
			if (rigidbody2D.velocity.y < -2 * maxYSpeed)
				rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, -2 * maxYSpeed);
		}
		
		public GameObject NormalBulletPrefab;
		public GameObject HeavyBulletPrefab;
		public GameObject LaserPrefab;
		
		public float fireDelay = 0.0f;
		public float lastFireTime;
		
		
		public void Fire()
		{
			if (IsDead)
				return;

            if (!gameController.IsStarted)
                return;
			
			if (Time.time - fireDelay >= lastFireTime)
			{
				// Create a new bullet at “transform.position”
				// Which is the current position of the ship
				GameObject bullet;
				bullet = (GameObject)Instantiate(NormalBulletPrefab, transform.position, Quaternion.Euler(new Vector3(0, 0, 0)));
				bullet.name = "Bullet" + control.PlayerID;
				if (facingRight)
				{
					bullet.rigidbody2D.position += new Vector2(0.5f, 0f);
				}
				else
				{
					bullet.rigidbody2D.position += new Vector2(-0.5f, 0f);
				}
				var bulletscript = bullet.GetComponent<BulletScript>();
				bulletscript.FacingRight = facingRight;
				fireDelay = bulletscript.fireDelay;
				lastFireTime = Time.time;
				rigidbody2D.AddForce(new Vector2(bulletscript.KnockBackForce * (facingRight ? -1 : 1), 0));
			}
			
			
		}
		
		private void Flip()
		{
			// Switch the way the player is labelled as facing.
			facingRight = !facingRight;
			
			// Multiply the player's x local scale by -1.
			Vector3 theScale = playerGraphics.localScale;
			theScale.x *= -1;
			playerGraphics.localScale = theScale;
		}
	}
}