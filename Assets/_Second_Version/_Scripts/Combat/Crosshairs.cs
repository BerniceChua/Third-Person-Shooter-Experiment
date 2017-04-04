using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshairs : MonoBehaviour {
    [SerializeField] Texture2D m_image;
    [SerializeField] int m_size;
    
    /// Removed because of refactor from PlayerStateMachine.
    //[SerializeField] float m_maxAngle;
    //[SerializeField] float m_minAngle;

    /// Removed because of refactor from PlayerStateMachine.
    //float m_lookHeight;
    
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /// Removed because of refactor from PlayerStateMachine.
    //public void LookHeight(float heightValue) {
    //    //m_lookHeight += heightValue;

    //    //if (m_lookHeight > m_maxAngle || m_lookHeight < m_minAngle)
    //    //    m_lookHeight -= heightValue;
    //}

    private void OnGUI() {
        /// Because of refactor from PlayerStateMachine, we will draw the crosshair if aiming.
        //if (GameManager.GameManagerInstance.LocalPlayer.PlayerState.m_WeaponState == PlayerStateMachine.EWeaponState.AIMING || GameManager.GameManagerInstance.LocalPlayer.PlayerState.m_WeaponState == PlayerStateMachine.EWeaponState.AIMEDFIRING) {

        //    Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position);
        //    screenPosition.y = Screen.height - screenPosition.y;

        //    /// Removed m_lookHeight because of refactor from PlayerStateMachine.
        //    //GUI.DrawTexture(new Rect(screenPosition.x, screenPosition.y - m_lookHeight, m_size, m_size), m_image);

        //    /// Refactored during 'aiming,_shooting,_and_target'
        //    //GUI.DrawTexture(new Rect(screenPosition.x, screenPosition.y, m_size, m_size), m_image);
        //    GUI.DrawTexture(new Rect(screenPosition.x - m_size/2, screenPosition.y - m_size / 2, m_size, m_size), m_image);
        //}

        if (GameManager.GameManagerInstance.LocalPlayer.PlayerState.m_MoveState != PlayerStateMachine.EMoveState.SPRINTING) {

            Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position);
            screenPosition.y = Screen.height - screenPosition.y;

            /// Removed m_lookHeight because of refactor from PlayerStateMachine.
            //GUI.DrawTexture(new Rect(screenPosition.x, screenPosition.y - m_lookHeight, m_size, m_size), m_image);

            /// Refactored during 'aiming,_shooting,_and_target'
            //GUI.DrawTexture(new Rect(screenPosition.x, screenPosition.y, m_size, m_size), m_image);
            GUI.DrawTexture(new Rect(screenPosition.x - m_size / 2, screenPosition.y - m_size / 2, m_size, m_size), m_image);
        }
    }

}
