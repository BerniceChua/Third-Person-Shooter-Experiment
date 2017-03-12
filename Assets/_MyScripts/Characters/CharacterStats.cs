using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour {

    [Range(0,100)] public float m_health;
    public int m_enemyFaction;
    
    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        m_health = Mathf.Clamp(m_health, 0, 100);
    }
}