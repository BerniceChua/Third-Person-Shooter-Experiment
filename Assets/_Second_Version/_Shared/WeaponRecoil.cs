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

    [SerializeField] float m_strengthOfRecoilVarianceMin;
    [SerializeField] float m_strengthOfRecoilVarianceMax;

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

    DynamicCrosshairs m_crosshair;
    DynamicCrosshairs Crosshair {
        get {
            if (m_crosshair == null)
                /// it is made this way because we only want the crosshairs to appear for the local player.
                m_crosshair = GameManager.GameManagerInstance.LocalPlayer.m_PlayerAim.GetComponentInChildren<DynamicCrosshairs>();

            return m_crosshair;
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
            //float percentage = Mathf.Clamp01(m_recoilActiveTime / m_recoilSpeed);
            float percentage = GetPercentage();

            Vector3 recoilAmount = Vector3.zero; /// direction of the recoil, so when we're firing, it will always start at zero, then add the value to it

            /// Look through all the layers
            for (int i = 0; i < layers.Length; i++) {
                recoilAmount += layers[i].direction * layers[i].curve.Evaluate(percentage);
            }

            this.Shooter.m_AimTargetOffset = Vector3.Lerp(Shooter.m_AimTargetOffset, Shooter.m_AimTargetOffset + recoilAmount, m_strengthOfRecoil * Time.deltaTime);

            /// original version
            //this.Crosshair.ApplyScale(percentage * Random.Range(m_strengthOfRecoil * m_strengthOfRecoilVarianceMin, m_strengthOfRecoil * m_strengthOfRecoilVarianceMax));
            /// my version
            this.Crosshair.ApplyScale(percentage * (m_strengthOfRecoil + Random.Range(m_strengthOfRecoilVarianceMin, m_strengthOfRecoilVarianceMax)));
        }
        else {
            /// if NOT holding the fire button, reduce recoilActiveTime.
            m_recoilActiveTime -= Time.deltaTime;

            if (m_recoilActiveTime < 0)
                m_recoilActiveTime = 0; /// don't allow recoilActiveTime to go below zero

            this.Crosshair.ApplyScale(GetPercentage());

            if (m_recoilActiveTime == 0) {
                this.Shooter.m_AimTargetOffset = Vector3.zero;  /// don't add any aim offsets to target if no recoil or if the recoil is not in effect
                this.Crosshair.ApplyScale(0);  /// won't apply any scaling if not shooting or stopped shooting.
            }

        }
	}

    public void ActivateCooldown() {
        m_nextRecoilCooldown = Time.time + m_recoilCooldown;
    }

    float GetPercentage() {
        return Mathf.Clamp01(m_recoilActiveTime / m_recoilSpeed);
    }

}