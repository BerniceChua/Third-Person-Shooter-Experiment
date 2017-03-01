using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// be careful with [ExecuteInEditMode] and make sure there are no endless loops!
[ExecuteInEditMode]
public class CameraRig : MonoBehaviour {
    public Transform m_target;
    public bool m_autoTargetPlayer;
    public LayerMask m_wallLayers;

    public enum Shoulder {
        Right,
        Left
    }

    public Shoulder m_shoulder;

    [System.Serializable]
    public class CameraSettings {
        [Header("-Positioning-")]
        public Vector3 camPositionOffsetLeft;
        public Vector3 camPositionOffsetRight;

        [Header("-Camera Options-")]
        public Camera UICamera;

        public float mouseXSensitivity = 2.0f;
        public float mouseYSensitivity = 2.0f;
        //public float minCamRotateAngle = -30.0f;
        public float minCamRotateAngle = -70.0f;
        public float maxCamRotateAngle = 70.0f;
        public float rotationSpeed = 5.0f;
        public float maxCheckDist = 0.1f;

        [Header("-Zoom-")]
        public float defaultFieldOfView = 70.0f;
        public float zoomFieldOfView = 30.0f;
        public float zoomSpeed = 3.0f;

        // hides mesh from camera if you're too close to the mesh
        [Header("-Visual Options-")]
        public float hideMeshWithinDistance = 0.5f;
    }
    [SerializeField] public CameraSettings m_cameraSettings;

    [System.Serializable]
    public class InputSettings {
        public string verticalAxis = "Mouse X";
        public string horizontalAxis = "Mouse Y";
        public string aimButton = "Fire2";
        public string switchShoulderButton = "Fire4";
    }
    [SerializeField] public InputSettings m_input;

    [System.Serializable]
    public class MovementSettings {
        public float movementLerpSpeed = 5.0f;
    }
    [SerializeField] public MovementSettings m_movement;

    Transform m_pivot;
    Camera m_mainCamera;

    // values that will be passed when we move the mouse
    float m_newX = 0.0f;
    float m_newY = 0.0f;

    // Use this for initialization
    void Start () {
        if (GameObject.FindGameObjectWithTag("Player"))
            m_autoTargetPlayer = true;

        if (Camera.main)
            m_mainCamera = Camera.main;
        else
            Debug.Log("No main camera component available.  Please add a camera component.");

        // gets 1st child of camera rig.
        m_pivot = transform.GetChild(0);
	}
	
	// Update is called once per frame
	void Update () {
        if (m_target) {
            if (Application.isPlaying) {
                RotateCamera();
                CheckWall();
                CheckMeshRenderer();
                Zoom(Input.GetButton(m_input.aimButton));

                if (Input.GetButtonDown(m_input.switchShoulderButton)) {
                    SwitchShoulders();
                }
            }
        }

    }

    // all the functionality for following the player
    private void LateUpdate() {
        if (!m_target)
            TargetPlayer();
        else {
            Vector3 targetPosition = m_target.position;
            Quaternion targetRotation = m_target.rotation;

            FollowTarget(targetPosition, targetRotation);
        }
    }

    // find the player GameObject and sets it as target
    void TargetPlayer() {
        if (m_autoTargetPlayer) {
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (player) {
                Transform playerT = player.transform;
                m_target = playerT;
            }
        }
    }

