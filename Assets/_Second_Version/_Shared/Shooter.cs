using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour {

    [SerializeField] float m_rateOfFire;
    [SerializeField] Projectile m_projectile;

    /// NOTE: this "Muzzle" game object MUST be on the same hierarchy level as the game object that has this script.  If it's a child in the hierarchy, it won't work even with transform.FindChild();
    //[HideInInspector] public Transform m_muzzle;
    [HideInInspector] public Transform m_muzzle { get { return transform.Find("Muzzle"); } set { m_muzzle = value; } }
    [HideInInspector] public bool m_canFire;

    //WeaponReloader m_reloader;
    WeaponReloader m_reloader { get { return GetComponent<WeaponReloader>(); } set { m_reloader = value; } }

    float m_timeBeforeNextFireAllowed;

	// Use this for initialization
	void Awake () {
        /// NOTE: this "Muzzle" game object MUST be on the same hierarchy level as the game object that has this script.  If it's a child in the hierarchy, it won't work even with transform.FindChild();
        //m_muzzle = transform.Find("Muzzle");

        //m_reloader = GetComponent<WeaponReloader>();
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void Reload() {
        if (m_reloader == null)
            return;

        m_reloader.Reload();
    }

    /// <summary>
    /// This method Fire() is virtual, so it can be over-rided by a different class if needed.
    /// </summary>
    public virtual void FireWeapon() {
        m_canFire = false;

        if (Time.time < m_timeBeforeNextFireAllowed)
            return;

        if (m_reloader != null) {
            if (m_reloader.IsReloading)
                return;

            if (m_reloader.RoundsRemainingInClip == 0)
                return;

            m_reloader.TakeFromClip(1);
        }

        m_timeBeforeNextFireAllowed = Time.time + m_rateOfFire;

        print("Firing weapon at " + Time.time);
        // Instantiate the projectile
        Instantiate(m_projectile, m_muzzle.position, m_muzzle.rotation);

        m_canFire = true;

    }
}
