using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollManager : MonoBehaviour {
    private Collider[] m_ragdollColliders { get { return GetComponentsInChildren<Collider>(); } set { m_ragdollColliders = value; } }
    private Rigidbody[] m_rigidbodies { get { return GetComponentsInChildren<Rigidbody>(); } set { m_rigidbodies = value; } }
    private Animator m_animator { get { return GetComponentInParent<Animator>(); } set { m_animator = value; } }

    // Use this for initialization
    void Start () {
        if (m_ragdollColliders.Length == 0)
            return;

        if (m_rigidbodies.Length == 0)
            return;

        // these both start turned off, and they will be switched on when the Ragdoll() method is called
        foreach (Collider col in m_ragdollColliders) {
            col.enabled = false;
            //col.isTrigger = true;
        }

        // these both start turned on, and they will be switched off when the Ragdoll() method is called
        foreach (Rigidbody r in m_rigidbodies) {
            r.isKinematic = true;
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    // call this whenever you want the character to ragdoll.
    public void Ragdoll() {
        if (m_animator == null)
            return;

        if (m_ragdollColliders.Length == 0)
            return;

        if (m_rigidbodies.Length == 0)
            return;

        m_animator.enabled = false;

        foreach(Collider col in m_ragdollColliders) {
            col.enabled = true;
            //col.enabled = false;
        }

        foreach (Rigidbody r in m_rigidbodies) {
            r.isKinematic = false;
        }

    }

}
