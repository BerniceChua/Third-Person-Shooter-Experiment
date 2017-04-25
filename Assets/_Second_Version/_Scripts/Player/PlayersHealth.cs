using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayersHealth : Destructable {

    [SerializeField] Ragdoll m_ragdoll;

    public override void Die() {
        base.Die();

        m_ragdoll.EnableRagdoll(true);
    }

}