﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollTEST : Destructable {

    [SerializeField] SpawnPoint[] m_spawnPoints;

    public Animator m_Animator;

    Rigidbody[] m_bodyParts;
    MoveController m_moveController { get { return GetComponent<MoveController>(); } set { m_moveController = value; } }

	// Use this for initialization
	void Start () {
        m_bodyParts = transform.GetComponentsInChildren<Rigidbody>();
        //print("m_bodyParts.Length = " + m_bodyParts.Length);
        //if (m_bodyParts.Length == 0)
        //    return;

        EnableRagdoll(false);
        //m_moveController = GetComponent<MoveController>();

    }
	
	// Update is called once per frame
	void Update () {
        if (!IsAlive) {
            return;
        }

        m_Animator.SetFloat("Vertical", 1);
        m_moveController.Move(new Vector2(5, 0));
	}

    void SpawnAtNewSpawnPoint() {
        print("Inside SpawnAtNewSpawnPoint()...");
        int spawnIndex = Random.Range(0, m_spawnPoints.Length);
        transform.position = m_spawnPoints[spawnIndex].transform.position;
        transform.rotation = m_spawnPoints[spawnIndex].transform.rotation;
    }

    public override void Die() {
        base.Die();
        print("Inside public override void Die()");
        EnableRagdoll(true);
        m_Animator.enabled = false;
        print("m_Animator.enabled = " + m_Animator.enabled);
        print("this.gameObject = " + this.gameObject);
        GameManager.GameManagerInstance.Timer.Add(() => { this.gameObject.SetActive(false); }, 5);

        GameManager.GameManagerInstance.Timer.Add(() => {
            EnableRagdoll(false);
            m_Animator.enabled = true;
            Reset();
            SpawnAtNewSpawnPoint();
            //this.gameObject.SetActive(true); 
            /// This was replaced with the line below, or else 
            /// the spawned character kept on spawning at each of the spawnpoints.
            /// It doesn't really solve the issue, it just hides that the glitchy appearance is there.
            GameManager.GameManagerInstance.Timer.Add(() => { this.gameObject.SetActive(true); }, 5);
        }, 5);
    }

    void EnableRagdoll(bool value) {
        print("Inside EnableRagdoll(" + value + ")");
        //print("value = " + value);
        for (int i = 0; i < m_bodyParts.Length; i++) {
            m_bodyParts[i].isKinematic = !value;
            //print("m_bodyParts[" + i + "].isKinematic = " + m_bodyParts[i].isKinematic);
            //print("m_bodyParts[" + i + "].name = " + m_bodyParts[i].name);
        }
    }

}