using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour {
    //Animator m_animator;
    Animator m_animator { get { return GetComponentInChildren<Animator>(); } set { m_animator = value; } }
    
    private PlayerAim m_playerAim;
    private PlayerAim PlayerAim {
        get {
            if (m_playerAim == null)
                m_playerAim = GameManager.GameManagerInstance.LocalPlayer.m_PlayerAim;

            return m_playerAim;
        }
    }

    private void Awake() {
        //m_animator = GetComponentInChildren<Animator>();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (GameManager.GameManagerInstance.m_PlayerIsPaused)
            return;

        m_animator.SetFloat("Vertical", GameManager.GameManagerInstance.InputController.m_Vertical);
        m_animator.SetFloat("Horizontal", GameManager.GameManagerInstance.InputController.m_Horizontal);

        //m_animator.SetBool("IsWalking", GameManager.GameManagerInstance.InputController.m_IsWalking);
        m_animator.SetBool("IsWalking", GameManager.GameManagerInstance.LocalPlayer.PlayerState.m_MoveState == PlayerStateMachine.EMoveState.WALKING);
        m_animator.SetBool("IsSprinting", GameManager.GameManagerInstance.InputController.m_IsSprinting);
        m_animator.SetBool("IsCrouched", GameManager.GameManagerInstance.InputController.m_IsCrouched);

        //Debug.Log("PlayerAim.GetAngle() = " + PlayerAim.GetAngle() + " at " + Time.time);
        m_animator.SetFloat("AimAngle", PlayerAim.GetAngle());

        m_animator.SetBool("IsAiming", GameManager.GameManagerInstance.LocalPlayer.PlayerState.m_WeaponState == PlayerStateMachine.EWeaponState.AIMING || GameManager.GameManagerInstance.LocalPlayer.PlayerState.m_WeaponState == PlayerStateMachine.EWeaponState.AIMEDFIRING);

        m_animator.SetBool("IsInCover", GameManager.GameManagerInstance.LocalPlayer.PlayerState.m_MoveState == PlayerStateMachine.EMoveState.COVER);

        //if (GameManager.GameManagerInstance.LocalPlayer.PlayerState.m_MoveState == PlayerStateMachine.EMoveState.COVER) {
        //    //print("This is Snake.  I'm in position.  Kept you waiting, huh?");
        //    m_animator.SetLayerWeight(3, 1);
        //} else {
        //    m_animator.SetLayerWeight(3, 0);
        //}
        while (m_animator.GetBool("IsInCover"))
            m_animator.SetLayerWeight(3, 1);

    }
}
