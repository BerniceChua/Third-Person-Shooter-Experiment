using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
//[RequireComponent(typeof(MoveController))]
[RequireComponent(typeof(PlayerStateMachine))]
[RequireComponent(typeof(PlayersHealth))]
public class Player : MonoBehaviour {
    /// These were for troubleshooting:
    #region Troubleshooting
    //   InputController m_inputController { get { return GameManager.GameManagerInstance.InputController; } set { m_inputController = value; } }
    //   //InputController m_inputController;

    //   // Use this for initialization
    //   void Start () {
    //       //m_inputController = GameManager.GameManagerInstance.InputController;
    //   }

    //// Update is called once per frame
    //void Update () {
    //       print("Horizontal = " + m_inputController.m_horizontal);
    //       print("Vertical = " + m_inputController.m_vertical);
    //   }
    #endregion

    /// adding damping & sensitivity
    [System.Serializable]
    public class MouseInput {
        public Vector2 Damping;
        public Vector2 Sensitivity;
        public bool LockMouse;
    }

    /// <summary>
    /// Refactored and replaced with scriptable object
    /// </summary>
    //[SerializeField] float m_runSpeed;
    //[SerializeField] float m_walkSpeed;
    //[SerializeField] float m_crouchSpeed;
    //[SerializeField] float m_sprintSpeed;
    [SerializeField] NPCEnemy m_settings;
    
    [SerializeField] MouseInput m_mouseControl;
    [SerializeField] AudioController m_footsteps;
    [SerializeField] float m_minimumMoveThreshold;

    public PlayerAim m_PlayerAim;

    Vector3 m_previousPosition;

    //private MoveController m_moveController;
    //public MoveController MoveController {
    //    get {
    //        //if (!m_moveController)
    //        if (m_moveController == null)
    //            m_moveController = GetComponent<MoveController>();

    //        return m_moveController;
    //    }
    //}
    private CharacterController m_moveController;
    public CharacterController MoveController {
        get {
            //if (!m_moveController)
            if (m_moveController == null)
                m_moveController = GetComponent<CharacterController>();

            return m_moveController;
        }
    }
    private PlayerShoot m_playerShoot;
    public PlayerShoot PlayerShoot {
        get {
            if (m_playerShoot == null)
                m_playerShoot = GetComponent<PlayerShoot>();
            return m_playerShoot;
        }
    }

    /// Removed because of refactor from PlayerStateMachine.
    //private Crosshairs m_crosshair;
    //private Crosshairs Crosshair {
    //    get {
    //        if (!m_crosshair)
    //            m_crosshair = GetComponentInChildren<Crosshairs>();

    //        return m_crosshair;
    //    }
    //}

    private PlayerStateMachine m_playerState;
    public PlayerStateMachine PlayerState {
        get {
            if (!m_playerState)
                m_playerState = GetComponent<PlayerStateMachine>();

            return m_playerState;
        }
    }

    private PlayersHealth m_playersHealth;
    public PlayersHealth PlayersHealth {
        get {
            if (m_playersHealth == null)
                m_playersHealth = GetComponent<PlayersHealth>();

            return m_playersHealth;
        }
    }

    InputController m_playerInput;
    //InputController m_playerInput { get { return GameManager.GameManagerInstance.InputController; } set { m_playerInput = value; } }
    Vector2 m_mouseInput;

