using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : Destructable {

    public override void Die() {
        base.Die();

        print("We died");
    }

    public override void TakeDamage(float damageAmount) {
        base.TakeDamage(damageAmount);

        print("Remaining health = " + HitPointsRemaining);
    }

}
