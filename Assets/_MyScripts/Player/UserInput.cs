﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInput : MonoBehaviour {
    public CharacterMovement m_charMove { get; protected set; }
    public WeaponHandler m_weaponhandler { get; protected set; }

    [System.Serializable]
    public class InputSettings {
        public string verticalAxis = "Vertical";
        public string horizontalAxis = "Horizontal";
        public string jumpButton = "Jump";

        public string reloadButton = "Reload";
        public string aimButton = "Fire2";
        public string fireButton = "Fire1";
        public string dropWeapon = "DropWeapon";
        public string switchWeaponButton = "SwitchWeapon";
    }
    [SerializeField] InputSettings m_input;

    [System.Serializable]
    public class OtherSettings {
        public float lookSpeed = 5.0f;
        public float lookDistance = 10.0f;
        public bool requireInputForTurn = true;

        public LayerMask aimDetectionLayers;
    }
    [SerializeField] public OtherSettings m_other;

    public bool m_debugAim; // helps us to position spine correctly
    public Transform m_spine;
    bool m_aiming;

    //Camera m_mainCam;
    public Camera m_thirdPersonCam;

    // Use this for initialization
    void Start () {
        m_charMove = GetComponent<CharacterMovement>();
        //m_thirdPersonCam = Camera.main;
        m_weaponhandler = GetComponent<WeaponHandler>();
	}
	
	// Update is called once per frame
	void Update () {
        CharacterLogic();
        CameraLookLogic();
        WeaponLogic();
    }

    private void LateUpdate() {
        if (m_weaponhandler) {
            if (m_weaponhandler.m_currentWeapon) {
                if (m_aiming)
                    PositionSpine();
            }
        }
    }

    // handles character logic
    void CharacterLogic() {
        if (!m_charMove) {
            Debug.Log("Warning: no m_charMove.");
            return;
        }

        m_charMove.AnimateChar(Input.GetAxis(m_input.verticalAxis), Input.GetAxis(m_input.horizontalAxis));

        if (Input.GetButtonDown(m_input.jumpButton))
            m_charMove.Jump();
        //m_animator.SetBool(m_animations.jumpBool, m_isJumping);
    }

    // handles camera look logic
    void CameraLookLogic() {
        if (!m_thirdPersonCam) {
            Debug.Log("Warning: no m_mainCam.");
            return;
        }

        if (m_other.requireInputForTurn) {
            if (Input.GetAxis(m_input.horizontalAxis) != 0 || Input.GetAxis(m_input.verticalAxis) != 0)
                CharacterLook();
        } else {
            CharacterLook();
        }
    }

    // make character look at a forward point from the camera
    void CharacterLook() {
        Transform mainCamT = m_thirdPersonCam.transform;
        
        //Vector3 mainCamPos = mainCamT.position;
        Transform pivotT = mainCamT.parent;
        Vector3 pivotPos = pivotT.position;
        
        //Vector3 lookTarget = mainCamPos + (mainCamT.forward * m_other.lookDistance);
        Vector3 lookTarget = pivotPos + (pivotT.forward * m_other.lookDistance);

        Vector3 thisPos = transform.position;
        Vector3 lookDir = lookTarget - thisPos;
        Quaternion lookRot = Quaternion.LookRotation(lookDir);
        lookRot.x = 0;
        lookRot.z = 0;

        Quaternion newRotation = Quaternion.Lerp(transform.rotation, lookRot, Time.deltaTime * m_other.lookSpeed);
        transform.rotation = newRotation;
    }

    void WeaponLogic() {
        if (!m_weaponhandler) {
            Debug.Log("Warning: no m_weaponHandler.");
            return;
        }

        m_aiming = Input.GetButton(m_input.aimButton) || m_debugAim;

        //if (m_weaponhandler.m_currentWeapon) {
            m_weaponhandler.Aim(m_aiming);

            m_other.requireInputForTurn = !m_aiming;

            m_weaponhandler.FingerOnTrigger(Input.GetButton(m_input.fireButton));

            if (Input.GetButtonDown(m_input.reloadButton))
                m_weaponhandler.Reload();

            if (Input.GetButtonDown(m_input.dropWeapon))
                m_weaponhandler.DropCurrentWeapon();

            if (Input.GetButtonDown(m_input.switchWeaponButton))
                m_weaponhandler.SwitchWeapons();
        //}

        if (!m_weaponhandler.m_currentWeapon)
            return;

        m_weaponhandler.m_currentWeapon.m_shootRay = new Ray(m_thirdPersonCam.transform.position, m_thirdPersonCam.transform.forward);
    }


    // positions spine when aiming
    void PositionSpine() {
        if (!m_spine || !m_weaponhandler.m_currentWeapon || !m_thirdPersonCam)
            return;

        //RaycastHit hit;
        Transform mainCamT = m_thirdPersonCam.transform;
        Vector3 mainCamPos = mainCamT.position;
        Vector3 dir = mainCamT.forward;
        Ray ray = new Ray(mainCamPos, dir);

        m_spine.LookAt(ray.GetPoint(50));

        //if (Physics.Raycast(ray, out hit, 100, m_other.aimDetectionLayers)) {
        //    Vector3 hitPoint = hit.point;
        //    m_spine.LookAt(hitPoint);
        //} else {
        //    m_spine.LookAt(ray.GetPoint(50));
        //}

        Vector3 eulerAngleOffset = m_weaponhandler.m_currentWeapon.m_userSettings.spineRotation;
        m_spine.Rotate(eulerAngleOffset);
    }

}