using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour {

    private void OnDrawGizmos() {
        Gizmos.color = Color.blue;

        /// this allows us to rotate the gizmo in the editor
        Gizmos.matrix = transform.localToWorldMatrix;

        /// Vector3.zero allows it to use the Gizmos.matrix.
        /// 1st Vector3.up*1 moves the Gizmo upwards.
        /// Vector3.one is the same as Vector3(1,1,1), which is a cube.
        /// 2nd Vector3.up*1 increases the height of the Gizmo.
        Gizmos.DrawWireCube(Vector3.zero + Vector3.up*1, Vector3.one + Vector3.up * 1);
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
