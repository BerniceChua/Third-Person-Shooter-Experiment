using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugRay : MonoBehaviour {
    [SerializeField] Transform bulletSpawn;

    //WeaponHandler m_weaponHandler { get { return GameObject.FindGameObjectWithTag("Player").GetComponent<WeaponHandler>(); } set { m_weaponHandler = value; } }
    WeaponHandler m_weaponHandler;
    //Weapon currentWeapon = m_weaponHandler.m_currentWeapon;
    Weapon currentWeapon;

    // Use this for initialization
    void Start () {
        //m_weaponHandler = GameObject.FindGameObjectWithTag("Player").GetComponent<WeaponHandler>();
        currentWeapon = m_weaponHandler.m_currentWeapon;
    }
	
	// Update is called once per frame
	void Update () {
        Ray ray = new Ray(transform.position, transform.forward);
        Vector3 bSpawnPoint = bulletSpawn.position;
        //Vector3 dir = bulletSpawn.forward;
        
        //Vector3 dir = ray.GetPoint(currentWeapon.m_weaponSettings.range) - bSpawnPoint;

        //Debug.DrawRay(bSpawnPoint, dir, Color.green);
    }
}
