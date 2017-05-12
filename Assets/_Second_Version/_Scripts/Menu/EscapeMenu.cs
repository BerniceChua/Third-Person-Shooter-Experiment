using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EscapeMenu : MonoBehaviour {

    [SerializeField] GameObject EscapeMenuPanel;

    [SerializeField] Button m_yesButton;
    [SerializeField] Button m_noButton;

    [SerializeField] PauseAndLockCursor m_cursor;

    //private void Awake() {
        
    //}

    // Use this for initialization
    void Start () {
        EscapeMenuPanel.SetActive(false);
        m_yesButton.onClick.AddListener(OnYesClicked);
        m_noButton.onClick.AddListener(OnNoClicked);
    }
	
	// Update is called once per frame
	void Update () {
        if (EscapeMenuPanel.activeSelf)
            return;

		if (GameManager.GameManagerInstance.InputController.m_Escape) {
            GameManager.GameManagerInstance.m_PlayerIsPaused = true;
            Cursor.visible = true;
            //m_cursor.PauseGame();
            EscapeMenuPanel.SetActive(true);
        }
	}

    void OnYesClicked() {
        SceneManager.LoadScene("Second_Version_Main_Menu");
    }

    void OnNoClicked() {
        GameManager.GameManagerInstance.m_PlayerIsPaused = false;
        EscapeMenuPanel.SetActive(false);
    }

}