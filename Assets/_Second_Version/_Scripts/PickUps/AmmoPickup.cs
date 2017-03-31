using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : PickUpItem {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void OnPickUp(Transform item) {
        //base.OnPickUp(item);
        Debug.Log("Inside the public override void OnPickUp(Transform item)....");
    }

}