using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour {

    [SerializeField] float m_rateOfFire;
    [SerializeField] Projectile m_projectile;

    [HideInInspector] public Transform m_muzzle { get { return transform.Find("Muzzle"); } set { m_muzzle = value; } }
    //[HideInInspector] public Transform m_muzzle;
    [HideInInspector] public bool m_canFire;

    float m_timeBeforeNextFireAllowed;

	// Use this for initialization
	void Awake () {
        //m_muzzle = transform.Find("Muzzle");
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// This method Fire() is virtual, so it can be over-rided by a different class if needed.
    /// </summary>
    public virtual void FireWeapon() {
        m_canFire = false;

        if (Time.time < m_timeBeforeNextFireAllowed)
            return;
        else {
            m_timeBeforeNextFireAllowed = Time.time + m_rateOfFire;

            print("Firing weapon at " + Time.time);
            // Instantiate the projectile
            Instantiate(m_projectile, m_muzzle.position, m_muzzle.rotation);

            m_canFire = true;

        }

    }
}
