using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAim : MonoBehaviour {

    public void m_SetRotation(float rotationAmount) {
        transform.eulerAngles = new Vector3(transform.eulerAngles.x - rotationAmount, transform.eulerAngles.y, transform.eulerAngles.z);
    }

    public float GetAngle() {
        return CheckAngle(transform.eulerAngles.x);
    }

    public float CheckAngle(float value) {
        float angle = value - 180;

        if (angle > 0)
            return angle - 180;

        return angle + 180;
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        //Debug.Log("test at time " + Time.time);
        //Debug.Log("Angle = " + GetAngle() + "at" + Time.time);
	}
}
