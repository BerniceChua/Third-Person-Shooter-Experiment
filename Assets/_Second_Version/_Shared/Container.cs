using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Container : MonoBehaviour {

    [System.Serializable]
    public class ContainerItem {
        public Guid ID;         // id #
        public String Name;     // name that the container is called
        public int Maximum;     // max amount of items that it can hold

        public int amountTaken;

        public ContainerItem() {
            ID = Guid.NewGuid();
        }

        // Tells how much items are left in the container.
        public int Remaining { get { return Maximum - amountTaken; } set { Remaining = value; } }

        public int Get(int value) {
            if ( (amountTaken + value) > Maximum) {
                int tooMuch = (amountTaken + value) - Maximum;
                amountTaken = Maximum;  // amount taken is already at Maximum

                return value - tooMuch;
            }
            amountTaken += value;

            return value;
        }

        public void Put(int value) {
            //amountTaken 
        }
    }

    // [SerializeField] does not make List<>s appear in the inspector.
    //[SerializeField] List<ContainerItem> m_items;
    public List<ContainerItem> m_items;
    //public List<ContainerItem> m_items { get { return new List<ContainerItem>(); } set { m_items = value; } }

    public event System.Action OnContainerReady;

    // Use this for initialization
    void Awake () {
        m_items = new List<ContainerItem>();

        if (OnContainerReady != null) {
            Debug.Log("Initializing OnContainerReady();");
            OnContainerReady();
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public Guid Add(String name, int max) {
        m_items.Add(new ContainerItem {
            // ID was moved to ContainerItem() in the constructor.
            //ID = Guid.NewGuid(),

            Maximum = max,
            Name = name
        });

        return m_items.Last().ID;
    }

    public int TakeFromContainer(Guid id, int amount) {
        //ContainerItem containerItem = m_items.Where(x => x.ID == id).FirstOrDefault();
        var containerItem = m_items.Where(x => x.ID == id).FirstOrDefault();

        if (containerItem == null)
            return -1;  // if less than one, that means the container item isn't in the list.

        return containerItem.Get(amount);
    }

}