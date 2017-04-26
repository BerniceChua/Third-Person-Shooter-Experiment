using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Pathfinding))]
//[RequireComponent(typeof(Scanner))]
[RequireComponent(typeof(EnemyNPCHealth))]
[RequireComponent(typeof(EnemyStateMachine))]
public class EnemyPlayer : MonoBehaviour {

    /// <summary>
    ///  Removed during refactoring for waypoints.
    ///  This functionality has moved to EnemyAnimation.cs
    /// </summary>
    //[SerializeField] Animator m_animator;

    //Pathfinding m_pathfinding;
    Pathfinding m_pathfinding { get { return GetComponent<Pathfinding>(); } set { m_pathfinding = value; } }
    //Scanner m_scanner;
    //Scanner m_scanner { get { return GetComponent<Scanner>(); } set { m_scanner = value; } }
	//[SerializeField] GenericScanner m_playerScanner { get { return GetComponent<GenericScanner>(); } set { m_playerScanner = value; } }
	[SerializeField] GenericScanner m_playerScanner;

    [SerializeField] NPCEnemy m_settings;

	Player m_priorityTarget;
	List<Player> m_targetsList;

    public event System.Action<Player> OnTargetSelected;

    private EnemyNPCHealth m_EnemyHealth;
    public EnemyNPCHealth EnemyHealth {
        get {
            if (m_EnemyHealth == null)
                m_EnemyHealth = GetComponent<EnemyNPCHealth>();

            return m_EnemyHealth;
        }
    }

    private EnemyStateMachine m_EnemyStateMachine;
    public EnemyStateMachine EnemyStateMachine {
        get {
            if (m_EnemyStateMachine == null)
                m_EnemyStateMachine = GetComponent<EnemyStateMachine>();

            return m_EnemyStateMachine;
        }
    }

    // Use this for initialization
    void Start () {
        //m_pathfinding = GetComponent<Pathfinding>();

        m_pathfinding.m_NavMeshAgent.speed = m_settings.m_WalkSpeed;

        //m_scanner = GetComponent<Scanner>();
        //m_scanner.OnTargetSelected += Scanner_OnTargetSelected;
		m_playerScanner.OnScanReady += GenericScanner_OnScanReady;

		GenericScanner_OnScanReady();

        EnemyHealth.OnDeath += EnemyHealth_OnDeath;

        EnemyStateMachine.OnModeChanged += EnemyStateMachine_OnModeChanged;

    }

    private void EnemyStateMachine_OnModeChanged(EnemyStateMachine.EEnemyStates state) {
        m_pathfinding.m_NavMeshAgent.speed = m_settings.m_WalkSpeed;

        if (state == EnemyStateMachine.EEnemyStates.AWARE)
            m_pathfinding.m_NavMeshAgent.speed = m_settings.m_RunSpeed;
    }

    private void EnemyHealth_OnDeath() {
        
    }

    private void GenericScanner_OnScanReady(){
		if (m_priorityTarget != null)
			return;

		m_targetsList = m_playerScanner.ScanForTargetsWithComponent<Player>();

		if (m_targetsList.Count == 1)
			m_priorityTarget = m_targetsList [0];
		else
			SelectClosestTarget(m_targetsList);

		if (m_priorityTarget != null) {
            /// Removed when refactoring for enemy-attack.
            //SetDestinationToPriorityTarget ();

            if (OnTargetSelected != null)
                OnTargetSelected(m_priorityTarget);
        }
    }

    /// <summary>
    /// When a target is selected, it will set a target to the pathfinding,
    /// and the pathfinding will set the destination to the nav mesh agent, 
    /// and this gameobject will move towards that target.
    /// </summary>
    /// <param name="position"></param>
    /*private void Scanner_OnTargetSelected(Vector3 position) {
        m_pathfinding.SetTarget(position);
    }*/

    /// Removed when refactoring for enemy-attack.
    //private void SetDestinationToPriorityTarget() {
    //	m_pathfinding.SetTarget(m_priorityTarget.transform.position);
    //}

    /// <summary>
    ///  Removed during refactoring for waypoints.
    ///  This functionality has moved to EnemyAnimation.cs
    /// </summary>
    //// Update is called once per frame
    //void Update () {
    //    m_animator.SetFloat("Vertical", m_pathfinding.m_NavMeshAgent.velocity.z);
    //    //m_animator.SetFloat("Horizontal", m_pathfinding.m_NavMeshAgent.velocity.x);
    //}

    private void SelectClosestTarget(List<Player> targets) {
		float closestTarget = m_playerScanner.m_ScanRange;

		foreach (var possibleTarget in m_targetsList) {
			if (Vector3.Distance (transform.position, possibleTarget.transform.position) < closestTarget)
				m_priorityTarget = possibleTarget;
		}
	}

    private void Update() {
        if (m_priorityTarget == null)
            return;

        transform.LookAt(m_priorityTarget.transform.position);
        /// The above was my version.  I wonder why the "transform" was repeated in the original?
        //transform.LookAt(m_priorityTarget.transform.transform.position);
    }

}