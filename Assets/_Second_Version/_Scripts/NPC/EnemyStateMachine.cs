using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine : MonoBehaviour {

    public enum EEnemyStates {
        AWARE,
        UNAWARE
    }

    //public EEnemyStates m_CurrentMode;
    private EEnemyStates m_CurrentMode;
    public EEnemyStates CurrentMode {
        get {
            return m_CurrentMode;
        }

        set {
            if (m_CurrentMode == value)
                return;

            m_CurrentMode = value;

            if (OnModeChanged != null)
                OnModeChanged(m_CurrentMode);
        }
    }

    /// <summary>
    /// The 2 lines below are equivalent of public event System.Action<EEnemyStates> OnModeChanged;
    /// </summary>
    /// <returns></returns>
    //public delegate EEnemyStates OnModeChangedDelegate();
    //public event OnModeChangedDelegate OnModeChanged;

    public event System.Action<EEnemyStates> OnModeChanged;

    //void Start () {
    //    m_CurrentMode = EEnemyStates.UNAWARE;
    //}
    void Awake() {
        m_CurrentMode = EEnemyStates.UNAWARE;
    }

    // Update is called once per frame
    void Update () {
		
	}

    [ContextMenu("Set 'Aware'")]
    void SetToAware() {
        CurrentMode = EEnemyStates.AWARE;
    }

    [ContextMenu("Set 'Unaware'")]
    void SetToUnaware() {
        CurrentMode = EEnemyStates.UNAWARE;
    }

    // Use this for initialization
    /// This was replaced by the get-set as a property that 
    /// happens before Start() is called instead of this method.
    //public void ChangedMode(EEnemyStates mode) {
    //    /// check if mode is the same as what's currently assigned.

    //    if (mode == m_CurrentMode)
    //        return;

    //    m_CurrentMode = mode;

    //    //if (OnModeChanged != null)
    //    //    OnModeChanged(mode);
    //}

}