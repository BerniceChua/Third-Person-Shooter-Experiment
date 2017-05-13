using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Destructable : MonoBehaviour {
    [SerializeField] float m_hitPoints;

    public event System.Action OnDeath;
    public event System.Action OnDamageReceived;

    float m_damageTaken;
    
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public float HitPointsRemaining { get { return m_hitPoints - m_damageTaken; } }
    
    public bool IsAlive { get { return HitPointsRemaining > 0; } }

    public virtual void Die() {
        /// Removed when adding EnemyNPCHealth.cs because
        /// this stops the rest of the script from executing when
        /// it's needed it.
        //if (!IsAlive)
        //    return;

        if (OnDeath != null)
            OnDeath();
    }

    public virtual void TakeDamage(float damageAmount) {
        if (!IsAlive)
            return;

        m_damageTaken += damageAmount;

        if (OnDamageReceived != null)
            OnDamageReceived();

        if (HitPointsRemaining <= 0) {
            Die();
        }
    }

    public void Reset() {
        m_damageTaken = 0;
    }

}
