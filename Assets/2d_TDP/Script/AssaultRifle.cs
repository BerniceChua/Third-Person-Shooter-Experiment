﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssaultRifle : Shooter {

    public override void Fire() {
        base.Fire();

        if (m_canFire) {
            // We can fire the gun.
        }

    }

}
