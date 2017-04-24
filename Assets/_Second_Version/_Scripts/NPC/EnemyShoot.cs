﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyPlayer))]
public class EnemyShoot : WeaponsController {

    [SerializeField] float m_shootingSpeed;
    [SerializeField] float m_burstDurationMin;
    [SerializeField] float m_burstDurationMax;

    //EnemyPlayer m_enemyPlayer;
    EnemyPlayer m_enemyPlayer { get { return GetComponent<EnemyPlayer>(); } set { m_enemyPlayer = value; } }

    bool m_shouldFire;

    private void Start() {
        //m_enemyPlayer = GetComponent<EnemyPlayer>();
        m_enemyPlayer.OnTargetSelected += EnemyPlayer_OnTargetSelected;
    }

    /// <summary>
    /// When target is selected, run this function/method
    /// </summary>
    /// <param name="target"></param>
    private void EnemyPlayer_OnTargetSelected(Player target) {
        ActiveWeapon.m_AimTarget = target.transform;
        ActiveWeapon.m_AimTargetOffset = Vector3.up * 1.5f;
    }

    void StartBurst() {
        if (!m_enemyPlayer.EnemyHealth.IsAlive)
            return;

        CheckReload();
        m_shouldFire = true;

        GameManager.GameManagerInstance.Timer.Add(EndBurst, Random.Range(m_burstDurationMin, m_burstDurationMax));
    }

    void EndBurst() {
        m_shouldFire = false;

        if (!m_enemyPlayer.EnemyHealth.IsAlive)
            return;

        CheckReload();
        GameManager.GameManagerInstance.Timer.Add(StartBurst, m_shootingSpeed);
    }

    void CheckReload() {
        if (ActiveWeapon.m_reloader.RoundsRemainingInClip == 0)
            ActiveWeapon.Reload();
    }

    private void Update() {
        if (!m_shouldFire || !m_CanFire || !m_enemyPlayer.EnemyHealth.IsAlive)
            return;

        ActiveWeapon.FireWeapon();
    }

}