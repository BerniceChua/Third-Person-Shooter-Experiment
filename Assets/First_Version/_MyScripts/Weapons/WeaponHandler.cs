using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHandler : MonoBehaviour {
    private Animator m_animator { get { return GetComponent<Animator>(); } set { m_animator = value; } }
    private SoundController m_soundControl;

    public bool m_isAI;

    [System.Serializable]
    public class UserSettings {
        public Transform rightHand;
        public Transform pistolUnequipSpot;
        public Transform rifleUnequipSpot;
    }
    [SerializeField] public UserSettings m_userSettings;

    [System.Serializable]
    public class WeaponAnimations {
        public string weaponTypeInt = "WeaponType";
        public string reloadingBool = "IsReloading";
        public string aimingBool = "Aiming";
    }
    [SerializeField] public WeaponAnimations m_weaponAnims;

    public Weapon m_currentWeapon;
    public List<Weapon> m_weaponsList = new List<Weapon>();
    public int m_maxWeapons = 2;

    public bool m_aim; /*{ get; protected set; }*/
    public bool m_reload;

    private int m_weaponType;
    private bool m_settingWeapon;

	// Use this for initialization
	void OnEnable () {
        GameObject check = GameObject.FindGameObjectWithTag("SoundController");

        if (check != null) {
            m_soundControl = check.GetComponent<SoundController>();
        }
        m_soundControl = GameObject.FindGameObjectWithTag("SoundController").GetComponent<SoundController>();

        /// instead of this in the Start() or Awake() or OnEnable(), you can just use the
        /// "get"+"set" design pattern to initialize.
        //m_animator = GetComponent<Animator>();

        SetupWeapons();
	}

    void SetupWeapons() {
        if (m_currentWeapon) {
            m_currentWeapon.SetEquipped(true);
            m_currentWeapon.SetOwner(this);
            AddWeaponToList(m_currentWeapon);

            //m_currentWeapon.m_isOwnerAiming = m_aim;

            if (m_currentWeapon.m_ammo.clipAmmo <= 0)
                Reload();

            if (m_reload) {
                if (m_settingWeapon) {
                    m_reload = false;
                }
            }
        }

        if (m_weaponsList.Count > 0) {
            for (int i = 0; i < m_weaponsList.Count; i++) {
                if (m_weaponsList[i] != m_currentWeapon) {
                    m_weaponsList[i].SetEquipped(false);
                    m_weaponsList[i].SetOwner(this);
                }
            }
        }

    }

    // Update is called once per frame
    void Update () {
        AnimateWeaponHandling();
	}

    // animates character 
    void AnimateWeaponHandling() {
        if (!m_animator)
            return;

        m_animator.SetBool(m_weaponAnims.aimingBool, m_aim);
        m_animator.SetBool(m_weaponAnims.reloadingBool, m_reload);
        m_animator.SetInteger(m_weaponAnims.weaponTypeInt, m_weaponType);

        if (!m_currentWeapon) {
            m_weaponType = 0;
            return;
        }

        switch(m_currentWeapon.m_weaponType) {
            case Weapon.WeaponType.Pistol:
                m_weaponType = 1;
                break;
            case Weapon.WeaponType.Rifle:
                m_weaponType = 2;
                break;
        }
    }

    // adds weapon to m_weaponList
    void AddWeaponToList(Weapon weapon) {
        if (m_weaponsList.Contains(weapon))
            return;

        m_weaponsList.Add(weapon);
    }

    public void FireCurrentWeapon(Ray aimRay) {
        if (m_currentWeapon.m_ammo.clipAmmo == 0) {
            Reload();
            return;
        }

        m_currentWeapon.FireWeapon(aimRay);
    }

    // puts finger on the trigger and asks if we pull
    public void FingerOnTrigger(bool pulling) {
        if (!m_currentWeapon)
            return;

        m_currentWeapon.PullTrigger(pulling && m_aim && !m_reload);
    }

    public void Reload() {
        if (m_reload || !m_currentWeapon)
            return;

        if (m_currentWeapon.m_ammo.carryingAmmo <= 0 || m_currentWeapon.m_ammo.clipAmmo == m_currentWeapon.m_ammo.maxClipAmmo)
            return;

        if (m_soundControl != null) {
            if (m_currentWeapon.m_soundSettings.reloadSound != null) {
                if (m_currentWeapon.m_soundSettings.audSource != null) {
                    m_soundControl.PlaySound(
                        m_currentWeapon.m_soundSettings.audSource,
                        m_currentWeapon.m_soundSettings.reloadSound,
                        true,
                        m_currentWeapon.m_soundSettings.pitchMin,
                        m_currentWeapon.m_soundSettings.pitchMax
                    );
                }
            }
        }

        m_reload = true;
        StartCoroutine(StopReload());
    }

    IEnumerator StopReload() {
        yield return new WaitForSeconds(m_currentWeapon.m_weaponSettings.reloadDuration);

        m_currentWeapon.LoadClip();
        m_reload = false;
    }

    public void Aim(bool aiming) {
        m_aim = aiming;
    }

    // drops current weapon
    public void DropCurrentWeapon() {
        if (!m_currentWeapon)
            return;

        m_currentWeapon.SetEquipped(false);
        m_currentWeapon.SetOwner(null);
        m_weaponsList.Remove(m_currentWeapon);

        m_currentWeapon = null;
    }

    // Switches to the next weapon
    public void SwitchWeapons() {
        if (m_settingWeapon || m_weaponsList.Count == 0)
            return;

        if (m_currentWeapon) {
            int currentWeaponIndex = m_weaponsList.IndexOf(m_currentWeapon);
            int nextWeaponIndex = (currentWeaponIndex + 1) % m_weaponsList.Count;

            m_currentWeapon = m_weaponsList[nextWeaponIndex];
        } else {
            m_currentWeapon = m_weaponsList[0];
        }

        // these 2 stops animations from constantly changing weapon & glitching out the IK & helps disable the IK when switching weapons so hand doesn't go through body.
        m_settingWeapon = true;
        StartCoroutine(StopSettingWeapon());

        // refreshes whenever player switch weapons.
        SetupWeapons();
    }

    IEnumerator StopSettingWeapon() {
        yield return new WaitForSeconds(0.7f);
        m_settingWeapon = false;
    }

    private void OnAnimatorIK(int layerIndex) {
        if (!m_animator) {
            Debug.Log("No animator for weapon handler");
            return;
        }

        // alternate version:
        if (m_currentWeapon && m_currentWeapon.m_userSettings.leftHandIKTarget && m_weaponType == 2 && !m_reload && !m_settingWeapon && !m_isAI) {
        //if (m_currentWeapon && m_currentWeapon.m_userSettings.leftHandIKTarget && m_weaponType == 2 && !m_reload && !m_settingWeapon) {
            m_animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
            m_animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);

            Transform target = m_currentWeapon.m_userSettings.leftHandIKTarget;
            Vector3 targetPos = target.position;
            Quaternion targetRot = target.rotation;

            m_animator.SetIKPosition(AvatarIKGoal.LeftHand, targetPos);
            m_animator.SetIKRotation(AvatarIKGoal.LeftHand, targetRot);
        } else {
            m_animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
            m_animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
        }        
    }

}