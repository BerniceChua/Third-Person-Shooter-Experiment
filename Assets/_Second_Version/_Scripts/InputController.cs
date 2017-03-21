using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour {

    public float m_vertical { get { return Input.GetAxis("Vertical"); } set { m_vertical = value; } }
    public float m_horizontal { get { return Input.GetAxis("Horizontal"); } set { m_horizontal = value; } }

    // this does not work, gives " error CS0119: Expression denotes a `type', where a `variable', `value' or `method group' was expected"
    //public Vector2 m_mouseInput { get { return Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")); } set { m_mouseInput = value; } }

    //public float m_vertical;
    //public float m_horizontal;
    public Vector2 m_mouseInput;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        //m_vertical = Input.GetAxis("Vertical");
        //m_horizontal = Input.GetAxis("Horizontal");
        m_mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
    }
}
