using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerShoot : WeaponsController {
    /// <summary>
    /// Moved everything to the WeaponsController when refactoring for enemy attacking.
    /// </summary>

    bool m_isPlayerAlive;

    // Use this for initialization
    void Start() {
        GetComponent<Player>().PlayersHealth.OnDeath += PlayersHealth_OnDeath;
        m_isPlayerAlive = true;
    }

    private void PlayersHealth_OnDeath() {
        m_isPlayerAlive = false;
    }

    // Update is called once per frame
    void Update () {
        if (!m_isPlayerAlive)
            return;

        if (GameManager.GameManagerInstance.InputController.m_MouseWheelDown)
            SwitchWeapon(1);

        if (GameManager.GameManagerInstance.InputController.m_MouseWheelUp)
            SwitchWeapon(-1);

        if (GameManager.GameManagerInstance.LocalPlayer.PlayerState.m_MoveState == PlayerStateMachine.EMoveState.SPRINTING)
            return;

        if (!m_CanFire)
            return;

        if (GameManager.GameManagerInstance.InputController.m_Fire1) {
            //Debug.Log("m_fire1 = " + GameManager.GameManagerInstance.InputController.m_fire1 + " at " + Time.time);
            ActiveWeapon.FireWeapon();
        }

        if (GameManager.GameManagerInstance.InputController.m_Reload) {
            ActiveWeapon.Reload();
        }
    }

}