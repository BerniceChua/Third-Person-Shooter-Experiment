using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour {

    [SerializeField] float m_rateOfFire;
    [SerializeField] Projectile m_projectile;
    [SerializeField] Transform m_hand;

    /*[HideInInspector]*/ public bool m_canFire = true;

    /// NOTE: this "Muzzle" game object MUST be on the same hierarchy level as the game object that has this script.  If it's a child in the hierarchy, it won't work even with transform.FindChild();
    //[HideInInspector] public Transform m_muzzle;
    //[HideInInspector] public Transform m_muzzle { get { return transform.Find("Muzzle"); } set { m_muzzle = value; } }
    /// In the refactor, we changed m_muzzle to private.
    Transform m_muzzle { get { return transform.Find("ModelPositionGameObject/Muzzle"); } set { m_muzzle = value; } }

    //WeaponReloader m_reloader;
    WeaponReloader m_reloader { get { return GetComponent<WeaponReloader>(); } set { m_reloader = value; } }

    float m_timeBeforeNextFireAllowed;

    /// <summary>
    /// When we have all the weapons, we make them all inactive.  Pls. see DeactivateWeapons() in PlayerShoot.cs.
    /// </summary>
    public void Equip() {
        transform.SetParent(m_hand);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnDisable() {

    }

    // Use this for initialization
    void Awake () {
        /// NOTE: this "Muzzle" game object MUST be on the same hierarchy level as the game object that has this script.  If it's a child in the hierarchy, it won't work even with transform.FindChild();
        //m_muzzle = transform.Find("Muzzle");

        //m_reloader = GetComponent<WeaponReloader>();

        // Moved this to OnEnable()
        //transform.SetParent(m_hand);
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
        //m_canFire = true;

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