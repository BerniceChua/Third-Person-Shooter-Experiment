using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager {

    private GameObject gameObject;

    private static GameManager m_gameManagerInstance;

    public static GameManager GameManagerInstance {
        get {
            if (m_gameManagerInstance == null) {
                m_gameManagerInstance = new GameManager();
                m_gameManagerInstance.gameObject = new GameObject("_gameManager");
                m_gameManagerInstance.gameObject.AddComponent<InputController>();
            }

            return m_gameManagerInstance;
        }
    }

    private InputController m_InputController;
    // not static because only use m_InputController when it's instantiated
    public InputController InputController {
        get {
            if (m_InputController == null)
                m_InputController = gameObject.GetComponent<InputController>();

            return m_InputController;
        }
    }
}