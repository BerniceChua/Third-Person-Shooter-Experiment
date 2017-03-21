using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(MoveController))]
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
    }

    [SerializeField] float m_speed;
    [SerializeField] MouseInput m_mouseControl;

    private MoveController m_moveController;
    public MoveController MoveController {
        get {
            //if (m_moveController == null)
            if (!m_moveController)
                m_moveController = GetComponent<MoveController>();

            return m_moveController;
        }
    }

    InputController m_playerInput { get { return GameManager.GameManagerInstance.InputController; } set { m_playerInput = value; } }
    Vector2 m_mouseInput;

    // Use this for initialization
    void Awake() {
        //m_inputController = GameManager.GameManagerInstance.InputController;

        /// When player joins the game, GameManager will set local player as this player.
        /// Then it will raise the OnLocalPlayerJoined(m_localPlayer) event.
        GameManager.GameManagerInstance.LocalPlayer = this;
    }

    // Update is called once per frame
    void Update() {
        Vector2 direction = new Vector2(m_playerInput.m_horizontal * m_speed, m_playerInput.m_vertical * m_speed);
        MoveController.Move(direction);

        m_mouseInput.x = Mathf.Lerp(m_mouseInput.x, m_playerInput.m_mouseInput.x, 1.0f/m_mouseControl.Damping.x);

        transform.Rotate(Vector3.up * m_mouseInput.x * m_mouseControl.Sensitivity.x);
    }
}
