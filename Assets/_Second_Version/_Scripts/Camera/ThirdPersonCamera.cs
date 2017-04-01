using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour {

    [SerializeField] Vector3 m_cameraOffset;
    [SerializeField] float m_damping;

    Transform m_cameraLookTarget;
    Player m_localPlayer;

	// Use this for initialization
	void Awake () {
        GameManager.GameManagerInstance.OnLocalPlayerJoined += HandleOnLocalPlayerJoined;
	}

    /// <summary>
    ///  Attaches player to main camera.
    /// </summary>
    /// <param name="player"></param>
    void HandleOnLocalPlayerJoined(Player player) {
        m_localPlayer = player;
        m_cameraLookTarget = m_localPlayer.transform.Find("CameraLookTargetGameObject");

        if (!m_cameraLookTarget)
            m_cameraLookTarget = m_localPlayer.transform;
    }

    // Update is called once per frame
    void Update () {
        if (!m_localPlayer)
            return;

        // sets target of camera to be forward of local player and adds the camera offset.  (If z is negative, it will go behind the player.)
        Vector3 targetPosition = (m_cameraLookTarget.position + m_localPlayer.transform.forward * m_cameraOffset.z) + (m_localPlayer.transform.up * m_cameraOffset.y) + (m_localPlayer.transform.right * m_cameraOffset.x);

        Quaternion targetRotation = Quaternion.LookRotation(m_cameraLookTarget.position - targetPosition, Vector3.up);

        transform.position = Vector3.Lerp(transform.position, targetPosition, m_damping * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, m_damping * Time.deltaTime);
	}
}
