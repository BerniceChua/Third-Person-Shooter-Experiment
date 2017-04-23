using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponsController : MonoBehaviour {

    // Removed at "weapon switcher" phase.
    //[SerializeField] Shooter m_assaultRifle;

    [SerializeField]
    float m_weaponSwitchTime;

    // Looks for weapons in any transforms.
    Shooter[] m_weaponsArray;
    //Shooter[] m_weaponsArray { get { return m_weaponHolster.GetComponentsInChildren<Shooter>(); } set { m_weaponsArray = value; } }
    Shooter m_activeWeapon;

    int m_currentWeaponIndex;
    bool m_canFire;
    Transform m_weaponHolster;
    //Transform m_weaponHolster { get { return transform.FindChild("WeaponsGameObject"); } set { m_weaponHolster = value; } }

    public event System.Action<Shooter> OnWeaponSwitch;

    public Shooter ActiveWeapon { get { return m_activeWeapon; } }

    // Use this for initialization
    void Awake() {
        m_canFire = true;
        m_weaponHolster = transform.FindChild("WeaponsGameObject");
        m_weaponsArray = m_weaponHolster.GetComponentsInChildren<Shooter>();

        //Debug.Log("m_currentWeaponIndex = " + m_currentWeaponIndex);
        //Debug.Log("m_weaponsArray[" + m_currentWeaponIndex + "]" + m_weaponsArray[m_currentWeaponIndex]);

        //Debug.Log(m_weaponsArray.Length);

        /// Refactored into the line below
        //if (m_weaponsArray.Length > 0)
        //    m_activeWeapon = m_weaponsArray[0];

        //EquipWeapon(m_currentWeaponIndex);

        if (m_weaponsArray.Length > 0)
            EquipWeapon(0);
    }

    void EquipWeapon(int index) {
        /// Each time a weapon is equipped, we deactivate all the weapons, and only activate the selected weapon.
        DeactivateWeapon();

        m_canFire = true;
        m_activeWeapon = m_weaponsArray[index];

        m_activeWeapon.Equip();

        m_weaponsArray[index].gameObject.SetActive(true);
        print("executing EquipWeapon()...");
        if (OnWeaponSwitch != null)
            OnWeaponSwitch(m_activeWeapon);
    }

    void DeactivateWeapon() {
        for (int i = 0; i < m_weaponsArray.Length; i++) {
            print("Deactivating...");
            m_weaponsArray[i].gameObject.SetActive(false);
            m_weaponsArray[i].transform.SetParent(m_weaponHolster);
        }
    }

    void SwitchWeapon(int direction) {
        m_canFire = false;

        m_currentWeaponIndex += direction; // even if it's negative, it will still unequip/switch the weapon.

        if (m_currentWeaponIndex > m_weaponsArray.Length - 1)
            m_currentWeaponIndex = 0;

        if (m_currentWeaponIndex < 0)
            m_currentWeaponIndex = m_weaponsArray.Length - 1;

        GameManager.GameManagerInstance.Timer.Add(() => { EquipWeapon(m_currentWeaponIndex); }, m_weaponSwitchTime);
    }

}
