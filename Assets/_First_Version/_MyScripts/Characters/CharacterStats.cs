using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour {

    private CharacterController m_charController { get { return GetComponent<CharacterController>(); } set { m_charController = value; } }
    private RagdollManager m_ragdollManager { get { return GetComponentInChildren<RagdollManager>(); } set { m_ragdollManager = value; } }

    [Range(0,1000)] public float m_health;
    public int m_enemyFaction;

    public MonoBehaviour[] m_scriptsToDisable;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        m_health = Mathf.Clamp(m_health, 0, 100);
    }

    public void Damage(float damage) {
        m_health -= damage;

        if (m_health <= 0)
            Die();
    }

    void Die() {
        m_charController.enabled = false;

        if (m_scriptsToDisable.Length == 0) {
            Debug.Log("All scripts are still running on this character but he/she is dead.");
            return;
        }

        foreach (MonoBehaviour script in m_scriptsToDisable) {
            script.enabled = false;
        }

        if (m_ragdollManager != null)
            m_ragdollManager.Ragdoll();
    }

}