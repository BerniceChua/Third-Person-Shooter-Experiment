using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointController : MonoBehaviour {

    //Waypoint[] m_waypoints;
    Waypoint[] m_waypoints { get { return GetComponentsInChildren<Waypoint>(); } set { m_waypoints = value; } }

    /// <summary>
    ///  This is set to -1 because when the game starts up, it will add 1 to this index.
    /// </summary>
    int m_currentWaypointIndex = -1;

    /// <summary>
    /// Use this event to return the waypoint instead of a method.
    /// </summary>
    public event System.Action<Waypoint> OnWaypointChanged;

	// Use this for initialization
	void Awake () {
        //m_waypoints = GetComponentsInChildren<Waypoint>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetNextWaypoint() {
        m_currentWaypointIndex++;

        if (m_currentWaypointIndex == m_waypoints.Length)
            m_currentWaypointIndex = 0;

        if (OnWaypointChanged != null)
            OnWaypointChanged(m_waypoints[m_currentWaypointIndex]);
    }

    /// <summary>
    /// This is here, because the waypoints are only called in the 
    /// beginning, during runtime.  So there are no waypoints in 
    /// the editor.
    /// </summary>
    private Waypoint[] GetWaypoints() {
        return GetComponentsInChildren<Waypoint>();
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.blue;

        /// Loops through array of waypoints.
        foreach(Waypoint waypoint in m_waypoints) {
            /// Draw a sphere on the transform of the waypoint.
            Gizmos.DrawSphere(waypoint.transform.position, 0.2f);
        }
    }

}
