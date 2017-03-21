using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    InputController m_inputController { get { return GameManager.GameManagerInstance.InputController; } set { m_inputController = value; } }
    //InputController m_inputController;

    // Use this for initialization
    void Start () {
        //m_inputController = GameManager.GameManagerInstance.InputController;
    }
	
	// Update is called once per frame
	void Update () {
        print("Horizontal = " + m_inputController.m_horizontal);
        print("Vertical = " + m_inputController.m_vertical);
    }
}
