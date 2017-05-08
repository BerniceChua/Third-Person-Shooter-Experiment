using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using _Shared.Extensions;

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
        //print("~~~~~~~~~ Inside  EnemyPlayer_OnTargetSelected(" + target + ") ~~~~~~~~~~~~~~~~~~~~~");
        ActiveWeapon.m_AimTarget = target.transform;
        ActiveWeapon.m_AimTargetOffset = Vector3.up * 1.5f;
        StartBurst();
    }

    void CrouchedState() {
        /// this gives a 25% chance that NPC will take cover.
        bool takeCover = Random.Range(0, 3) == 0;

        if (!takeCover)
            return;

        /// Check how far away from target
        float distanceToTarget = Vector3.Distance(transform.position, ActiveWeapon.m_AimTarget.position);
        if (distanceToTarget > 15)
            m_enemyPlayer.GetComponent<EnemyAnimation>().IsCrouched = true;
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

        /// add a new burst only if target is visible.
        if (CanSeeTarget())
            GameManager.GameManagerInstance.Timer.Add(StartBurst, m_shootingSpeed);
    }

    bool CanSeeTarget() {
        /// if cannot find target
        if (!transform.IsInLineOfSight(ActiveWeapon.m_AimTarget.position, 90, m_enemyPlayer.m_playerScanner.m_layerMask, Vector3.up)) {
            /// reset the target
            m_enemyPlayer.ClearTargetAndScan();
            return false;
        }

        return true;
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