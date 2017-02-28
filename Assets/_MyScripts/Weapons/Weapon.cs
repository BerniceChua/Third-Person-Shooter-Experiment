using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Collider))]
[RequireComponent (typeof(Rigidbody))]
public class Weapon : MonoBehaviour {
    Collider m_collider;
    Rigidbody m_rigidbody;
    Animator m_animator;

    public enum WeaponType {
        Pistol,
        Rifle
    }
    public WeaponType m_weaponType;

    [System.Serializable]
    public class UserSettings {
        public Transform leftHandIKTarget;
        public Vector3 spineRotation;  // in case the weapon's placement is different than the hand
    }
    [SerializeField] public UserSettings m_userSettings;

    [System.Serializable]
    public class WeaponSettings {
        [Header("-Bullet Options")]
        public Transform bulletSpawn;
        public float damage = 5.0f;
        public float bulletSpread = 5.0f;
        public float fireRate = 0.2f;
        public LayerMask bulletLayers;
        public float range = 400.0f;

        [Header("-Effects-")]
        public GameObject muzzleFlash;
        public GameObject decal;
        public GameObject shell;
        public GameObject clip;

        [Header("-Other-")]
        public float reloadDuration = 2.0f;
        public Transform shellEjectSpot;
        public float shellEjectSpeed = 7.5f;
        public Transform clipEjectPos;
        public GameObject clipGO;

        [Header("-Positioning-")]
        public Vector3 equiptPosition;
        public Vector3 equiptRotation;
        public Vector3 unequiptPosition;
        public Vector3 unequiptRotation;

        [Header("-Animation-")]
        public bool useAnimation;
        public int fireAnimationLayer; // layer of animator that has fire animation of weapon
        public string fireAnimationName = "Fire"; // lets animation play automatically without transitions
    }
    [SerializeField] public WeaponSettings m_weaponSettings;

    [System.Serializable]
    public class Ammunition {
        public int carryingAmmo;
        public int clipAmmo;
        public int maxClipAmmo;
    }
    [SerializeField] public Ammunition m_ammo;

    public Ray m_shootRay { protected get; set; }

    WeaponHandler m_owner;
    bool m_isEquipped;
    bool m_pullingTrigger;
    bool m_resettingCartridge;

	// Use this for initialization
	void Start () {
        m_collider = GetComponent<Collider>();
        m_rigidbody = GetComponent<Rigidbody>();
        m_animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		if (m_owner) {
            DisableOrEnableComponents(false);

            if (m_isEquipped) {
                if (m_owner.m_userSettings.rightHand) {
                    Equip();

                    if (m_pullingTrigger) {
                        FireWeapon(m_shootRay);
                    }
                }
            } else {
                Unequip(m_weaponType);
            }
        } else {
            DisableOrEnableComponents(true);

            transform.SetParent(null);
        }
	}

    // fires the weapon
    void FireWeapon(Ray ray) {
        if (m_ammo.clipAmmo <= 0 || m_resettingCartridge || !m_weaponSettings.bulletSpawn || !m_isEquipped)
            return;

        RaycastHit hit;
        Transform bulletSpawn = m_weaponSettings.bulletSpawn;
        Vector3 bSpawnPoint = bulletSpawn.position;
        //Vector3 dir = bulletSpawn.forward;
        Vector3 dir = ray.GetPoint(m_weaponSettings.range) - bSpawnPoint;

        dir += (Vector3)Random.insideUnitCircle * m_weaponSettings.bulletSpread;

        if (Physics.Raycast(bSpawnPoint, dir, out hit, m_weaponSettings.range, m_weaponSettings.bulletLayers)) {
            HitEffects(hit);
        }

        GunEffects();

        if (m_weaponSettings.useAnimation)
            m_animator.Play(m_weaponSettings.fireAnimationName, m_weaponSettings.fireAnimationLayer);

        m_ammo.clipAmmo--;
        m_resettingCartridge = true;
        StartCoroutine(LoadNextBullet());

    }

    // loads next bullet into chamber
    IEnumerator LoadNextBullet() {
        yield return new WaitForSeconds(m_weaponSettings.fireRate);
        m_resettingCartridge = false;
    }

