using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Pathfinding))]
[RequireComponent(typeof(EnemyPlayer))]
public class EnemyAnimation : MonoBehaviour {

    [SerializeField] Animator m_animator;

    Vector3 m_lastPosition;
    //Pathfinding m_pathfinding;
    Pathfinding m_pathfinding { get { return GetComponent<Pathfinding>(); } set { m_pathfinding = value; } }

    //EnemyPlayer m_enemyPlayer;
    EnemyPlayer m_enemyPlayer { get { return GetComponent<EnemyPlayer>(); } set { m_enemyPlayer = value; } }

    // Use this for initialization
    void Awake () {
        //m_pathfinding = GetComponent<Pathfinding>();
        //m_enemyPlayer = GetComponent<EnemyPlayer>();
    }

    // Update is called once per frame
    void Update () {
        float velocity = (transform.position - m_lastPosition).magnitude / Time.deltaTime;
        m_lastPosition = transform.position;

        //m_animator.SetBool("IsWalking", true);
        m_animator.SetBool("IsWalking", m_enemyPlayer.EnemyStateMachine.m_CurrentMode == EnemyStateMachine.EEnemyStates.AWARE);

        /// Divide velocity by m_pathfinding.m_NavMeshAgent.speed, so that the animation won't do a weird turn.
        m_animator.SetFloat("Vertical", velocity/m_pathfinding.m_NavMeshAgent.speed);
	}
}
