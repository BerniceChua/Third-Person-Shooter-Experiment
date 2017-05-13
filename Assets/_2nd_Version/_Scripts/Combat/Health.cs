﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : Destructable {

    [SerializeField] float inSeconds;

    public override void Die() {
        base.Die();

        print("We died");
        GameManager.GameManagerInstance.Respawner.Despawn(gameObject, inSeconds);
    }

    private void OnEnable() {
        Reset();
    }

    public override void TakeDamage(float damageAmount) {
        base.TakeDamage(damageAmount);

        print("Remaining health = " + HitPointsRemaining);
    }

}