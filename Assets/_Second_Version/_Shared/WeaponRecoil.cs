using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Shooter))]
public class WeaponRecoil : MonoBehaviour {

    [System.Serializable] public struct Layer {
        public AnimationCurve curve;
        public Vector3 direction;
    }

    [SerializeField] Layer[] layers;

    [SerializeField] float m_recoilSpeed;

    [SerializeField] float m_recoilCooldown;

    [SerializeField] float m_strengthOfRecoil;

    float m_nextRecoilCooldown;
    float m_recoilActiveTime;

    Shooter m_shooter;
    public Shooter Shooter {
        get {
            if (m_shooter == null)
                m_shooter = GetComponent<Shooter>();
            return m_shooter;
        } set {
            m_shooter = value;
        }
    }
    
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (m_nextRecoilCooldown > Time.time) {
            /// if holding the fire button, add to the recoilActiveTime
            m_recoilActiveTime += Time.deltaTime;
            float percentage = Mathf.Clamp01(m_recoilActiveTime / m_recoilSpeed);

            Vector3 recoilAmount = Vector3.zero; /// direction of the recoil, so when we're firing, it will always start at zero, then add the value to it

            /// Look through all the layers
            for (int i = 0; i < layers.Length; i++) {
                recoilAmount += layers[i].direction * layers[i].curve.Evaluate(percentage);
            }

            this.Shooter.m_AimTargetOffset = Vector3.Lerp(Shooter.m_AimTargetOffset, Shooter.m_AimTargetOffset + recoilAmount, m_strengthOfRecoil * Time.deltaTime);
        }
        else {
            /// if not holding the fire button, reduce recoilActiveTime.
            m_recoilActiveTime -= Time.deltaTime;

            if (m_recoilActiveTime < 0)
                m_recoilActiveTime = 0; /// don't allow recoilActiveTime to go below zero

            if (m_recoilActiveTime == 0)
                this.Shooter.m_AimTargetOffset = Vector3.zero;  /// don't add any aim offsets to target if no recoil or if the recoil is not in effect
        }
	}

    public void ActivateCooldown() {
        m_nextRecoilCooldown = Time.time + m_recoilCooldown;
    }
}