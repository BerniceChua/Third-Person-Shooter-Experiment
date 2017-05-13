using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour {

	public Animator m_animator;
	private Rigidbody[] m_bodyparts;

	// Use this for initialization
	void Start () {
		m_bodyparts = transform.GetComponentsInChildren<Rigidbody>();
		EnableRagdoll(false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	/// <summary>
	/// If the ragdoll is enabled, disable the animator, and
	/// make everything not kinematic.
	/// </summary>
	/// <returns>The ragdoll.</returns>
	/// <param name="value">Value.</param>
	public void EnableRagdoll(bool value) {
		m_animator.enabled = !value;

		for (int i = 0; i < m_bodyparts.Length; i++)
			m_bodyparts [i].isKinematic = !value;
	}
}
