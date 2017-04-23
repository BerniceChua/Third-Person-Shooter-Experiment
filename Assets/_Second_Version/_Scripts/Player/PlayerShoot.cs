using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : WeaponsController {
    /// <summary>
    /// Moved everything to the WeaponsController when refactoring for enemy attacking.
    /// </summary>
    
	// Update is called once per frame
	void Update () {
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
	}

}