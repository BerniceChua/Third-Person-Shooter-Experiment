using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EscapeMenu : MonoBehaviour {

    [SerializeField] GameObject m_escapeMenuPanel;

    [SerializeField] Button m_yesButton;
    [SerializeField] Button m_noButton;

    [SerializeField] PauseAndLockCursor m_cursor;

    //private void Awake() {
        
    //}

    // Use this for initialization
    void Start () {
        m_escapeMenuPanel.SetActive(false);
        m_yesButton.onClick.AddListener(OnYesClicked);
        m_noButton.onClick.AddListener(OnNoClicked);
    }
	
	// Update is called once per frame
	void Update () {
        if (m_escapeMenuPanel.activeSelf)
            return;

		if (GameManager.GameManagerInstance.InputController.m_Escape) {
            GameManager.GameManagerInstance.m_PlayerIsPaused = true;
            Cursor.visible = true;
            //m_cursor.PauseGame();
            m_escapeMenuPanel.SetActive(true);
        }
	}

    void OnYesClicked() {
        SceneManager.LoadScene("Second_Version_Main_Menu");
    }

    void OnNoClicked() {
        GameManager.GameManagerInstance.m_PlayerIsPaused = false;
        m_cursor.UnpauseGame();
        m_escapeMenuPanel.SetActive(false);
    }

}