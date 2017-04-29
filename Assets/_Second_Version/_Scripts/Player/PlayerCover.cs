using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCover : MonoBehaviour {

    private bool m_canTakeCover;
    private bool m_isInCover;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (!m_canTakeCover)
            return;

        if (Input.GetButtonDown("Get Into Cover")) {

        }
	}

    internal void SetPlayerCoverAllowed(bool value) {
        m_canTakeCover = value;
    }

}
