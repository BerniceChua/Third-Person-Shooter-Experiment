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

    private bool m_isCrouched;
    /// <summary>
    /// Made this way, so that when setting the crouch, it will be able to get out of the crouch after a certain amount of time after target is no longer detected.
    /// </summary>
    public bool IsCrouched {
        get {
            return m_isCrouched;
        } internal set {
            m_isCrouched = true;

            /// It will be crouched at max 25 seconds after checking that target is no longer detected.
            GameManager.GameManagerInstance.Timer.Add(CheckIfItsSafeToStandUp, 25);
        }
    }

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
        m_animator.SetBool("IsWalking", m_enemyPlayer.EnemyStateMachine.CurrentMode == EnemyStateMachine.EEnemyStates.UNAWARE);

        /// Divide velocity by m_pathfinding.m_NavMeshAgent.speed, so that the animation won't do a weird turn.
        m_animator.SetFloat("Vertical", velocity/m_pathfinding.m_NavMeshAgent.speed);

        /// if target is found, switch animation to "IsAiming".
        m_animator.SetBool("IsAiming", m_enemyPlayer.EnemyStateMachine.CurrentMode == EnemyStateMachine.EEnemyStates.AWARE);

        m_animator.SetBool("IsCrouched", IsCrouched);
	}

    void CheckIfItsSafeToStandUp() {
        bool isUnaware = m_enemyPlayer.EnemyStateMachine.CurrentMode == EnemyStateMachine.EEnemyStates.UNAWARE;

        if (isUnaware)
            IsCrouched = false;
    }

}
