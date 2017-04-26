using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine : MonoBehaviour {

    public enum EEnemyStates {
        AWARE,
        UNAWARE
    }

    EEnemyStates m_currentMode;

    /// <summary>
    /// The 2 lines below are equivalent of public event System.Action<EEnemyStates> OnModeChanged;
    /// </summary>
    /// <returns></returns>
    //public delegate EEnemyStates OnModeChangedDelegate();
    //public event OnModeChangedDelegate OnModeChanged;

    public event System.Action<EEnemyStates> OnModeChanged;

	// Use this for initialization
	void Start () {
        m_currentMode = EEnemyStates.UNAWARE;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ChangedMode(EEnemyStates mode) {
        /// check if mode is the same as what's currently assigned.

        if (mode == m_currentMode)
            return;

        m_currentMode = mode;

        if (OnModeChanged != null)
            OnModeChanged(mode);
    }
}