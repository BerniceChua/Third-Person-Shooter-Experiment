using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
    public static GameController GC;

    private UserInput m_player { get { return FindObjectOfType<UserInput>(); } set { m_player = value; } }

    private PlayerUI m_playerUI { get { return FindObjectOfType<PlayerUI>(); } set { m_playerUI = value; } }

    private WeaponHandler m_weaponHandler { get { return FindObjectOfType<WeaponHandler>(); } set { m_weaponHandler = value; } }


    private void Awake() {
        if (GC == null) {
            GC = this;
        } else {
            if (GC != this)
                Destroy(gameObject);
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        UpdateUI();
	}

    void UpdateUI() {
        if (m_player) {
            if (m_playerUI) {
                if (m_weaponHandler) {
                    if (m_playerUI.ammoText) {
                        if (m_weaponHandler.m_currentWeapon == null) {
                            m_playerUI.ammoText.text = "Unarmed";
                        } else {
                            m_playerUI.ammoText.text = m_weaponHandler.m_currentWeapon.m_ammo.clipAmmo + "/" + m_weaponHandler.m_currentWeapon.m_ammo.carryingAmmo;
                        }
                    }
                }

                if (m_playerUI.healthBar && m_playerUI.healthText) {
                    m_playerUI.healthText.text = Mathf.Round(m_playerUI.healthBar.value).ToString();
                }

            }
        }
    }

}