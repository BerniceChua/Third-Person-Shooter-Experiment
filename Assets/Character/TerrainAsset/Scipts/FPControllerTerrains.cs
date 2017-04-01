using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.CrossPlatformInput;
using TerrainAsset;
using Random = UnityEngine.Random;

namespace UnityStandardAssets.Characters.FirstPerson
{
    [RequireComponent(typeof (Rigidbody))]
    [RequireComponent(typeof (CapsuleCollider))]
    [RequireComponent(typeof (AudioSource))]
    [RequireComponent(typeof (ConfigsOfPlayer))]
    public class FPControllerTerrains : MonoBehaviour
    {
        //local project variables (not included in library)//
        public AudioClip breakBone;
        public AudioClip splash;
        public AudioClip bubblesSound;
        private AudioSource m_AudioSource;
        public ConfigsOfPlayer playerStatsScript;

        [Serializable]
        public class MovementSettings
        {
            //MOVEMENT
            public bool isSwimming = false;
            public bool onTerrain = true;
            public ConfigsOfPlayer.Stats playerStats;
            public KeyCode RunKey = KeyCode.LeftShift;
            public float JumpForce = 30f;
            public AnimationCurve SlopeCurveModifier = new AnimationCurve(new Keyframe(-90.0f, 1.0f), new Keyframe(0.0f, 1.0f), new Keyframe(90.0f, 0.0f));
            public float CurrentTargetSpeed = 8f;

#if !MOBILE_INPUT
            private bool m_Running;
#endif

            public void UpdateDesiredTargetSpeed(Vector2 input)
            {
	            if (input == Vector2.zero) return;
				if (input.x > 0 || input.x < 0)
				{
					//strafe
					CurrentTargetSpeed = playerStats.StrafeSpeed;
				}
				if (input.y < 0)
				{
					//backwards
					CurrentTargetSpeed = playerStats.BackwardSpeed;
				}
				if (input.y > 0)
				{
					//forwards
					//handled last as if strafing and moving forward at the same time forwards speed should take precedence
					CurrentTargetSpeed = playerStats.ForwardSpeed;
				}
#if !MOBILE_INPUT
	            if (Input.GetKey(RunKey))
	            {
		            CurrentTargetSpeed *= playerStats.RunMultiplier;
		            m_Running = true;
	            }
	            else
	            {
		            m_Running = false;
	            }
#endif
            }

#if !MOBILE_INPUT
            public bool Running
            {
                get { return m_Running; }
            }
#endif
        }

        [Serializable]
        public class AdvancedSettings
        {
            public float groundCheckDistance = 0.01f; // distance for checking if the controller is grounded ( 0.01f seems to work best for this )
            public float stickToGroundHelperDistance = 0.5f; // stops the character
            public float slowDownRate = 20f; // rate at which the controller comes to a stop when there is no input
            public bool airControl; // can the user control the direction that is being moved in the air
            [Tooltip("set it to 0.1 or more if you get stuck in wall")]
            public float shellOffset; //reduce the radius by that ratio to avoid getting stuck in wall (a value of 0.1f is nice)
        }

        
        public Camera cam;
        public MovementSettings movementSettings = new MovementSettings();
        public MouseLook mouseLook = new MouseLook();
        public AdvancedSettings advancedSettings = new AdvancedSettings();

        //advanced control variables
        private ConfigsOfPlayer.Stats defaultStats;
        private float onAirTime;
        public float footstepCounter;
        private float footstepTimeLimit;
        private Rigidbody m_RigidBody;
        private CapsuleCollider m_Capsule;
        private float m_YRotation;
        private Vector3 m_GroundContactNormal;
        private bool m_Jump, m_PreviouslyGrounded, m_Jumping, m_IsGrounded;
        
        public Vector3 Velocity
        {
            get { return m_RigidBody.velocity; }
        }

        public bool Grounded
        {
            get { return m_IsGrounded; }
        }

        public bool Jumping
        {
            get { return m_Jumping; }
        }

        public bool Running
        {
            get
            {
 #if !MOBILE_INPUT
				return movementSettings.Running;
#else
	            return false;
#endif
            }
        }


        private void Start()
        {
            onAirTime = 0.0f;
            footstepCounter = 0.0f;
            m_RigidBody = GetComponent<Rigidbody>();
            m_Capsule = GetComponent<CapsuleCollider>();
            mouseLook.Init (transform, cam.transform);
            m_AudioSource = GetComponent<AudioSource>();
            playerStatsScript = GetComponent<ConfigsOfPlayer>();

            //PlayerStats to player attributes
            RenderSettings.fog = true;
            RenderSettings.fogDensity = 0.0001f;
            movementSettings.playerStats = playerStatsScript.defaultStats;
        }


