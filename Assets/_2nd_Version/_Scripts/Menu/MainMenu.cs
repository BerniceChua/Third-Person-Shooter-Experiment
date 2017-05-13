using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

    [SerializeField] Button m_startGameButton;
    [SerializeField] Button m_quitGameButton;

    public string m_LevelName;
    public Scene m_scene;

    // Use this for initialization
    void Start () {
        m_startGameButton.onClick.AddListener(() => {
            StartGame(m_LevelName);
        });

        m_quitGameButton.onClick.AddListener(QuitGame);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void StartGame(string levelName) {
        SceneManager.LoadScene(levelName);
    }

    public void QuitGame() {
        Application.Quit();
    }

}
