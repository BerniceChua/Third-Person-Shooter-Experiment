using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Rigidbody))]
public class Projectile : MonoBehaviour {
    [SerializeField] float m_speed;
    [SerializeField] float m_timeToLive;
    [SerializeField] float m_damage;

	// Use this for initialization
	void Start () {
        Destroy(gameObject, m_timeToLive);
	}
	
	// Update is called once per frame
	void Update () {
        transform.Translate(Vector3.forward * m_speed * Time.deltaTime);
	}

    private void OnTriggerEnter(Collider other) {
        //print("Projectile collided at time " + Time.time);
        //print("Projectile has hit " + other.name);

        var destructable = other.transform.GetComponent<Destructable>();

        if (destructable == null)
            //if (!destructable) // this one won't work, it really needs to be "destructible == null" in order for TakeDamage(float damageAmount) to trigger OnDeath() event.
            return;

        destructable.TakeDamage(m_damage);
    }

}
