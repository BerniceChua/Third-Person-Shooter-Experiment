using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : PickUpItem {

	[SerializeField] EWeaponType m_weaponType;
    [SerializeField] float m_respawnTime;
    [SerializeField] int m_amount;
    
    // Use this for initialization
	void Start () {
        /// The below method/function was debug/testing code to test the event bus.  It's not necessary.
        //GameManager.GameManagerInstance.EventBus.AddListener("EnemyDeath", new EventBus.EventListener() {
        //    Method = () => {
        //        print("Enemy Death Listener " + transform.name);
        //    }
        //});
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void OnPickUp(Transform item) {
        //base.OnPickUp(item);
        //Debug.Log("Inside the public override void OnPickUp(Transform item)....");

        var playerInventory = item.GetComponentInChildren<Container>();
        GameManager.GameManagerInstance.Respawner.Despawn(gameObject, m_respawnTime);
        playerInventory.Put(m_weaponType.ToString(), m_amount);

        item.GetComponent<Player>().PlayerShoot.ActiveWeapon.m_reloader.HandleOnAmmoChanged();
    }

}