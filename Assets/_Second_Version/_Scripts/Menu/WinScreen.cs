using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WinScreen : MonoBehaviour {

    [SerializeField] GameObject m_winMessagePanel;

    [SerializeField] Button m_playAgainButton;
    [SerializeField] Button m_returnToMainMenuButton;

    [SerializeField] PauseAndLockCursor m_cursor;

    //private void Awake() {
        
    //}

    // Use this for initialization
    void Start () {
        m_winMessagePanel.SetActive(false);
        GameManager.GameManagerInstance.EventBus.AddListener("OnAllEnemiesKilled", () => {
            GameManager.GameManagerInstance.Timer.Add(() => {
                GameManager.GameManagerInstance.m_PlayerIsPaused = true;
                Cursor.visible = true;
                //m_cursor.PauseGame();
                m_winMessagePanel.SetActive(true);
            }, 5);
        });

        m_playAgainButton.onClick.AddListener(OnPlayAgainClicked);
        m_returnToMainMenuButton.onClick.AddListener(OnReturnToMainMenuButtonClicked);
    }
	
	// Update is called once per frame
	void Update () {

    }

    void OnPlayAgainClicked() {
        SceneManager.LoadScene("Second_Version");
    }

    void OnReturnToMainMenuButtonClicked() {
        SceneManager.LoadScene("Second_Version_Main_Menu");
    }

}