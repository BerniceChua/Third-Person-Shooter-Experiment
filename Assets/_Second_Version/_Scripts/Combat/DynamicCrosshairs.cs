using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicCrosshairs : MonoBehaviour {
    
    [SerializeField] float m_crosshairMoveSpeed;

    public Transform m_Reticule;

    Transform m_crossTop;
    Transform m_crossBottom;
    Transform m_crossLeft;
    Transform m_crossRight;

    float m_reticuleStartPoint;

    // Use this for initialization
    void Start() {
        m_crossTop = m_Reticule.FindChild("CrossGameObject/TopImage").transform;
        m_crossBottom = m_Reticule.FindChild("CrossGameObject/BottomImage").transform;
        m_crossLeft = m_Reticule.FindChild("CrossGameObject/LeftImage").transform;
        m_crossRight = m_Reticule.FindChild("CrossGameObject/RightImage").transform;

        m_reticuleStartPoint = m_crossTop.localPosition.y;
    }

    // Update is called once per frame
    void Update() {
        /// position the crosshair
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position);
        m_Reticule.transform.position = Vector3.Lerp(m_Reticule.transform.position, screenPosition, m_crosshairMoveSpeed * Time.deltaTime);
    }

    public void ApplyScale(float scale) {
        m_crossTop.localPosition = new Vector3(0, m_reticuleStartPoint + scale, 0);
        m_crossBottom.localPosition = new Vector3(0, -m_reticuleStartPoint - scale, 0);
        m_crossLeft.localPosition = new Vector3(-m_reticuleStartPoint - scale, 0, 0);
        m_crossRight.localPosition = new Vector3(m_reticuleStartPoint + scale, 0, 0);
    }

}