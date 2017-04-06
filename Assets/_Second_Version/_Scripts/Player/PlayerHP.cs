using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHP : Health {

    [SerializeField] SpawnPoint[] m_spawnPoints;

    void SpawnAtNewSpawnPoint() {
        int spawnIndex = Random.Range(0, m_spawnPoints.Length);
        transform.position = m_spawnPoints[spawnIndex].transform.position;
        transform.rotation = m_spawnPoints[spawnIndex].transform.rotation;
    }

    public override void Die() {
        base.Die();
        SpawnAtNewSpawnPoint();
    }

    [ContextMenu("Test Die")]
    void TestDie() {
        Die();
    }
}