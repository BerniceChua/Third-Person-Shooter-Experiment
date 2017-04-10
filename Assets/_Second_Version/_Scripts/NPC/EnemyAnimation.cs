using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Pathfinding))]
public class EnemyAnimation : MonoBehaviour {

    [SerializeField] Animator m_animator;

    Vector3 m_lastPosition;
    //Pathfinding m_pathfinding;
    Pathfinding m_pathfinding { get { return GetComponent<Pathfinding>(); } set { m_pathfinding = value; } }

    // Use this for initialization
    void Awake () {
        //m_pathfinding = GetComponent<Pathfinding>();
	}
	
	// Update is called once per frame
	void Update () {
        float velocity = (transform.position - m_lastPosition).magnitude / Time.deltaTime;
        m_lastPosition = transform.position;

        /// Divide velocity by m_pathfinding.m_NavMeshAgent.speed, so that the animation won't do a weird turn.
        m_animator.SetFloat("Vertical", velocity/m_pathfinding.m_NavMeshAgent.speed);
	}
}
