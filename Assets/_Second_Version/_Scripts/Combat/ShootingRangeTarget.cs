using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingRangeTarget : Destructable {

    [SerializeField] float m_rotationSpeed;
    [SerializeField] float m_repairTime;

    Quaternion m_initialRotation;
    Quaternion m_targetRotation;
    bool m_requiresRotation;

	// Use this for initialization
	void Awake () {
		m_initialRotation = transform.rotation;
	}
	
	// Update is called once per frame
	void Update () {
        /// If the targetRotation is 90 degrees from the initial rotation, it will flip backwards.
        /// If not, it will go back to the original rotation.
        if (!m_requiresRotation)
            return;

        transform.rotation = Quaternion.Lerp(transform.rotation, m_targetRotation, m_rotationSpeed * Time.deltaTime);

        /// check if transform's rotation is same as target rotation, so it won't 
        /// keep running transform.rotation = Quaternion.Lerp(transform.rotation, m_targetRotation, m_rotationSpeed * Time.deltaTime);
        if (transform.rotation == m_targetRotation)
            m_requiresRotation = false;
	}

    public override void Die() {
        base.Die();

        /// transform.right means you are making a right angle.  '* 90' means you are multiplying this right angle by 90.
        m_targetRotation = Quaternion.Euler(transform.right * 90);
        m_requiresRotation = true;
        GameManager.GameManagerInstance.Timer.Add(() => {
            m_targetRotation = m_initialRotation;
            m_requiresRotation = true;
        }, m_repairTime);
    }

}