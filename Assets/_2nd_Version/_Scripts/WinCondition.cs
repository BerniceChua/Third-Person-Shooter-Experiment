using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinCondition : MonoBehaviour {

    [SerializeField] Destructable[] m_targets;

    int m_targetsDestroyedCounter;

	// Use this for initialization
	void Start () {
		for (int i = 0; i < m_targets.Length; i++) {
            m_targets[i].OnDeath += WinCondition_OnDeath;
        }
	}

    private void WinCondition_OnDeath() {
        m_targetsDestroyedCounter++;

        if (m_targetsDestroyedCounter == m_targets.Length) {
            print("Play! Of! The! Game!");
            GameManager.GameManagerInstance.EventBus.RaiseEvent("OnAllEnemiesKilled");
        }

    }

    // Update is called once per frame
    void Update () {
		
	}
}