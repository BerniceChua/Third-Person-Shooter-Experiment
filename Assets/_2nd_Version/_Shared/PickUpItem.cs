using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpItem : MonoBehaviour {

    private void OnTriggerEnter(Collider otherCollider) {
        if (otherCollider.tag != "Player")
            return;

        PickUp(otherCollider.transform);
    }

    public virtual void OnPickUp(Transform item) {
        Debug.Log("Inside OnPickUp(Transform item)...");
    }

    void PickUp(Transform item) {
        Debug.Log("Inside PickUp(Transform item)...");
        OnPickUp(item);
    }

}