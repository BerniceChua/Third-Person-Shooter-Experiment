﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCover : MonoBehaviour {

    [SerializeField] int m_numberOfRays;
    [SerializeField] LayerMask m_coverMask;

    private bool m_canTakeCover;
    private bool m_isInCover;

    private RaycastHit m_closestHit;

    bool m_isAiming { get { return GameManager.GameManagerInstance.LocalPlayer.PlayerState.m_WeaponState == PlayerStateMachine.EWeaponState.AIMING || GameManager.GameManagerInstance.LocalPlayer.PlayerState.m_WeaponState == PlayerStateMachine.EWeaponState.AIMEDFIRING; } }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (m_isAiming && m_isInCover) {
            ExecuteCoverToggle();
            return;
        }

        if (!m_canTakeCover)
            return;

        //if (Input.GetButton("Get Into Cover")) {
        if (GameManager.GameManagerInstance.InputController.m_CoverToggle)
            TakeCover();

    }

    void TakeCover() {
        FindCoverAroundPlayerWithRaycasts();

        /// if nothing is hit by raycast
        if (m_closestHit.distance == 0)
            return;

        ExecuteCoverToggle();
    }

    void ExecuteCoverToggle() {
        m_isInCover = !m_isInCover;

        GameManager.GameManagerInstance.EventBus.RaiseEvent("CoverToggle");

        transform.rotation = Quaternion.LookRotation(m_closestHit.normal) * Quaternion.Euler(0.0f, 180.0f, 0.0f);
    }

    private void FindCoverAroundPlayerWithRaycasts() {
        /// This resets the raycast each time.
        m_closestHit = new RaycastHit();

        float angleStep = 360 / m_numberOfRays;
        for (int i = 0; i < m_numberOfRays; i++) {
            Quaternion angle = Quaternion.AngleAxis(i * angleStep, transform.up);
            Debug.DrawRay(transform.position + Vector3.up * 0.3f, angle * Vector3.forward * 5.0f, Color.magenta);

            CheckClosestPointWithRaycasts(angle);
        }

        Debug.DrawLine(transform.position + Vector3.up * 0.3f, m_closestHit.point, Color.cyan, 0.5f);
    }

    private void CheckClosestPointWithRaycasts(Quaternion angle) {
        /// find the closest raycast, and align to it.
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 0.3f, angle * Vector3.forward, out hit, 5.0f, m_coverMask)) {
            /// Get the raycast with the shortest distance
            if (m_closestHit.distance == 0 || hit.distance < m_closestHit.distance)
                m_closestHit = hit;

            Debug.DrawLine(transform.position + Vector3.up * 0.3f, hit.point, Color.yellow, 0.5f);
        }
    }

    internal void SetPlayerCoverAllowed(bool value) {
        m_canTakeCover = value;

        if (!m_canTakeCover && m_isInCover)
            ExecuteCoverToggle();
    }

}