using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssaultRifle : Shooter {

    public override void FireWeapon() {
        base.FireWeapon();

        if (m_canFire) {
            // We can fire the gun.
        }

    }

    public void Update() {
        if (GameManager.GameManagerInstance.InputController.m_reload) {
            Reload();
        }
    }

}
