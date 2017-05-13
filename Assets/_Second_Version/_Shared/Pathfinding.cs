using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Pathfinding : MonoBehaviour {

    [SerializeField] float distanceRemainingThreshold;

    //[HideInInspector] public NavMeshAgent m_NavMeshAgent;
    [HideInInspector] public NavMeshAgent m_NavMeshAgent { get { return GetComponent<NavMeshAgent>(); } set { m_NavMeshAgent = value; } }

    bool m_isDestinationReached;
    bool isDestinationReached {
        get {
            return m_isDestinationReached;
        } set {
            m_isDestinationReached = value;
            /// If this is true, raise an event
            if(m_isDestinationReached) {
                /// Check if destination was assigned a value by public event System.Action OnDestinationReached
                if (OnDestinationReached != null) {
                    OnDestinationReached();
                }
            }
        }
    }
    public event System.Action OnDestinationReached;

    // Use this for initialization
    void Start () {
        //m_NavMeshAgent = GetComponent<NavMeshAgent>();
	}

    // Update is called once per frame
    /// <summary>
    /// All of these isDestinationReached is the struct, not the primitive bool object m_isDestinationReached
    /// </summary>
    void Update () {
        if (isDestinationReached || !m_NavMeshAgent.hasPath) // 2nd condition will make sure that there's no path set for the m_NavMeshAgent yet.
            return;

        if (m_NavMeshAgent.remainingDistance < distanceRemainingThreshold)
            isDestinationReached = true;

	}

    public void SetTarget(Vector3 target) {
        m_isDestinationReached = false;

        if (m_NavMeshAgent.isActiveAndEnabled)
            m_NavMeshAgent.SetDestination(target);
    }

}