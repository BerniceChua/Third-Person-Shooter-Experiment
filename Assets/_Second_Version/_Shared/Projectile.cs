using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Rigidbody))]
public class Projectile : MonoBehaviour {
    [SerializeField] float m_speed;
    [SerializeField] float m_timeToLive;
    [SerializeField] float m_damage;
    [SerializeField] Transform m_bulletHole;

    Vector3 m_hitDestination;

	// Use this for initialization
	void Start () {
        Destroy(gameObject, m_timeToLive);
	}

    // Update is called once per frame
    void Update() {
        if (IsDestinationReached()) {
            Destroy(gameObject);
            return;
        }

        transform.Translate(Vector3.forward * m_speed * Time.deltaTime);

        if (m_hitDestination != Vector3.zero)
            return;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit)) {
            CheckDestructable(hit);
        }
	}

    //private void OnTriggerEnter(Collider other) {
    //    //print("Projectile collided at time " + Time.time);
    //    //print("Projectile has hit " + other.name);

    //    var destructable = other.transform.GetComponent<Destructable>();

    //    if (destructable == null)
    //        //if (!destructable) // this one won't work, it really needs to be "destructible == null" in order for TakeDamage(float damageAmount) to trigger OnDeath() event.
    //        return;

    //    destructable.TakeDamage(m_damage);
    //}
    /// <summary>
    ///  Refactored in aiming,_shooting,_and_target from OnTriggerEnter() into CheckDestructable()
    /// </summary>
    /// <param name="other"></param>
    //private void CheckDestructable(Transform other) {
    //    //print("other.name = " + other.name);

    //    var destructable = other.GetComponent<Destructable>();
    //    //print("destructable = " + destructable);

    //    if (destructable == null) {
    //        //if (!destructable) // this one won't work, it really needs to be "destructible == null" in order for TakeDamage(float damageAmount) to trigger OnDeath() event.
    //        //print("I collided with " + destructable.name + " at time " + Time.time + ", therefore destructable == null");
    //        return;
    //    }

    //    //print("Projectile collided with " + destructable.name + " at time " + Time.time + ".  Giving m_damage == " + m_damage);
    //    destructable.TakeDamage(m_damage);
    //}

    /// <summary>
    ///  Refactored in bullet-holes-2nd-version to use the RaycastHit hitInfo parameter instead of "Transform other".
    /// </summary>
    /// <param name="hit"></param>
    private void CheckDestructable(RaycastHit hitInfo) {
        //print("other.name = " + other.name);

        var destructable = hitInfo.transform.GetComponent<Destructable>();
        //print("destructable = " + destructable);

        /// hitInfo.point is the point in worldspace where the Raycast's ray hits the collider.
        m_hitDestination = hitInfo.point;

        Instantiate(GameObject.CreatePrimitive(PrimitiveType.Cube), m_hitDestination, Quaternion.identity);

        if (destructable == null) {
            //if (!destructable) // this one won't work, it really needs to be "destructible == null" in order for TakeDamage(float damageAmount) to trigger OnDeath() event.
            //print("I collided with " + destructable.name + " at time " + Time.time + ", therefore destructable == null");
            return;
        }

        //print("Projectile collided with " + destructable.name + " at time " + Time.time + ".  Giving m_damage == " + m_damage);
        destructable.TakeDamage(m_damage);
    }

    bool IsDestinationReached() {
        if (m_hitDestination == Vector3.zero)
            return false;

        Vector3 directionToDestination = m_hitDestination - transform.position;
        float dotProduct = Vector3.Dot(directionToDestination, transform.position);
        /// if dotProduct is bigger than 0, it means that we are still going towards this surface/destination.
        /// if dotProduct is smaller than 0, it means that we have passed the destination.
        if (dotProduct < 0)
            return true;

        return false;
    }

}