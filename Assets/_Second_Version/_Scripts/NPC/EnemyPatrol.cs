using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Pathfinding))]
[RequireComponent(typeof(EnemyPlayer))]
public class EnemyPatrol : MonoBehaviour {

    [SerializeField] WaypointController m_waypointController;
    [SerializeField] float m_waitTimeMin;
    [SerializeField] float m_waitTimeMax;

    //Pathfinding m_pathfinding;
    Pathfinding m_pathfinding { get { return GetComponent<Pathfinding>(); } set { m_pathfinding = value; } }

    EnemyPlayer m_enemyPlayer;
    public EnemyPlayer EnemyPlayer {
        get {
            if (m_enemyPlayer == null)
                m_enemyPlayer = GetComponent<EnemyPlayer>();

            return m_enemyPlayer;
        }
    }

    // Use this for initialization
    void Start () {
        m_waypointController.SetNextWaypoint();
	}
	
    private void Awake() {
        //m_pathfinding = GetComponent<Pathfinding>();
        m_pathfinding.OnDestinationReached += Pathfinding_OnDestinationReached;
        m_waypointController.OnWaypointChanged += WaypointController_OnWaypointChanged;

        EnemyPlayer.EnemyHealth.OnDeath += EnemyHealth_OnDeath;
        EnemyPlayer.OnTargetSelected += EnemyPlayer_OnTargetSelected;
    }

    private void EnemyPlayer_OnTargetSelected(Player obj) {
        m_pathfinding.m_NavMeshAgent.Stop();
    }

    private void EnemyHealth_OnDeath() {
        m_pathfinding.m_NavMeshAgent.Stop();
    }

    // Update is called once per frame
    void Update () {
		
	}

    /// <summary>
    /// When we reach the destination, immediately create a new timer until it gets to the new 
    /// waypoint, within the random range.
    /// </summary>
    private void Pathfinding_OnDestinationReached() {
        /// Assume we are patrolling and looping.

        GameManager.GameManagerInstance.Timer.Add(m_waypointController.SetNextWaypoint, Random.Range(m_waitTimeMin, m_waitTimeMax));
    }

    /// <summary>
    /// When the waypoint controller changes the waypoint, we get it back here and it sets the new target as the destination of Pathfinding.
    /// </summary>
    /// <param name="waypoint"></param>
    private void WaypointController_OnWaypointChanged(Waypoint waypoint) {
        //print("Inside WaypointController_OnWaypointChanged(Waypoint waypoint), waypoint = " + waypoint);
        m_pathfinding.SetTarget(waypoint.transform.position);
    }

}