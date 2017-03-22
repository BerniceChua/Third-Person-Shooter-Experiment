using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour {
    //Animator m_animator;
    Animator m_animator { get { return GetComponentInChildren<Animator>(); } set { m_animator = value; } }

    private void Awake() {
        //m_animator = GetComponentInChildren<Animator>();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        m_animator.SetFloat("Vertical", GameManager.GameManagerInstance.InputController.m_vertical);
        m_animator.SetFloat("Horizontal", GameManager.GameManagerInstance.InputController.m_horizontal);
    }
}
