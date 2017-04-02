using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshairs : MonoBehaviour {
    [SerializeField] Texture2D m_image;
    [SerializeField] int m_size;
    [SerializeField] float m_maxAngle;
    [SerializeField] float m_minAngle;

    float m_lookHeight;
    
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void LookHeight(float heightValue) {
        //m_lookHeight += heightValue;

        //if (m_lookHeight > m_maxAngle || m_lookHeight < m_minAngle)
        //    m_lookHeight -= heightValue;
    }

    private void OnGUI() {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position);
        screenPosition.y = Screen.height - screenPosition.y;
        GUI.DrawTexture(new Rect(screenPosition.x, screenPosition.y - m_lookHeight, m_size, m_size), m_image);
    }
}
