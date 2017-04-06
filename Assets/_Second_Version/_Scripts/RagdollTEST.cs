using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollTEST : Destructable {

    [SerializeField] SpawnPoint[] m_spawnPoints;

    public Animator m_Animator;

    Rigidbody[] m_bodyParts;
    MoveController m_moveController { get { return GetComponent<MoveController>(); } set { m_moveController = value; } }

	// Use this for initialization
	void Start () {
        m_bodyParts = transform.GetComponentsInChildren<Rigidbody>();
        print("m_bodyParts.Length = " + m_bodyParts.Length);
        //if (m_bodyParts.Length == 0)
        //    return;

        EnableRagdoll(false);
        //m_moveController = GetComponent<MoveController>();

    }
	
	// Update is called once per frame
	void Update () {
        if (!IsAlive) {
            Die();
            return;
        }

        m_Animator.SetFloat("Vertical", 1);
        m_moveController.Move(new Vector2(5, 0));
	}

    void SpawnAtNewSpawnPoint() {
        int spawnIndex = Random.Range(0, m_spawnPoints.Length);
        transform.position = m_spawnPoints[spawnIndex].transform.position;
        transform.rotation = m_spawnPoints[spawnIndex].transform.rotation;
    }

    public override void Die() {
        base.Die();
        print("Inside public override void Die()");
        EnableRagdoll(true);
        m_Animator.enabled = false;
        print("m_Animator.enabled = " + m_Animator.enabled);

        GameManager.GameManagerInstance.Timer.Add(() => {
            EnableRagdoll(false);
            SpawnAtNewSpawnPoint();
            m_Animator.enabled = true;
            Reset();
        }, 5);
    }

    void EnableRagdoll(bool value) {
        print("Inside EnableRagdoll(bool value)");
        print("value = " + value);
        for (int i = 0; i < m_bodyParts.Length; i++) {
            m_bodyParts[i].isKinematic = !value;
            print("m_bodyParts[" + i + "].isKinematic = " + m_bodyParts[i].isKinematic);
            print("m_bodyParts[i].name = " + m_bodyParts[i].name);
        }
    }

}