using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAimWithIK : MonoBehaviour {

    [SerializeField] float m_minAngle;
    [SerializeField] float m_maxAngle;

    [SerializeField] Transform m_playerDirection;
    [SerializeField] DynamicCrosshairs m_dynamicCrosshair;

    public void m_SetRotation(float rotationAmount) {
        float clampedAngle = GetClampedAngle(rotationAmount);

        transform.eulerAngles = new Vector3(clampedAngle, transform.eulerAngles.y, transform.eulerAngles.z);
    }

    private float GetClampedAngle(float rotationAmount) {
        float newAngle = CheckAngle(transform.eulerAngles.x - rotationAmount);
        float clampedAngle = Mathf.Clamp(newAngle, m_minAngle, m_maxAngle);
        return clampedAngle;
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

    /// <summary>
    /// This allows the player's gun to aim at the crosshair, not just the projectiles.
    /// </summary>
    public void GetCrosshairPosition() {
        m_playerDirection.LookAt(m_dynamicCrosshair.transform);
    }

}
