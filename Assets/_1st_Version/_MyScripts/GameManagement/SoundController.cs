using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SoundController : MonoBehaviour {

    public void PlaySound(AudioSource audSource, AudioClip audClip, bool randomizePitch = false, float randomPitchMin = 1, float randomPitchMax = 1) {
        audSource.clip = audClip;

        if (randomizePitch == true) {
            audSource.pitch = Random.Range(randomPitchMin, randomPitchMax);
        }

        audSource.Play();
    }

    // instantiates the clips, because playing the clip and having it loop will cut the last clip
    // this allows us to play the audio clip at the point when we instantiate it.
    public void InstantiateClip(Vector3 pos, AudioClip audClip, float time = 2.0f, bool randomizePitch = false, float randomPitchMin = 1, float randomPitchMax = 1) {
        GameObject clone = new GameObject("one shot audio");
        clone.transform.position = pos;
        AudioSource audio = clone.AddComponent<AudioSource>();
        audio.spatialBlend = 1;
        audio.clip = audClip;
        audio.Play();

        Destroy(clone, time);
    }

}