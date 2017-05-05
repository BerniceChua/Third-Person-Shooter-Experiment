using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour {

    [System.Serializable]
    public class CameraRig {
        public Vector3 CameraOffset;
        public float CrouchHeight;
        public float Damping;
    }
    /// <summary>
    /// Default values of Third Person Camera script:
    ///     Camera Offset = 1, 1, -6;
    ///     Damping = 20;
    /// </summary>

    /// m_cameraOffset & m_damping replaced by 'public class CameraRig'
    //[SerializeField] Vector3 m_cameraOffset;
    //[SerializeField] float m_damping;

    [SerializeField] CameraRig m_defaultCamera;
    [SerializeField] CameraRig m_aimCamera;
    
    Transform m_cameraLookTarget;
    Player m_localPlayer;

	// Use this for initialization
	void Awake () {
        print("Inside Awake() of ThirdPersonCamera.cs at time " + Time.time);
        GameManager.GameManagerInstance.OnLocalPlayerJoined += HandleOnLocalPlayerJoined;
	}

    /// <summary>
    ///  Attaches player to main camera.
    /// </summary>
    /// <param name="player"></param>
    void HandleOnLocalPlayerJoined(Player player) {
        print("Inside HandleOnLocalPlayerJoined(Player player) of ThirdPersonCamera.cs at time " + Time.time);
        m_localPlayer = player;
        //m_cameraLookTarget = m_localPlayer.transform.Find("CameraLookTargetGameObject");
        m_cameraLookTarget = m_localPlayer.transform.Find("AimPivotGameObject");

        if (!m_cameraLookTarget)
            m_cameraLookTarget = m_localPlayer.transform;
    }

    // Update is called once per frame
    //void Update () {
    /// Changed this to LateUpdate(), because LateUpdate() smooths the camera movement better since we changed from MoveController.cs to CharacterController.
    void LateUpdate() {
        if (!m_localPlayer)
            return;

        CameraRig camRig = m_defaultCamera;

        if (m_localPlayer.PlayerState.m_WeaponState == PlayerStateMachine.EWeaponState.AIMING || m_localPlayer.PlayerState.m_WeaponState == PlayerStateMachine.EWeaponState.AIMEDFIRING)
            camRig = m_aimCamera;

        // sets target of camera to be forward of local player and adds the camera offset.  (If z is negative, it will go behind the player.)
        //Vector3 targetPosition = (m_cameraLookTarget.position + m_localPlayer.transform.forward * m_cameraOffset.z) + (m_localPlayer.transform.up * m_cameraOffset.y) + (m_localPlayer.transform.right * m_cameraOffset.x);
        // refactored after started to use the PlayerStateMachine.
        float targetHeight = camRig.CameraOffset.y + (m_localPlayer.PlayerState.m_MoveState == PlayerStateMachine.EMoveState.CROUCHING ? camRig.CrouchHeight : 0);
        //Vector3 targetPosition = (m_cameraLookTarget.position + m_localPlayer.transform.forward * camRig.CameraOffset.z) + 
        //    (m_localPlayer.transform.up * (camRig.CameraOffset.y + targetHeight) ) + 
        //    (m_localPlayer.transform.right * camRig.CameraOffset.x);
        Vector3 targetPosition = (m_cameraLookTarget.position + m_localPlayer.transform.forward * camRig.CameraOffset.z) +
            (m_localPlayer.transform.up * targetHeight) +
            (m_localPlayer.transform.right * camRig.CameraOffset.x);

        Vector3 collisionDestination = m_cameraLookTarget.position + m_localPlayer.transform.up * targetHeight - m_localPlayer.transform.forward*0.005f;
        Debug.DrawLine(targetPosition, collisionDestination, Color.blue);

        /// the 'ref' part means pass by reference, instead of pass by value,
        /// so that we don't need to write it like this: targetPosition = HandleCameraCollision(ref targetPosition, collisionDestination);
        HandleCameraCollision(collisionDestination, ref targetPosition);

        //Quaternion targetRotation = Quaternion.LookRotation(m_cameraLookTarget.position - targetPosition, Vector3.up);
        Quaternion targetRotation = m_cameraLookTarget.rotation;

        //transform.position = Vector3.Lerp(transform.position, targetPosition, m_damping * Time.deltaTime);
        //transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, m_damping * Time.deltaTime);
        transform.position = Vector3.Lerp(transform.position, targetPosition, camRig.Damping * Time.deltaTime);
        //transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, camRig.Damping * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, m_cameraLookTarget.rotation, camRig.Damping * Time.deltaTime);
    }

    private static Vector3 HandleCameraCollision(Vector3 toCollisionTargetPoint, ref Vector3 fromTargetPosition) {
        RaycastHit hit;
        if (Physics.Linecast(toCollisionTargetPoint, fromTargetPosition, out hit)) {
            Vector3 pointOfHit = new Vector3(hit.point.x + hit.normal.x * 0.2f, hit.point.y, hit.point.z + hit.normal.z * 0.2f);
            fromTargetPosition = new Vector3(pointOfHit.x, fromTargetPosition.y, pointOfHit.z);
        }

        return fromTargetPosition;
    }
}