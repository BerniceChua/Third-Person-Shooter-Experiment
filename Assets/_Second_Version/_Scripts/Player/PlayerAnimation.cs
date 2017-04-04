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
        m_animator.SetFloat("Vertical", GameManager.GameManagerInstance.InputController.m_Vertical);
        m_animator.SetFloat("Horizontal", GameManager.GameManagerInstance.InputController.m_Horizontal);

        m_animator.SetBool("IsWalking", GameManager.GameManagerInstance.InputController.m_IsWalking);
        m_animator.SetBool("IsSprinting", GameManager.GameManagerInstance.InputController.m_IsSprinting);
        m_animator.SetBool("IsCrouched", GameManager.GameManagerInstance.InputController.m_IsCrouched);

        //Debug.Log("PlayerAim.GetAngle() = " + PlayerAim.GetAngle() + " at " + Time.time);
        m_animator.SetFloat("AimAngle", PlayerAim.GetAngle());

        m_animator.SetBool("IsAiming", GameManager.GameManagerInstance.LocalPlayer.PlayerState.m_WeaponState == PlayerStateMachine.EWeaponState.AIMING || GameManager.GameManagerInstance.LocalPlayer.PlayerState.m_WeaponState == PlayerStateMachine.EWeaponState.AIMEDFIRING);
    }
}