    void HitEffects(RaycastHit hit) {
        // spawns a decal
        #region decal
        if (hit.collider.gameObject.isStatic) {
            if (m_weaponSettings.decal) {
                Vector3 hitPoint = hit.point;
                Quaternion lookRotation = Quaternion.LookRotation(hit.normal);
                GameObject decal = Instantiate(m_weaponSettings.decal, hitPoint, lookRotation) as GameObject;
                Transform decalT = decal.transform;
                Transform hitT = hit.transform;
                decalT.SetParent(hitT);
                Destroy(decal, Random.Range(15.0f, 20.0f));
            }
        }
        #endregion
    }

    void GunEffects() {
        #region muzzle flash
        if (m_weaponSettings.muzzleFlash) {
            Vector3 bulletSpawnPos = m_weaponSettings.bulletSpawn.position;
            GameObject muzzleFlash = Instantiate(m_weaponSettings.muzzleFlash, bulletSpawnPos, Quaternion.identity) as GameObject;
            Transform muzzleT = muzzleFlash.transform;
            muzzleT.SetParent(m_weaponSettings.bulletSpawn);
            Destroy(muzzleFlash, 1.0f);
        }
        #endregion

        #region shell
        if (m_weaponSettings.shell) {
            if (m_weaponSettings.shellEjectSpot) {
                Vector3 shellEjectPos = m_weaponSettings.shellEjectSpot.position;
                Quaternion shellEjectRot = m_weaponSettings.shellEjectSpot.rotation;
                GameObject shell = Instantiate(m_weaponSettings.shell, shellEjectPos, shellEjectRot) as GameObject;

                if (shell.GetComponent<Rigidbody>()) {
                    Rigidbody rb = shell.GetComponent<Rigidbody>();
                    rb.AddForce(m_weaponSettings.shellEjectSpot.forward * m_weaponSettings.shellEjectSpeed, ForceMode.Impulse);
                }

                Destroy(shell, Random.Range(30.0f, 45.0f));
            }
        }
        #endregion
    }

    // disable or enable collider and rigidbody
    void DisableOrEnableComponents(bool enabled) {
        if (!enabled) {
            m_rigidbody.isKinematic = true;
            m_collider.enabled = false;
        } else {
            m_rigidbody.isKinematic = false;
            m_collider.enabled = true;
        }
    }

    // "equips" (positions) the weapon to the hand.
    void Equip() {
        if (!m_owner)
            return;
        else if (!m_owner.m_userSettings.rightHand)
            return;

        transform.SetParent(m_owner.m_userSettings.rightHand);
        transform.localPosition = m_weaponSettings.equiptPosition;
        Quaternion equipRot = Quaternion.Euler(m_weaponSettings.equiptRotation);
        transform.localRotation = equipRot;
    }

    // unequips weapon and places it in the desired location.
    void Unequip(WeaponType wpType) {
        if (!m_owner)
            return;

        switch (wpType) {
            case WeaponType.Pistol:
                transform.SetParent(m_owner.m_userSettings.pistolUnequipSpot);
                break;
            case WeaponType.Rifle:
                transform.SetParent(m_owner.m_userSettings.rifleUnequipSpot);
                break;
        }

        transform.localPosition = m_weaponSettings.unequiptPosition;
        Quaternion unequipRot = Quaternion.Euler(m_weaponSettings.unequiptRotation);
        transform.localRotation = unequipRot;
    }

    // loads the clip and calculates the ammo (called after reload is called in weapon handler)
    public void LoadClip() {
        int ammoNeeded = m_ammo.maxClipAmmo - m_ammo.clipAmmo;

        if (ammoNeeded >= m_ammo.carryingAmmo) {
            m_ammo.clipAmmo = m_ammo.carryingAmmo;
            m_ammo.carryingAmmo = 0;
        } else {
            m_ammo.carryingAmmo -= ammoNeeded;
            m_ammo.clipAmmo = m_ammo.maxClipAmmo;
        }
    }

    // sets the weapons equip state
    public void SetEquipped(bool equipped) {
        m_isEquipped = equipped;
    }

    // pulls trigger programmatically
    public void PullTrigger(bool isPulling) {
        m_pullingTrigger = isPulling;
    }

    // sets weapon's owner
    public void SetOwner(WeaponHandler wpHandler) {
        m_owner = wpHandler;
    }

}