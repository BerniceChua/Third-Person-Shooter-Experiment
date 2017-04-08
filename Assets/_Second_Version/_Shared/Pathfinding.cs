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
                /// Check if destination was assigned a value
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
	void Update () {
        if (m_isDestinationReached)
            return;

        if(m_NavMeshAgent.remainingDistance < distanceRemainingThreshold) {

        }
	}

    public void SetTarget(Vector3 target) {
        m_NavMeshAgent.SetDestination(target);
    }

}