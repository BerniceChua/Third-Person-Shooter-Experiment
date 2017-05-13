﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour {

    [SerializeField] float m_rateOfFire;
    [SerializeField] Projectile m_projectile;
    [SerializeField] Transform m_hand;

    [SerializeField] AudioController m_audioReload;
    [SerializeField] AudioController m_audioFireWeapon;

    //[SerializeField] Transform m_aimTarget;
    public Transform m_AimTarget;
    public Vector3 m_AimTargetOffset;

    /*[HideInInspector]*/
    public bool m_canFire = true;

    /// NOTE: this "Muzzle" game object MUST be on the same hierarchy level as the game object that has this script.  If it's a child in the hierarchy, it won't work even with transform.FindChild();
    //[HideInInspector] public Transform m_muzzle;
    //[HideInInspector] public Transform m_muzzle { get { return transform.Find("Muzzle"); } set { m_muzzle = value; } }
    /// In the refactor, we changed m_muzzle to private.
    Transform m_muzzle { get { return transform.Find("ModelPositionGameObject/Muzzle"); } set { m_muzzle = value; } }

    //WeaponReloader m_reloader;
    [HideInInspector] public WeaponReloader m_reloader { get { return GetComponent<WeaponReloader>(); } set { m_reloader = value; } }

    /// Check if m_muzzle has component for particle system
    //ParticleSystem m_muzzleFireParticle;
    ParticleSystem m_muzzleFireParticle { get {return m_muzzle.GetComponent<ParticleSystem>(); } set { m_muzzleFireParticle = value; } }

    private WeaponRecoil m_weaponRecoil;
    private WeaponRecoil WeaponRecoil {
        get {
            if (m_weaponRecoil == null)
                m_weaponRecoil = GetComponent<WeaponRecoil>();

            return m_weaponRecoil;
        }
    }

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

        //m_muzzleFireParticle = m_muzzle.GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void Reload() {
        if (m_reloader == null)
            return;

        m_reloader.Reload();
        m_audioReload.Play();
    }

    void FiringEffect() {
        //if (m_muzzleFireParticle == null) {
        if (!m_muzzleFireParticle)
            return;

        m_muzzleFireParticle.Play();
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

        //print("Firing weapon at " + Time.time);
        //Debug.Log("m_muzzle = " + m_muzzle );

        /// If it's a player character & not NPC enemy, make it aim and shoot at the center of the screen
        bool isLocalPlayerControlled = m_AimTarget == null; /// if it does NOT have an aim target, it means it is a local player & not an NPC.
        if (!isLocalPlayerControlled) {
            //m_muzzle.LookAt(m_AimTarget);
            m_muzzle.LookAt(m_AimTarget.position + m_AimTargetOffset);
        }

        /// Spawn a new bullet
        Projectile newBullet = (Projectile)Instantiate(m_projectile, m_muzzle.position, m_muzzle.rotation);

        if (isLocalPlayerControlled) {
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
            RaycastHit hit;
            Vector3 targetPosition = ray.GetPoint(500.0f); /// this means that this will shoot accurately for 500 meters in the worldspace that's in front of this point.

            /// if the point of this raycast collides with something, it will set what it's colliding with as the target position
            if (Physics.Raycast(ray, out hit))
                targetPosition = hit.point;
            /// position the new bullets to the targetPosition.
            newBullet.transform.LookAt(targetPosition + m_AimTargetOffset);
        }

        if (this.WeaponRecoil)
            this.WeaponRecoil.ActivateCooldown();

        FiringEffect();

        // Instantiate the projectile
        //Instantiate(m_projectile, m_muzzle.position, m_muzzle.rotation);
        m_audioFireWeapon.Play();
        m_canFire = true;

    }
}