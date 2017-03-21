using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager {

    public event System.Action<Player> OnLocalPlayerJoined;

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

    /// If the player is ready, it will notify the Game Manager.
    /// The Game Manager will have an event.
    /// Any other classes that uses this event will get a notification from the player.
    private Player m_localPlayer;
    public Player LocalPlayer {
        get {
            return m_localPlayer;
        }
        set {
            m_localPlayer = value;
            if (OnLocalPlayerJoined != null) {
                OnLocalPlayerJoined(m_localPlayer);
            }
        }
    }

}