using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyPlayer))]
public class EnemyShoot : WeaponsController {

    [SerializeField] float m_shootingSpeed;
    [SerializeField] float m_burstDurationMin;
    [SerializeField] float m_burstDurationMax;

    //EnemyPlayer m_enemyPlayer;
    EnemyPlayer m_enemyPlayer { get { return GetComponent<EnemyPlayer>(); } set { m_enemyPlayer = value; } }

    private void Start() {
        //m_enemyPlayer = GetComponent<EnemyPlayer>();
        m_enemyPlayer.OnTargetSelected += M_enemyPlayer_OnTargetSelected;
    }

    private void M_enemyPlayer_OnTargetSelected(Player target) {
        ActiveWeapon.FireWeapon();
    }
}