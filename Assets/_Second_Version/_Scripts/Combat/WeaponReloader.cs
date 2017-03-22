using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponReloader : MonoBehaviour {
    [SerializeField] int m_MaxAmmo;
    [SerializeField] float m_ReloadTime;
    [SerializeField] int m_ClipSize;

    int m_ammoCount;
    [HideInInspector] public int m_shotsFiredInClip;
    bool m_isReloading;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public int RoundsRemainingInClip {
        get { return m_ClipSize - m_shotsFiredInClip; }
    }

    public bool IsReloading {
        get { return m_isReloading; }
    }

    public void Reload() {
        if (m_isReloading)
            return;

        m_isReloading = true;
        Debug.Log("Reload started at time " + Time.time);
        GameManager.GameManagerInstance.Timer.Add(ExecuteReload, m_ReloadTime);
    }

    void ExecuteReload() {
        Debug.Log("Reload executed at time " + Time.time);

        m_isReloading = false;

        m_ammoCount -= m_shotsFiredInClip;
        m_shotsFiredInClip = 0;

        if (m_ammoCount < 0) {
            m_ammoCount = 0;
            m_shotsFiredInClip += -m_ammoCount;
        }
    }

    public void TakeFromClip(int ammoAmount) {
        m_shotsFiredInClip += ammoAmount;
    }

}