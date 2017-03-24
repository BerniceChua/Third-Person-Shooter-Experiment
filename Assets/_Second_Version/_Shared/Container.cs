using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Container : MonoBehaviour {

    private class ContainerItem {
        public Guid ID;         // id #
        public String Name;     // name that the container is called
        public int Maximum;     // max amount of items that it can hold

        private int amountTaken;

        public ContainerItem() {
            ID = Guid.NewGuid();
        }

        public int Remaining { get { return Maximum - amountTaken; } }

        public int Get(int value) {
            if ( (amountTaken + value) > Maximum) {
                int tooMuch = (amountTaken + value) - Maximum;
                amountTaken = Maximum;

                return value - tooMuch;
            }
            amountTaken += value;

            return value;
        }

        public void Put(int value) {
            //amountTaken 
        }
    }

    //List<ContainerItem> items;
    List<ContainerItem> m_items { get { return new List<ContainerItem>(); } set { m_items = value; } }

    // Use this for initialization
    void Awake () {
        //m_items = new List<ContainerItem>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public Guid Add(String name, int max) {
        m_items.Add(new ContainerItem {
            Maximum = max,
            Name = name
        });

        return m_items.Last().ID;
    }

    public int TakeFromContainer(Guid id, int amount) {
        ContainerItem containerItem = m_items.Where(x => x.ID == id).FirstOrDefault();

        if (containerItem == null)
            return -1;  // if less than one, that means the container item isn't in the list.

        return containerItem.Get(amount);
    }

}