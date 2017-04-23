﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scans for targets within the field of view
/// </summary>
[RequireComponent(typeof(SphereCollider))]
public class GenericScanner : MonoBehaviour {

    [SerializeField] float m_scanSpeed;
    [SerializeField] [Range(0,360)] float m_fieldOfView;
    [SerializeField] LayerMask m_layerMask;

    //SphereCollider m_rangeTrigger;
    //SphereCollider m_rangeTrigger { get { return GetComponent<SphereCollider>(); } set { m_rangeTrigger = value; } }
	SphereCollider m_rangeTrigger;

	public float m_ScanRange {
		get {
			if (m_rangeTrigger == null)
				m_rangeTrigger = GetComponent<SphereCollider> ();

			return m_rangeTrigger.radius;
		}
	}

    /// <summary>
    ///  Detects players
    /// </summary>
    //List<Player> m_targets = new List<Player>();
	/// the above was replaced with a local variable when we made the scanner generic.

    /// <summary>
    /// Use this to select a target from m_targets List<>
    /// </summary>
    /*Player selectedTarget;
    Player m_selectedTarget {
        get { return selectedTarget; }
        set {
            selectedTarget = value;

            if (selectedTarget == null)
                return;

            if (OnTargetSelected != null)
                OnTargetSelected(selectedTarget.transform.position);
        }
    }*/

    /// <summary>
    /// When the scanner has selected a target, we want the EnemyPlayerNPC to know about it.
    /// So we're making an event.
    /// </summary>
    //public event System.Action<Vector3> OnTargetSelected;
	//public event System.Action<Player> OnTargetSelected;
	public event System.Action OnScanReady;
	
	// Use this for initialization
	void Start () {
        //m_rangeTrigger = GetComponent<SphereCollider>();
	}
	
	// Update is called once per frame
	void Update () {
        PrepareScan();
	}

    void PrepareScan() {
        /// Don't scan if currently have a target selected.
        //if (m_selectedTarget != null)
        //    return;
		/// Removed when the scanner was genericized.

        //GameManager.GameManagerInstance.Timer.Add(ScanForTargets, m_scanSpeed);
		GameManager.GameManagerInstance.Timer.Add(() => {
			if (OnScanReady != null)
				OnScanReady();
		}, m_scanSpeed);
    }

    private void OnDrawGizmos() {
		/*Gizmos.color = Color.cyan;

        if (m_selectedTarget != null) {
            Gizmos.DrawLine(transform.position, m_selectedTarget.transform.position);
        }*/

        Gizmos.color = Color.green;

        /// to the right of the normal line coming out of transform.position
        Gizmos.DrawLine(transform.position, transform.position + GetViewAngle(m_fieldOfView/2) * GetComponent<SphereCollider>().radius);
        /// to the left of the normal line coming out of transform.position
        Gizmos.DrawLine(transform.position, transform.position + GetViewAngle(-m_fieldOfView / 2) * GetComponent<SphereCollider>().radius);
    }

    Vector3 GetViewAngle(float angle) {
        float radian = (angle + transform.eulerAngles.y) * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(radian), 0, Mathf.Cos(radian));
    }

    /*void ScanForTargets() {
        //print("Inside ScanForTargets()");
        Collider[] scanResults = Physics.OverlapSphere(transform.position, m_rangeTrigger.radius);

        for (int i = 0; i < scanResults.Length; i++) {
            var player = scanResults[i].transform.GetComponent<Player>();

            if (player == null)
                continue;
            
            if (!IsInLineOfSight(Vector3.up, player.transform.position) )
                continue;

            m_targets.Add(player);
        }

        /// Check how many m_targets are in the List<Player>();
        if (m_targets.Count == 1) {
            m_selectedTarget = m_targets[0];
        } else {
            /// Check for the closest target.
            float closestTarget = m_rangeTrigger.radius;

            foreach(var possibleTarget in m_targets) {
                if (Vector3.Distance(transform.position, possibleTarget.transform.position) < closestTarget)
                    m_selectedTarget = possibleTarget;
            }
        }

        /// After scanning for targets, this will cache the results in PrepareScan().
        PrepareScan();
    }*/
	public List<T> ScanForTargetsWithComponent<T>() {
		List<T> targets = new List<T> ();

		//print("Inside ScanForTargets()");
		//Collider[] scanResults = Physics.OverlapSphere(transform.position, m_rangeTrigger.radius);
		Collider[] scanResults = Physics.OverlapSphere(transform.position, m_ScanRange);

		for (int i = 0; i < scanResults.Length; i++) {
			var player = scanResults[i].transform.GetComponent<T>();

			if (player == null)
				continue;

			if (!IsInLineOfSight(Vector3.up, scanResults[i].transform.position) )
				continue;

			targets.Add(player);
		}

		/// Check how many m_targets are in the List<Player>();
		/*if (targets.Count == 1) {
			m_selectedTarget = targets[0];
		} else {
			/// Check for the closest target.
			float closestTarget = m_rangeTrigger.radius;

			foreach(var possibleTarget in targets) {
				if (Vector3.Distance(transform.position, possibleTarget.transform.position) < closestTarget)
					m_selectedTarget = possibleTarget;
			}
		}*/

		/// After scanning for targets, this will cache the results in PrepareScan().
		PrepareScan();

		return targets;
	}

    bool IsInLineOfSight(Vector3 eyeHeight, Vector3 targetPosition) {
        //print("Inside IsInLineOfSight()");
        Vector3 dir = targetPosition - transform.position;

        /// if something is within the field of view
        if (Vector3.Angle(transform.forward, dir.normalized) < m_fieldOfView/2) {
            float distanceToTarget = Vector3.Distance(transform.position, targetPosition);
            /// if another object is between the target and the object (gotten by checking the layer mask)
            if (Physics.Raycast(transform.position + eyeHeight, dir.normalized, distanceToTarget, m_layerMask)) {
                /// return false, because something is blocking the view.
                return false;
            }

            return true;
        }

        /// If something is outside the field of view, return false because it's not detectable.
        return false;
    }

}