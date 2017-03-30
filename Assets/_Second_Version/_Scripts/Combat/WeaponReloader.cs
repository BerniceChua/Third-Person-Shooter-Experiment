using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponReloader : MonoBehaviour {
    [SerializeField] int m_maxAmmo;
    [SerializeField] float m_reloadTime;
    [SerializeField] int m_clipSize;

    [SerializeField] Container m_inventory;

    // removed in refactor because m_ammoCount was replaced by putting a parameter in ExecuteReload();
    //int m_ammoCount;

    /*[HideInInspector]*/ public int m_shotsFiredInClip;
    [SerializeField] bool m_isReloading;

    System.Guid m_containerItemId;
    /// Cannot use the "get-set" design pattern on this, because
    ///    #1) the m_containerItemId will not appear in the inspector right away as soon as the game starts.  It only puts this in the inspector when you reload.
    ///    #2) after you first reload, each time you reload, it adds a new "m_containerItemId = m_inventory.Add(this.name, m_maxAmmo);" instead of reusing the current one.
    //System.Guid m_containerItemId { get { return m_inventory.Add(this.name, m_maxAmmo); } set { m_containerItemId = value; } }

    public event System.Action OnAmmoChanged;

    public int RoundsRemainingInClip {
        get { return m_clipSize - m_shotsFiredInClip; }
    }

    public int RoundsRemainingInInventory {
        get { return m_inventory.GetAmountRemaining(m_containerItemId); }
    }

    public bool IsReloading {
        get { return m_isReloading; }
    }

    // Use this for initialization
    void Awake () {
        m_inventory.OnContainerReady += () => {
            m_containerItemId = m_inventory.Add(this.name, m_maxAmmo);
        };
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Reload() {
        if (m_isReloading)
            return;

        m_isReloading = true;
        //Debug.Log("Reload started at time " + Time.time);

        int amountFromInventory = m_inventory.TakeFromContainer(m_containerItemId, m_clipSize - RoundsRemainingInClip);

        GameManager.GameManagerInstance.Timer.Add(() => { ExecuteReload(amountFromInventory); }, m_reloadTime);

    }

    //void ExecuteReload() {
    //    Debug.Log("Reload executed at time " + Time.time);

    //    m_isReloading = false;

    //    m_ammoCount -= m_shotsFiredInClip;
    //    m_shotsFiredInClip = 0;

    //    if (m_ammoCount < 0) {
    //        m_ammoCount = 0;
    //        m_shotsFiredInClip += -m_ammoCount;
    //    }
    //}

    void ExecuteReload(int amountToReload) {
        Debug.Log("Reload executed at time " + Time.time);
        Debug.Log("amountToReload = " + amountToReload);

        m_isReloading = false;

        m_shotsFiredInClip -= amountToReload;

        /// The if-statement was removed because the logic was already handled by 
        /// () => { ExecuteReload(amountFromInventory); } in 
        /// GameManager.GameManagerInstance.Timer.Add(() => { ExecuteReload(amountFromInventory); }, m_reloadTime);
        
        if (OnAmmoChanged != null)
            OnAmmoChanged();
    }

    public void TakeFromClip(int ammoAmount) {
        m_shotsFiredInClip += ammoAmount;

        if (OnAmmoChanged != null)
            OnAmmoChanged();
    }

}