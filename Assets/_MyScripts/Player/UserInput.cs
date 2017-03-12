using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInput : MonoBehaviour {
    public CharacterMovement m_charMove { get; protected set; }
    public WeaponHandler m_weaponHandler { get; protected set; }

    [System.Serializable]
    public class InputSettings {
        public string verticalAxis = "Vertical";
        public string horizontalAxis = "Horizontal";
        public string jumpButton = "Jump";

        public string reloadButton = "Reload";
        public string aimButton = "Fire2";
        public string fireButton = "Fire1";
        public string dropWeapon = "DropWeapon";
        public string switchWeaponButton = "SwitchWeapon";
    }
    [SerializeField] InputSettings m_input;

    [System.Serializable]
    public class OtherSettings {
        public float lookSpeed = 5.0f;
        public float lookDistance = 10.0f;
        public bool requireInputForTurn = true;

        public LayerMask aimDetectionLayers;
    }
    [SerializeField] public OtherSettings m_other;

    public bool m_debugAim; // helps us to position spine correctly
    public Transform m_spine;
    bool m_aiming;
    Weapon m_weapon;

    //Camera m_mainCam;
    public Camera m_thirdPersonCam;

    Dictionary<Weapon, GameObject> m_crosshairPrefabMap = new Dictionary<Weapon, GameObject>();

    // Use this for initialization
    void Start () {
        m_charMove = GetComponent<CharacterMovement>();
        //m_thirdPersonCam = Camera.main;
        m_weaponHandler = GetComponent<WeaponHandler>();
        m_weapon = GetComponent<Weapon>();

        SetupCrosshairs();
	}
	
    void SetupCrosshairs() {
        if (m_weaponHandler.m_weaponsList.Count > 0) {
            foreach (Weapon weapon in m_weaponHandler.m_weaponsList) {
                GameObject crosshairPrefab = weapon.m_weaponSettings.crosshairPrefab;

                if (crosshairPrefab != null) {
                    // this will drop the crosshair UI prefab first into the world space so we'll always have this reference and don't need to re-Instantiate each time it's needed.
                    //crosshairPrefab = Instantiate(crosshairPrefab);
                    GameObject clone = Instantiate(crosshairPrefab) as GameObject;

                    //m_crosshairPrefabMap.Add(weapon, crosshairPrefab);
                    m_crosshairPrefabMap.Add(weapon, clone);
                    
                    // we don't want this active right away, so it's false at first
                    ToggleCrosshairs(false, weapon);
                }
            }
        }
    }

	// Update is called once per frame
	void Update () {
        CharacterLogic();
        CameraLookLogic();
        WeaponLogic();
    }

    private void LateUpdate() {
        if (m_weaponHandler) {
            if (m_weaponHandler.m_currentWeapon) {
                if (m_aiming)
                    PositionSpine();
            }
        }
    }

    // handles character logic
    void CharacterLogic() {
        if (!m_charMove) {
            Debug.Log("Warning: no m_charMove.");
            return;
        }

        m_charMove.AnimateChar(Input.GetAxis(m_input.verticalAxis), Input.GetAxis(m_input.horizontalAxis));

        if (Input.GetButtonDown(m_input.jumpButton))
            m_charMove.Jump();
        //m_animator.SetBool(m_animations.jumpBool, m_isJumping);
    }

    // handles camera look logic
    void CameraLookLogic() {
        if (!m_thirdPersonCam) {
            Debug.Log("Warning: no m_mainCam.");
            return;
        }

        m_other.requireInputForTurn = !m_aiming;

        if (m_other.requireInputForTurn) {
            if (Input.GetAxis(m_input.horizontalAxis) != 0 || Input.GetAxis(m_input.verticalAxis) != 0)
                CharacterLook();
        } else {
            CharacterLook();
            Debug.Log("Weirdness...");
        }
    }

    // make character look at a forward point from the camera
    void CharacterLook() {
        Transform mainCamT = m_thirdPersonCam.transform;
        
        //Vector3 mainCamPos = mainCamT.position;
        Transform pivotT = mainCamT.parent;
        Vector3 pivotPos = pivotT.position;
        
        //Vector3 lookTarget = mainCamPos + (mainCamT.forward * m_other.lookDistance);
        Vector3 lookTarget = pivotPos + (pivotT.forward * m_other.lookDistance);

        Vector3 thisPos = transform.position;
        Vector3 lookDir = lookTarget - thisPos;
        Quaternion lookRot = Quaternion.LookRotation(lookDir);
        lookRot.x = 0;
        lookRot.z = 0;

        Quaternion newRotation = Quaternion.Lerp(transform.rotation, lookRot, Time.deltaTime * m_other.lookSpeed);
        transform.rotation = newRotation;
    }

    void WeaponLogic() {
        if (!m_weaponHandler) {
            Debug.Log("Warning: no m_weaponHandler.");
            return;
        }

        //m_aiming = Input.GetButton(m_input.aimButton) || m_debugAim;

        ////if (m_weaponhandler.m_currentWeapon) {
        //    m_weaponhandler.Aim(m_aiming);

        //    m_other.requireInputForTurn = !m_aiming;

        //    m_weaponhandler.FingerOnTrigger(Input.GetButton(m_input.fireButton));

        //    if (Input.GetButtonDown(m_input.reloadButton))
        //        m_weaponhandler.Reload();

        //    if (Input.GetButtonDown(m_input.dropWeapon))
        //        m_weaponhandler.DropCurrentWeapon();

        //    if (Input.GetButtonDown(m_input.switchWeaponButton))
        //        m_weaponhandler.SwitchWeapons();
        ////}

        //if (!m_weaponhandler.m_currentWeapon)
        //    return;

        //m_weaponhandler.m_currentWeapon.m_shootRay = new Ray(m_thirdPersonCam.transform.position, m_thirdPersonCam.transform.forward);

        m_aiming = Input.GetButton(m_input.aimButton) || m_debugAim;
        m_weaponHandler.Aim(m_aiming);

        if (Input.GetButtonDown(m_input.switchWeaponButton)) {
            m_weaponHandler.SwitchWeapons();
            UpdateCrosshairs();
        }

        if (m_weaponHandler.m_currentWeapon) {
            Ray aimRay = new Ray(m_thirdPersonCam.transform.position, m_thirdPersonCam.transform.forward);
            //Debug.DrawRay(aimRay.origin, aimRay.direction);
            //m_weaponhandler.m_currentWeapon.m_shootRay = aimRay;

            if (Input.GetButton(m_input.fireButton) && m_aiming)
                m_weaponHandler.FireCurrentWeapon(aimRay);

            if (Input.GetButtonDown(m_input.reloadButton))
                m_weaponHandler.Reload();

            if (Input.GetButtonDown(m_input.dropWeapon)) {
                DeleteCrosshair(m_weaponHandler.m_currentWeapon);
                m_weaponHandler.DropCurrentWeapon();
            }

            if (m_aiming) {
                ToggleCrosshairs(true, m_weaponHandler.m_currentWeapon);
                PositionCrosshairs(aimRay, m_weaponHandler.m_currentWeapon);
            } else
                ToggleCrosshairs(false, m_weaponHandler.m_currentWeapon);
        } else {
            TurnOffAllCrosshairs();
        }
    }

    /// turns off all crosshairs when we don't have a current weapon.
    void TurnOffAllCrosshairs() {
        foreach (Weapon wep in m_crosshairPrefabMap.Keys) {
            ToggleCrosshairs(false, wep);
        }
    }

    void CreateCrosshair(Weapon wep) {
        GameObject crosshairPrefab = wep.m_weaponSettings.crosshairPrefab;

        if (crosshairPrefab != null) {
            // this will drop the crosshair UI prefab first into the world space so we'll always have this reference and don't need to re-Instantiate each time it's needed.
            crosshairPrefab = Instantiate(crosshairPrefab);

            // we don't want this active right away, so it's false at first
            ToggleCrosshairs(false, wep);
        }
    }

    void DeleteCrosshair (Weapon wep) {
        if (!m_crosshairPrefabMap.ContainsKey(wep))
            return;

        Destroy(m_crosshairPrefabMap[wep]);
        m_crosshairPrefabMap.Remove(wep);
    }

    // positions crosshairs to the point that we are aiming
    void PositionCrosshairs(Ray ray, Weapon weapon) {
        Weapon currentWeapon = m_weaponHandler.m_currentWeapon;

        if (currentWeapon == null)
            return;

        if (!m_crosshairPrefabMap.ContainsKey(weapon))
            return;

        //GameObject crosshairPrefab = currentWeapon.m_weaponSettings.crosshairPrefab;
        GameObject crosshairPrefab = m_crosshairPrefabMap[weapon];

        RaycastHit hit;
        Transform bulletSpawn = currentWeapon.m_weaponSettings.bulletSpawn;
        Vector3 bSpawnPoint = bulletSpawn.position;
        //Vector3 dir = bulletSpawn.forward;
        Vector3 dir = ray.GetPoint(currentWeapon.m_weaponSettings.range) - bSpawnPoint;

        if (Physics.Raycast(bSpawnPoint, dir, out hit, currentWeapon.m_weaponSettings.range, currentWeapon.m_weaponSettings.bulletLayers)) {

            // prevents us from getting errors
            if (crosshairPrefab != null) {
                ToggleCrosshairs(true, currentWeapon);

                crosshairPrefab.transform.position = hit.point;
                crosshairPrefab.transform.LookAt(Camera.main.transform);
            }
        }
        else {
            ToggleCrosshairs(false, currentWeapon);
        }
    }

    // toggle on and off the crosshairs prefab
    public void ToggleCrosshairs(bool enabled, Weapon wep) {
        //if (wep.m_weaponSettings.crosshairPrefab != null) {
        //    wep.m_weaponSettings.crosshairPrefab.SetActive(enabled);
        //}

        if (!m_crosshairPrefabMap.ContainsKey(wep))
            return;

        m_crosshairPrefabMap[wep].SetActive(enabled);
    }

    void UpdateCrosshairs() {
        if (m_weaponHandler.m_weaponsList.Count == 0)
            return;

        foreach (Weapon weapon in m_weaponHandler.m_weaponsList) {
            if (weapon != m_weaponHandler.m_currentWeapon) {
                ToggleCrosshairs(false, weapon);
            }
        }
    }

    // positions spine when aiming
    void PositionSpine() {
        if (!m_spine || !m_weaponHandler.m_currentWeapon || !m_thirdPersonCam)
            return;

        //RaycastHit hit;
        Transform mainCamT = m_thirdPersonCam.transform;
        Vector3 mainCamPos = mainCamT.position;
        Vector3 dir = mainCamT.forward;
        Ray ray = new Ray(mainCamPos, dir);

        m_spine.LookAt(ray.GetPoint(50));

        //if (Physics.Raycast(ray, out hit, 100, m_other.aimDetectionLayers)) {
        //    Vector3 hitPoint = hit.point;
        //    m_spine.LookAt(hitPoint);
        //} else {
        //    m_spine.LookAt(ray.GetPoint(50));
        //}

        Vector3 eulerAngleOffset = m_weaponHandler.m_currentWeapon.m_userSettings.spineRotation;
        m_spine.Rotate(eulerAngleOffset);
    }

}