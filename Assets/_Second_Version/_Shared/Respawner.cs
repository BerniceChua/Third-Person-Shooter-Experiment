using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawner : MonoBehaviour {

    public void Despawn(GameObject gameObj, float inSeconds) {
        gameObj.SetActive(false);

        GameManager.GameManagerInstance.Timer.Add( () => {
            gameObj.SetActive(true);
        }, inSeconds);
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

}
