using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// EventBus.cs can be used to listen to ANY event, and any
/// class that is using EventBus to listen to an event can
/// trigger something.
/// </summary>

public class EventBus {

    public class EventListener {
        public delegate void Callback();
        public bool IsSingleShot;
        public Callback Method;

        public EventListener() {
            IsSingleShot = false;
        }
    }

    private Dictionary<string, IList<EventListener>> m_eventTable;
    private Dictionary<string, IList<EventListener>> EventTable {
        get {
            if (m_eventTable == null)
                m_eventTable = new Dictionary<string, IList<EventListener>>();

            return m_eventTable;
        }
    }

    public void AddListener(string name, EventListener listener) {
        if (!EventTable.ContainsKey(name))
            EventTable.Add(name, new List<EventListener>());

        if (EventTable[name].Contains(listener)) {
            return;
        }

        EventTable[name].Add(listener);
    }

    public void RaiseEvent(string name) {
        if (!EventTable.ContainsKey(name))
            return;

        /// Loop through all the items inside EventTable List<> with the key "name"
        /// For loop, so that the items can be removed from the list if not making a single shot (IsSingleShot == false).
        for (int i = 0; i < EventTable[name].Count; i++) {
            EventListener listener = EventTable[name][i];
            listener.Method();

            /// If an EventListener has a single shot, remove it.
            if (listener.IsSingleShot)
                EventTable[name].Remove(listener);
        }
    }

}