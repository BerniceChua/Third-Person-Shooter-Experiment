using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCover : MonoBehaviour {

    [SerializeField] int m_numberOfRays;

    private bool m_canTakeCover;
    private bool m_isInCover;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (!m_canTakeCover)
            return;

        float angleStep = 360 / m_numberOfRays;
        for (int i = 0; i < m_numberOfRays; i++) {
            Quaternion angle = Quaternion.AngleAxis(i * angleStep, transform.up);
            Debug.DrawRay(transform.position + Vector3.up * 0.3f, angle * Vector3.forward * 5.0f, Color.magenta);
        }

        if (Input.GetButtonDown("Get Into Cover")) {

        }
	}

    internal void SetPlayerCoverAllowed(bool value) {
        m_canTakeCover = value;
    }

}
