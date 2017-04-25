using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyPlayer))]
public class EnemyNPCHealth : Destructable {

    [SerializeField] Ragdoll m_ragdoll;

    public override void Die() {
        base.Die();

        m_ragdoll.EnableRagdoll(true);
    }

}
