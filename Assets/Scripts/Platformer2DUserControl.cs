using UnityEngine;
namespace UnitySampleAssets._2D
{

    [RequireComponent(typeof (PlatformerCharacter2D))]
    public class Platformer2DUserControl : MonoBehaviour
    {
        private PlatformerCharacter2D character;
        public string PlayerID = "P1";
        private bool jump;

        
        void Start()
        {
        }

        private void Awake()
        {
            character = GetComponent<PlatformerCharacter2D>();           
        }

        private void Update()
        {
            if(!jump)
            // Read the jump input in Update so button presses aren't missed.
                jump = Input.GetButton(PlayerID + "Jump");
            
            if (Input.GetButton(PlayerID + "Fire"))
            {
                character.Fire();
            }
        }

        private void FixedUpdate()
        {
            // Read the inputs.
            bool crouch = Input.GetButton(PlayerID + "Crouch");
            float h = Input.GetAxis(PlayerID + "Horizontal");
            // Pass all parameters to the character control script.
            character.Move(h * Time.deltaTime, crouch, jump);
            jump = false;
        }
    }
}