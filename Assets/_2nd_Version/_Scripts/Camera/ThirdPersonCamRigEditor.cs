using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CustomEditor(typeof(ThirdPersonCamera))]
public class ThirdPersonCamRigEditor : Editor {

    ThirdPersonCamera m_cameraRig;

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        m_cameraRig = (ThirdPersonCamera)target;

        EditorGUILayout.LabelField("Camera Helper");

        if (GUILayout.Button("Save camera's position now.")) {
            Camera cam = Camera.main;

            if (cam) {
                Transform camT = cam.transform;
                Vector3 camPos = camT.localPosition;
                Vector3 camRight = camPos;
                Vector3 camLeft = camPos;
                camLeft.x = -camPos.x;
                //m_cameraRig.m_cameraSettings.camPositionOffsetRight = camRight;
                //m_cameraRig.m_cameraSettings.camPositionOffsetLeft = camLeft;
            }
        }
    }

}