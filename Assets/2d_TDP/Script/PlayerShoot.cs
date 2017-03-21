using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour {

    [SerializeField] Shooter m_assaultRifle;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (GameManager.GameManagerInstance.InputController.m_fire1) {
            m_assaultRifle.Fire();
        }
	}
}
