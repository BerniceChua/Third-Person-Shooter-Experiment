using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyNPC", menuName = "Data/EnemyNPC")]
public class NPCEnemy : ScriptableObject {

    public float m_RunSpeed;
    public float m_WalkSpeed;
    public float m_CrouchSpeed;
    public float m_SprintSpeed;

}
