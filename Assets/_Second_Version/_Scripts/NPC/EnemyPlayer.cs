using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Pathfinding))]
[RequireComponent(typeof(Scanner))]
public class EnemyPlayer : MonoBehaviour {

    [SerializeField]
    Animator m_animator;

    //Pathfinding m_pathfinding;
    Pathfinding m_pathfinding { get { return GetComponent<Pathfinding>(); } set { m_pathfinding = value; } }
    //Scanner m_scanner;
    Scanner m_scanner { get { return GetComponent<Scanner>(); } set { m_scanner = value; } }

    // Use this for initialization
    void Start () {
        //m_pathfinding = GetComponent<Pathfinding>();
        //m_scanner = GetComponent<Scanner>();
        m_scanner.OnTargetSelected += Scanner_OnTargetSelected;
	}

    /// <summary>
    /// When a target is selected, it will set a target to the pathfinding,
    /// and the pathfinding will set the destination to the nav mesh agent, 
    /// and this gameobject will move towards that target.
    /// </summary>
    /// <param name="position"></param>
    private void Scanner_OnTargetSelected(Vector3 position) {
        m_pathfinding.SetTarget(position);
    }

    // Update is called once per frame
    void Update () {
        m_animator.SetFloat("Vertical", m_pathfinding.m_NavMeshAgent.velocity.z);
        m_animator.SetFloat("Horizontal", m_pathfinding.m_NavMeshAgent.velocity.x);
    }

    

}