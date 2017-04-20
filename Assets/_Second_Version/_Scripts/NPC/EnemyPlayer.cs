using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Pathfinding))]
//[RequireComponent(typeof(Scanner))]
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

	Player m_priorityTarget;
	List<Player> m_targetsList;

    // Use this for initialization
    void Start () {
        //m_pathfinding = GetComponent<Pathfinding>();
        //m_scanner = GetComponent<Scanner>();
        //m_scanner.OnTargetSelected += Scanner_OnTargetSelected;
		m_playerScanner.OnScanReady += GenericScanner_OnScanReady;

		GenericScanner_OnScanReady();
	}

	private void GenericScanner_OnScanReady(){
		if (m_priorityTarget != null)
			return;

		m_targetsList = m_playerScanner.ScanForTargetsWithComponent<Player>();

		if (m_targetsList.Count == 1)
			m_priorityTarget = m_targetsList [0];
		else
			SelectClosestTarget(m_targetsList);

		if (m_priorityTarget != null)
			SetDestinationToPriorityTarget ();
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

	private void SetDestinationToPriorityTarget() {
		m_pathfinding.SetTarget(m_priorityTarget.transform.position);
	}

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

}