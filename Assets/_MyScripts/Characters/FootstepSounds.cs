using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepSounds : MonoBehaviour {
    public TextureType[] m_textureTypes;
    public AudioSource m_audSource;

    SoundController m_soundControl;

	// Use this for initialization
	void Start () {
        GameObject check = GameObject.FindGameObjectWithTag("SoundController");

        if (check != null) {
            m_soundControl = check.GetComponent<SoundController>();
        }
        m_soundControl = GameObject.FindGameObjectWithTag("SoundController").GetComponent<SoundController>();
    }
	
    void PlayFootstepSound() {
        /// we're using raycasts because we want to raycast to the bottom of our feet
        RaycastHit hit;

        /// we are not raycasting from the character's feet because if we are level on the ground,
        /// the character would raycast BELOW the ground. We need to raycast ABOVE the ground.
        Vector3 start = transform.position + transform.up;
        Vector3 dir = Vector3.down;

        if (Physics.Raycast(start, dir, out hit, 1.3f)) {
            if (hit.collider.GetComponent<MeshRenderer>()) {
                PlayMeshSound(hit.collider.GetComponent<MeshRenderer>());
            }
        }
    }

    void PlayMeshSound(MeshRenderer meshRender) {
        if (m_audSource == null) {
            Debug.LogError("PlayMeshSound -- We have no audio source to play the sound from.");
            return;
        }

        if (m_soundControl == null) {
            Debug.LogError("PlayMeshSound -- No sound manager.");
            return;
        }

        if (m_textureTypes.Length > 0) {
            foreach (TextureType type in m_textureTypes) {
                if (type.footstepSounds.Length == 0) {
                    return;
                }

                foreach (Texture tex in type.textures) {
                    if (meshRender.material.mainTexture == tex) {
                        m_soundControl.PlaySound(m_audSource, type.footstepSounds[Random.Range(0, type.footstepSounds.Length)], true, 1.0f, 1.2f);
                    }
                }
            }
        }
    }

}

[System.Serializable]
public class TextureType {
    public string name;
    public Texture[] textures;
    public AudioClip[] footstepSounds;
}