    /// <summary>
    /// Changed "Awake()" to "OnEnable()" because the 
    /// order of execution has AmmoCountDisplay.cs first than 
    /// Player.cs, so OnEnable() affects the order of execution by 
    /// setting this up first, and because of 
    /// that, "GameManager.GameManagerInstance.LocalPlayer = this;"
    /// cannot find the Player Game Object before the rest of the game loads,
    /// which causes Player.cs to not to execute which causes Update() to not 
    /// execute either.  That's why the player character couldn't 
    /// change direction with the MouseLookAroundControl().  
    /// (Found this out because the print()s didn't work when
    /// this was Awake() instead of OnEnable().)
    /// </summary>
    //void Awake() {
    void OnEnable() {  // Use this for initialization
        //print("Hello!  I'm inside the Awake() of Player.cs.");
        m_playerInput = GameManager.GameManagerInstance.InputController;
        //print("Hello");
        //print("What is this? " + this);
        print("m_mouseControl.LockMouse = " + m_mouseControl.LockMouse);

        //if (m_mouseControl.LockMouse == false) {
        //    print("Hello!  I'm inside the Awake() of Player.cs.");
        //    Cursor.visible = false;
        //    Cursor.lockState = CursorLockMode.Locked;
        //}

        /// When player joins the game, GameManager will set local player as this player.
        /// Then it will raise the OnLocalPlayerJoined(m_localPlayer) event.
        GameManager.GameManagerInstance.LocalPlayer = this;
        //print(GameManager.GameManagerInstance.LocalPlayer);
        //print("Hello, after 'GameManager.GameManagerInstance.LocalPlayer = this;' ");
        //print("What is this? " + this);
    }

    // Update is called once per frame
    void Update() {
        if (PlayersHealth.IsAlive == false)
            return;

        //print("Inside Update() of Player.cs.");
        Move();

        MouseLookAroundControl();
    }

    void Move() {
        //print("Inside Move() of Player.cs.");
        float moveSpeed = m_settings.m_RunSpeed;
        //print("moveSpeed = " + moveSpeed);
        if (m_playerInput.m_IsWalking)
            moveSpeed = m_settings.m_WalkSpeed;

        if (m_playerInput.m_IsSprinting)
            moveSpeed = m_settings.m_SprintSpeed;

        if (m_playerState.m_MoveState == PlayerStateMachine.EMoveState.COVER)
            moveSpeed = m_settings.m_WalkSpeed;

        if (m_playerInput.m_IsCrouched)
            moveSpeed = m_settings.m_CrouchSpeed;

        //Vector2 direction = new Vector2(m_playerInput.m_Vertical * moveSpeed, m_playerInput.m_Horizontal * moveSpeed);
        Vector2 direction = new Vector2(m_playerInput.m_Vertical * moveSpeed, m_playerInput.m_Horizontal * moveSpeed);
        //MoveController.Move(direction);
        MoveController.Move(transform.forward * direction.x * 0.02f + transform.right * direction.y * 0.02f);

        /// This one is a different way to move your character, and the gravity is calculated for you.
        /// Notice that you'll need to also remove the speed multiplier.
        //MoveController.SimpleMove(transform.forward * direction.x + transform.right * direction.y);

        if (Vector3.Distance(transform.position, m_previousPosition) > m_minimumMoveThreshold /* && direction != Vector2.zero*/) {
            m_footsteps.Play();
        }

        m_previousPosition = transform.position;

    }

    private void MouseLookAroundControl() {
        //Debug.Log("Inside MouseLookAroundControl() at time " + Time.time + "; m_mouseInput.y = " + m_mouseInput.y + ", m_mouseInput.x = " + m_mouseInput.y);
        m_mouseInput.x = Mathf.Lerp(m_mouseInput.x, m_playerInput.m_MouseInput.x, 1.0f / m_mouseControl.Damping.x);
        m_mouseInput.y = Mathf.Lerp(m_mouseInput.y, m_playerInput.m_MouseInput.y, 1.0f / m_mouseControl.Damping.y);

        transform.Rotate(Vector3.up * m_mouseInput.x * m_mouseControl.Sensitivity.x);

        /// Removed because of refactor from PlayerStateMachine.
        //Crosshair.LookHeight(m_mouseInput.y * m_mouseControl.Sensitivity.y);

        m_PlayerAim.m_SetRotation(m_mouseInput.y * m_mouseControl.Sensitivity.y);
    }


}