        private void Update()
        {
            RotateView();

            if (CrossPlatformInputManager.GetButtonDown("Jump") && !m_Jump)
            {
                m_Jump = true;
            }
            PlayFootStepAudio();
        }

        void OnTriggerEnter(Collider col)
        {
            if (col.tag == "Water")
                triggerWater(true);
        }

        void OnTriggerExit(Collider col)
        {
            if (col.tag == "Water")
                triggerWater(false);
        }

        //water settings (trigger with tag "Water")
        private void triggerWater(bool entered)
        {
            if (entered)
            {
                m_AudioSource.PlayOneShot(splash);
                movementSettings.isSwimming = true;
                m_RigidBody.useGravity = false;
                m_RigidBody.drag = 1;
                RenderSettings.fog = true;
                RenderSettings.fogColor = new Color(0.3f, 0.5f, 0.65f);
                RenderSettings.fogDensity = 0.015f;
            }
            else
            {
                movementSettings.isSwimming = false;
                m_RigidBody.useGravity = true;
                RenderSettings.fog = false;
            }
        }

        private void FixedUpdate()
        {
            if (!movementSettings.isSwimming)
                groundMovement();
            else
                swimmingMovement();
        }

        private void groundMovement()
        {
            GroundCheck();
            Vector2 input = GetInput();

            if ((Mathf.Abs(input.x) > float.Epsilon || Mathf.Abs(input.y) > float.Epsilon) && (advancedSettings.airControl || m_IsGrounded))
            {
                if (playerStatsScript.UseReducedSpeed)
                {
                    movementSettings.CurrentTargetSpeed /= playerStatsScript.ReduceSpeedFactor;
                }
                // always move along the camera forward as it is the direction that it being aimed at
                Vector3 desiredMove = cam.transform.forward * input.y + cam.transform.right * input.x;
                desiredMove = Vector3.ProjectOnPlane(desiredMove, m_GroundContactNormal).normalized;

                desiredMove.x = desiredMove.x * movementSettings.CurrentTargetSpeed;
                desiredMove.z = desiredMove.z * movementSettings.CurrentTargetSpeed;
                desiredMove.y = desiredMove.y * movementSettings.CurrentTargetSpeed;
                if (m_RigidBody.velocity.sqrMagnitude <
                    (movementSettings.CurrentTargetSpeed * movementSettings.CurrentTargetSpeed))
                {
                    m_RigidBody.AddForce(desiredMove * SlopeMultiplier(), ForceMode.Impulse);
                }
            }

            if (m_IsGrounded)
            {
                m_RigidBody.drag = 5f;

                if (m_Jump)
                {
                    m_RigidBody.drag = 0f;
                    m_RigidBody.velocity = new Vector3(m_RigidBody.velocity.x, 0f, m_RigidBody.velocity.z);
                    m_RigidBody.AddForce(new Vector3(0f, movementSettings.JumpForce, 0f), ForceMode.Impulse);
                    m_Jumping = true;
                    PlayJumpSound();
                }

                if (!m_Jumping && Mathf.Abs(input.x) < float.Epsilon && Mathf.Abs(input.y) < float.Epsilon && m_RigidBody.velocity.magnitude < 1f)
                {
                    m_RigidBody.Sleep();
                }
            }
            else
            {
                onAirTime += Time.deltaTime;
                m_RigidBody.drag = 0f;
                if (m_PreviouslyGrounded && !m_Jumping)
                {
                    StickToGroundHelper();
                }
            }
            m_Jump = false;
        }

        private void swimmingMovement()
        {
            Vector2 input = GetInput();
            Vector3 desiredMove = cam.transform.forward * input.y + cam.transform.right * input.x;
            m_RigidBody.AddForce(desiredMove, ForceMode.Impulse);
        }

        private float SlopeMultiplier()
        {
            float angle = Vector3.Angle(m_GroundContactNormal, Vector3.up);
            return movementSettings.SlopeCurveModifier.Evaluate(angle);
        }
        
        private void StickToGroundHelper()
        {
            RaycastHit hitInfo;
            if (Physics.SphereCast(transform.position, m_Capsule.radius * (1.0f - advancedSettings.shellOffset), Vector3.down, out hitInfo,
                                   ((m_Capsule.height/2f) - m_Capsule.radius) +
                                   advancedSettings.stickToGroundHelperDistance, ~0, QueryTriggerInteraction.Ignore))
            {
                if (Mathf.Abs(Vector3.Angle(hitInfo.normal, Vector3.up)) < 85f)
                {
                    m_RigidBody.velocity = Vector3.ProjectOnPlane(m_RigidBody.velocity, hitInfo.normal);
                }
            }
        }


