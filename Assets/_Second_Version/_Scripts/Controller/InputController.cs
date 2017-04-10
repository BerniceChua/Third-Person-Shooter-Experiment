using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour {

    //public float m_vertical;
    public float m_Vertical { get { return Input.GetAxis("Vertical"); } /*set { m_Vertical = value; }*/ }
    
    //public float m_horizontal;
    public float m_Horizontal { get { return Input.GetAxis("Horizontal"); } /*set { m_Horizontal = value; }*/ }

    //public Vector2 m_mouseInput;
    // this does not work without "new" in front of "Vector2", gives " error CS0119: Expression denotes a `type', where a `variable', `value' or `method group' was expected"
    public Vector2 m_MouseInput { get { return new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")); } set { m_MouseInput = value; } }

    //public bool m_fire1;
    public bool m_Fire1 { get { return Input.GetButton("Fire1"); } }

    //public bool m_fire2;
    public bool m_Fire2 { get { return Input.GetButton("Fire2"); } }

    //public bool m_reload;
    //public bool m_Reload { get { return Input.GetButtonDown("Reload"); } set { m_Reload = value; } }
    public bool m_Reload { get { return Input.GetButton("Reload"); } set { m_Reload = value; } }

    //public bool m_IsWalking;
    public bool m_IsWalking { get { return Input.GetButton("Walk"); } set { m_IsWalking = value; } }

    //public bool m_IsRunning;
    public bool m_IsRunning { get { return Input.GetButton("Run"); } set { m_IsRunning = value; } }

    //public bool m_IsSprinting;
    public bool m_IsSprinting { get { return Input.GetButton("Sprint"); } set { m_IsSprinting = value; } }

    //public bool m_IsCrouched;
    public bool m_IsCrouched { get { return Input.GetButton("Crouch"); } set { m_IsCrouched = value; } }

    //public bool m_MouseWheelUp;
    public bool m_MouseWheelUp { get { return Input.GetAxis("Mouse ScrollWheel") > 0; } set { m_MouseWheelUp = value; } }

    //public bool m_MouseWheelDown;
    public bool m_MouseWheelDown { get { return Input.GetAxis("Mouse ScrollWheel") < 0; } set { m_MouseWheelDown = value; } }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        //m_Vertical = Input.GetAxis("Vertical");
        //m_Horizontal = Input.GetAxis("Horizontal");
        //m_MouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        //m_Fire1 = Input.GetButton("Fire1");
        //m_Fire2 = Input.GetButton("Fire2");
        //m_Reload = Input.GetButtonDown("Reload");
        //m_IsWalking = Input.GetButton("Walk");
        //m_IsRunning = Input.GetButton("Run");
        //m_IsSprinting = Input.GetButton("Sprint");
        //m_IsCrouched = Input.GetButton("Crouch");
        //m_MouseWheelUp = Input.GetAxis("Mouse ScrollWheel") > 0;
        //m_MouseWheelDown = Input.GetAxis("Mouse ScrollWheel") < 0;
    }

}
