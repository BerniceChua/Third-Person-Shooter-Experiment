using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioController : MonoBehaviour {

    [SerializeField] AudioClip[] m_audioClips;
    [SerializeField] float m_timeDelayBetweenSounds;

    bool m_canPlay;

    //AudioSource m_audSource;
    AudioSource m_audSource { get { return GetComponent<AudioSource>(); } set { m_audSource = value; } }

    // Use this for initialization
    void Start () {
        //m_audSource = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Play() {
        if (!m_canPlay)
            return;

        // Resets m_canPlay to true again, once the m_timeDelayBetweenSounds has passed.
        GameManager.GameManagerInstance.Timer.Add( () => {
            m_canPlay = true; }, 
            m_timeDelayBetweenSounds);

        m_canPlay = false;

        int clipIndex = Random.Range(0, m_audioClips.Length);
        AudioClip selectedClip = m_audioClips[clipIndex];

        // Plays the selected clip
        m_audSource.PlayOneShot(selectedClip);
    }

}