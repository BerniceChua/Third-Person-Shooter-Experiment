using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine : MonoBehaviour {

    public enum EMoveState {
        WALKING,
        RUNNING,
        CROUCHING,
        SPRINTING,
        COVER
    }

    public enum EWeaponState {
        IDLE,
        FIRING,
        AIMING,
        AIMEDFIRING
    }

    public EMoveState m_MoveState;
    public EWeaponState m_WeaponState;

    bool IsInCover = false;

    private InputController m_inputController;
    public InputController InputController {
        get {
            if (m_inputController == null)
                m_inputController = GameManager.GameManagerInstance.InputController;

            return m_inputController;
        }
    }
    
    // Use this for initialization
    void Awake () {
        GameManager.GameManagerInstance.EventBus.AddListener("CoverToggle", new EventBus.EventListener() {
            Method = ToggleCover
        });
    }

    // Use this for initialization
    void Start() {

    }

    private void ToggleCover() {
        IsInCover = !IsInCover;
    }

    // Update is called once per frame
    void Update () {
        SetMoveState();
        SetWeaponState();
    }

    void SetWeaponState() {
        m_WeaponState = EWeaponState.IDLE;

        if (InputController.m_Fire1)
            m_WeaponState = EWeaponState.FIRING;

        if (InputController.m_Fire2)
            m_WeaponState = EWeaponState.AIMING;

        if (InputController.m_Fire1 && InputController.m_Fire2)
            m_WeaponState = EWeaponState.AIMEDFIRING;
    }

    void SetMoveState() {
        m_MoveState = EMoveState.RUNNING;

        if (InputController.m_IsSprinting)
            m_MoveState = EMoveState.SPRINTING;

        if (InputController.m_IsWalking)
            m_MoveState = EMoveState.WALKING;

        if (InputController.m_IsCrouched)
            m_MoveState = EMoveState.CROUCHING;

        if (IsInCover)
            m_MoveState = EMoveState.COVER;
    }

}