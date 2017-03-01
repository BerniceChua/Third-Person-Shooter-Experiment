using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(Weapon))]
public class WeaponEditor : Editor {
    Weapon m_weapon;

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        m_weapon = (Weapon)target;

        EditorGUILayout.LabelField("Weapon Helpers");

        if (GUILayout.Button("Save gun equip location.")) {
            Transform weaponT = m_weapon.transform;
            Vector3 weaponPos = weaponT.localPosition;
            Vector3 weaponRot = weaponT.localEulerAngles;
            m_weapon.m_weaponSettings.equiptPosition = weaponPos;
            m_weapon.m_weaponSettings.equiptRotation = weaponRot;
        }

        if (GUILayout.Button("Save gun unequip location.")) {
            Transform weaponT = m_weapon.transform;
            Vector3 weaponPos = weaponT.localPosition;
            Vector3 weaponRot = weaponT.localEulerAngles;
            m_weapon.m_weaponSettings.unequiptPosition = weaponPos;
            m_weapon.m_weaponSettings.unequiptRotation = weaponRot;
        }

        EditorGUILayout.LabelField("Debug Positioning");
        if (GUILayout.Button("Move gun to equip location")) {
            Transform weaponT = m_weapon.transform;
            weaponT.localPosition = m_weapon.m_weaponSettings.equiptPosition;
            Quaternion weaponEulerAngles = Quaternion.Euler(m_weapon.m_weaponSettings.equiptRotation);
            weaponT.localRotation = weaponEulerAngles;
        }

        if (GUILayout.Button("Move gun to unequip location")) {
            Transform weaponT = m_weapon.transform;
            weaponT.localPosition = m_weapon.m_weaponSettings.unequiptPosition;
            Quaternion weaponEulerAngles = Quaternion.Euler(m_weapon.m_weaponSettings.unequiptRotation);
            weaponT.localRotation = weaponEulerAngles;
        }
    }

}