        private Vector2 GetInput()
        {
            
            Vector2 input = new Vector2
                {
                    x = CrossPlatformInputManager.GetAxis("Horizontal"),
                    y = CrossPlatformInputManager.GetAxis("Vertical")
                };
			movementSettings.UpdateDesiredTargetSpeed(input);
            return input;
        }

        private void RotateView()
        {
            //avoids the mouse looking if the game is paused
            if (Mathf.Abs(Time.timeScale) < float.Epsilon) return;

            // get the rotation before it's changed
            float oldYRotation = transform.eulerAngles.y;
            mouseLook.LookRotation (transform, cam.transform);

            if (m_IsGrounded || advancedSettings.airControl)
            {
                Quaternion velRotation = Quaternion.AngleAxis(transform.eulerAngles.y - oldYRotation, Vector3.up);
                m_RigidBody.velocity = velRotation*m_RigidBody.velocity;
            }
        }

        // sphere cast down just beyond the bottom of the capsule to see if the capsule is colliding and on terrain
        private void GroundCheck()
        {
            m_PreviouslyGrounded = m_IsGrounded;
            RaycastHit hitInfo;
            if (Physics.SphereCast(transform.position, m_Capsule.radius * (1.0f - advancedSettings.shellOffset), Vector3.down, out hitInfo,
                                   ((m_Capsule.height/2f) - m_Capsule.radius) + advancedSettings.groundCheckDistance, ~0, QueryTriggerInteraction.Ignore))
            {
                if (hitInfo.transform.gameObject.name == "Terrain" && !movementSettings.isSwimming)
                    movementSettings.playerStats = playerStatsScript.changeTerrain();
                else
                    movementSettings.playerStats = playerStatsScript.getDefaultStats();

                m_IsGrounded = true;
                m_GroundContactNormal = hitInfo.normal;
            }
            else
            {
                m_IsGrounded = false;
                m_GroundContactNormal = Vector3.up;
            }
            if (!m_PreviouslyGrounded && m_IsGrounded )
            {
                if(m_Jumping)
                    m_Jumping = false;

                if (onAirTime >= 0.6f)
                {
                    PlayLandingSound();
                }
                onAirTime = 0.0f;
                if (Velocity.y > -15.0f)
                {
                    return;
                }
                if (Velocity.y > -18.0f)
                {
                    MediumFallDamage();
                }
                else if(Velocity.y > -22.0f)
                {
                    HighFallDamage();
                }
                else
                {
                    //insta death
                    Death("FALL");
                }
            }
        }

        private void MediumFallDamage()
        {
            //implementation of medium fall damage
        }

        private void HighFallDamage()
        {
            breakLeg();
            //implementation of high fall damage
        }

        private void Death(string sourceOfDeath)
        {
            breakLeg();
        }
        
        private void breakLeg()
        {
            m_AudioSource.PlayOneShot(breakBone);
            playerStatsScript.UseReducedSpeed = true;
        }

        private void PlayFootStepAudio()
        {
            footstepCounter += Time.deltaTime;
            if (!m_IsGrounded && !movementSettings.isSwimming)
                return;
            else if(movementSettings.isSwimming)
            {
                if(footstepCounter > 3.5f)
                {
                    m_AudioSource.PlayOneShot(bubblesSound);
                    footstepCounter = 0.0f;
                }
                return;
            }

            footstepTimeLimit = movementSettings.playerStats.stepLength;
            if (Running)
                footstepTimeLimit = movementSettings.playerStats.stepLength / movementSettings.playerStats.RunMultiplier;

            if (playerStatsScript.UseReducedSpeed)
                footstepTimeLimit *= playerStatsScript.ReduceSpeedFactor;

            if (footstepCounter >= footstepTimeLimit && m_RigidBody.velocity.magnitude > 0.3)
            {
                int n = Random.Range(1, movementSettings.playerStats.Footsteps.Length);
                m_AudioSource.clip = movementSettings.playerStats.Footsteps[n];
                m_AudioSource.PlayOneShot(m_AudioSource.clip);
                movementSettings.playerStats.Footsteps[n] = movementSettings.playerStats.Footsteps[0];
                movementSettings.playerStats.Footsteps[0] = m_AudioSource.clip;
                footstepCounter = 0.0f;
            }
        }

        private void PlayLandingSound()
        {
            m_AudioSource.clip = movementSettings.playerStats.LandSound;
            m_AudioSource.Play();
        }

        private void PlayJumpSound()
        {
            m_AudioSource.clip = movementSettings.playerStats.JumpSound;
            m_AudioSource.Play();
        }
    }
}

