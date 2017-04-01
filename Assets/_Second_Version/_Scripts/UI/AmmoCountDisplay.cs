using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoCountDisplay : MonoBehaviour {
    [SerializeField] Text m_text;

    //Container m_inventory;
    PlayerShoot m_playerShoot;
    WeaponReloader m_reloader;

	// Use this for initialization
	void Awake () {
        GameManager.GameManagerInstance.OnLocalPlayerJoined += HandleOnLocalPlayerJoined;
	}

    // Update is called once per frame
    void Update () {
		
	}

    private void HandleOnLocalPlayerJoined(Player player) {
        //m_inventory = player.GetComponent<Container>();
        m_playerShoot = player.PlayerShoot;
        m_playerShoot.OnWeaponSwitch += HandleOnWeaponSwitch;
        print("entered HandleOnLocalPlayerJoined(Player player)...");
        m_reloader = m_playerShoot.ActiveWeapon.m_reloader;
        m_reloader.OnAmmoChanged += HandleOnAmmoChanged;

        HandleOnAmmoChanged();
    }

    private void HandleOnWeaponSwitch(Shooter activeWeapon) {
        m_reloader = activeWeapon.m_reloader;
        m_reloader.OnAmmoChanged += HandleOnAmmoChanged;
        HandleOnAmmoChanged();
    }

    private void HandleOnAmmoChanged() {
        int amountInInventory = m_reloader.RoundsRemainingInInventory;
        int amountInClip = m_reloader.RoundsRemainingInClip;

        //m_text.text = amountInClip.ToString() + "/" + amountInInventory.ToString();
        // Below is an alternate way from the line above
        m_text.text = string.Format("{0}/{1}", amountInClip, amountInInventory);
    }

}