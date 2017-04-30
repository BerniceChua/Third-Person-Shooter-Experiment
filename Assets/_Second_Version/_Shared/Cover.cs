using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cover : MonoBehaviour {

	[SerializeField] Collider m_trigger;
    PlayerCover m_playerCover;
 
    // Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private bool IsLocalPlayer(Collider other) {
        if (other.tag != "Player")
            return false;

        if (other.GetComponent<Player>() != GameManager.GameManagerInstance.LocalPlayer)
            return false;

        /// this is what you would use if you don't intend to extend this to multiplayer later on
        /// instead of GameManager.GameManagerInstance.LocalPlayer
        //other.gameObject.GetComponent<PlayerCover>();
        m_playerCover = GameManager.GameManagerInstance.LocalPlayer.GetComponent<PlayerCover>();

        m_playerCover.SetPlayerCoverAllowed(true);

        return true;
    }
       
    private void OnTriggerEnter(Collider other) {
        if (!IsLocalPlayer(other))
            return;

        m_playerCover.SetPlayerCoverAllowed(true);
    }

    private void OnTriggerExit(Collider other) {
        if (!IsLocalPlayer(other))
            return;

        m_playerCover.SetPlayerCoverAllowed(false);
    }

}