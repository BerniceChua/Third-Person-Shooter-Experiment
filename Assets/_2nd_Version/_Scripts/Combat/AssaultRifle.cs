﻿using System.Collections;
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
        //Debug.Log("this item is " + transform.name + ", and its parent object is " + transform.parent.name + "(" + Time.time + ")");

        /// Removed this from here to debug, because if player reloads, even NPC will reload even if clip wasn't emptied yet.
        //if (GameManager.GameManagerInstance.InputController.m_Reload) {
        //    Reload();
        //}
    }

}