    // follow target with Time.deltaTime smoothly
    void FollowTarget(Vector3 targetPosition, Quaternion targetRotation) {
        if (!Application.isPlaying) {
            transform.position = targetPosition;
            transform.rotation = targetRotation;
        } else {
            Vector3 newPos = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * m_movement.movementLerpSpeed);
            transform.position = newPos;
        }
    }

    private void RotateCamera() {
        if (!m_pivot)
            return;

        m_newX += m_cameraSettings.mouseXSensitivity * Input.GetAxis(m_input.verticalAxis);
        m_newY += m_cameraSettings.mouseYSensitivity * Input.GetAxis(m_input.horizontalAxis);

        Vector3 eulerAngleAxis = new Vector3();
        eulerAngleAxis.x = -m_newY;
        eulerAngleAxis.y = m_newX;

        m_newX = Mathf.Repeat(m_newX, 360);
        m_newY = Mathf.Clamp(m_newY, m_cameraSettings.minCamRotateAngle, m_cameraSettings.maxCamRotateAngle);

        // .localRotation because the pivot is a child object of the CameraRig, so we want to get its local rotation in relation to the CameraRig parent object.
        Quaternion newRotation = Quaternion.Slerp(m_pivot.localRotation, Quaternion.Euler(eulerAngleAxis), Time.deltaTime * m_cameraSettings.rotationSpeed);

        m_pivot.localRotation = newRotation;
    }

    // prevents camera from clipping through a wall
    private void CheckWall() {
        if (!m_pivot || !m_mainCamera)
            return;

        RaycastHit hit;

        Transform mainCamTransform = m_mainCamera.transform;
        Vector3 mainCamPosition = mainCamTransform.position;
        Vector3 pivotPosition = m_pivot.position;

        Vector3 start = pivotPosition;
        Vector3 direction = mainCamPosition - pivotPosition;

        float distanceBetween2Cams = Mathf.Abs(m_shoulder == Shoulder.Left ? m_cameraSettings.camPositionOffsetLeft.z : m_cameraSettings.camPositionOffsetRight.z);

        if (Physics.SphereCast(start, m_cameraSettings.maxCheckDist, direction, out hit, distanceBetween2Cams, m_wallLayers)) {
            MoveCameraUp(hit, pivotPosition, direction, mainCamTransform);
        } else {
            switch (m_shoulder) {
                case Shoulder.Left:
                    PositionCamera(m_cameraSettings.camPositionOffsetLeft);
                    break;
                case Shoulder.Right:
                    PositionCamera(m_cameraSettings.camPositionOffsetRight);
                    break;
            }
        }
    }

    // moves camera forward if it hits a wall
    private void MoveCameraUp(RaycastHit hit, Vector3 pivotPosition, Vector3 direction, Transform mainCamTransform) {
        float hitDistance = hit.distance;

        // returns center of the spherecast so camera can move to middle so it won't clip through walls.
        Vector3 sphereCastCenter = pivotPosition + (direction.normalized * hit.distance);

        Vector3 cameraTPose = mainCamTransform.position;

        mainCamTransform.position = sphereCastCenter;
    }

    // positions camera's localPosition to a given location
    void PositionCamera(Vector3 camPositionOffset) {
        if (!m_mainCamera)
            return;

        // gets info about the camera
        Transform mainCamT = m_mainCamera.transform;
        Vector3 mainCamPos = mainCamT.localPosition;

        // calculates the camera's linear extrapolation to the new position
        Vector3 newPos = Vector3.Lerp(mainCamPos, camPositionOffset, Time.deltaTime * m_movement.movementLerpSpeed);

        // replace the camera's current .localPosition with the calculated linear extrapolation.
        mainCamT.localPosition = newPos;
    }

    // hides the target mesh renderers when it's too close
    void CheckMeshRenderer() {
        if (!m_mainCamera || !m_target)
            return;

        SkinnedMeshRenderer[] meshes = m_target.GetComponentsInChildren<SkinnedMeshRenderer>();
        Transform mainCamT = m_mainCamera.transform;
        Vector3 mainCamPos = mainCamT.position;
        Vector3 targetPos = m_target.position;
        float dist = Vector3.Distance(mainCamPos, targetPos + m_target.up);

        if (meshes.Length > 0) {
            for (int i = 0; i < meshes.Length; i++) {
                if (dist <= m_cameraSettings.hideMeshWithinDistance) {
                    meshes[i].enabled = false;
                } else {
                    meshes[i].enabled = true;
                }
            }
        }
    }

    void Zoom(bool isZooming) {
        if (!m_mainCamera)
            return;

        if (isZooming) {
            float newFieldOfView = Mathf.Lerp(m_mainCamera.fieldOfView, m_cameraSettings.zoomFieldOfView, Time.deltaTime * m_cameraSettings.zoomSpeed);
            m_mainCamera.fieldOfView = newFieldOfView;

            if (m_cameraSettings.UICamera != null) {
                m_cameraSettings.UICamera.fieldOfView = newFieldOfView;
            }
        } else {
            float originalFieldOfView = Mathf.Lerp(m_mainCamera.fieldOfView, m_cameraSettings.defaultFieldOfView, Time.deltaTime * m_cameraSettings.zoomSpeed);
            m_mainCamera.fieldOfView = originalFieldOfView;

            if (m_cameraSettings.UICamera != null) {
                m_cameraSettings.UICamera.fieldOfView = originalFieldOfView;
            }
        }
    }

    // switch the camera shoulder view
    public void SwitchShoulders() {
        switch (m_shoulder) {
            case Shoulder.Left:
                m_shoulder = Shoulder.Right;
                break;
            case Shoulder.Right:
                m_shoulder = Shoulder.Left;
                break;
        }
    }

}