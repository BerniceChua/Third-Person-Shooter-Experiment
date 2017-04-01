using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour {

    private class TimedEvent {
        public float TimeToExecute;
        public Callback Method;
    }

    private List<TimedEvent> m_events;

    /// <summary>
    /// Callback() executes when TimedEvent reaches TimeToExecute.
    /// </summary>
    public delegate void Callback();

    private void Awake() {
        m_events = new List<TimedEvent>();
    }

    public void Add(Callback method, float inSeconds) {
        m_events.Add(new TimedEvent {
            Method = method,
            TimeToExecute = Time.time + inSeconds
        });
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (m_events.Count == 0)
            return;

        for (int i = 0; i < m_events.Count; i++) {
            //TimedEvent timedEvent = m_events[i];
            var timedEvent = m_events[i];

            if (timedEvent.TimeToExecute <= Time.time) {
                timedEvent.Method();
                m_events.Remove(timedEvent);
            }
        }
	}

}