using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(CharacterMovement))]
[RequireComponent(typeof(CharacterStats))]
[RequireComponent(typeof(Animator))]
public class EnemyNPCAI : MonoBehaviour {

    private NavMeshAgent m_navMesh;
    private CharacterMovement m_charMove { get { return GetComponent<CharacterMovement>(); } set { m_charMove = value; } }
    private Animator m_animator { get { return GetComponent<Animator>(); } set { m_animator = value; } }

    private CharacterStats m_characterStats { get { return GetComponent<CharacterStats>(); } set { m_characterStats = value; } }

    public enum AIState {
        Patrol,
        Attack,
        FindCover
    }
    [SerializeField] AIState m_AIState;

    [System.Serializable]
    public class PatrolSettings {
        public WaypointBase[] waypoints;
    }
    public PatrolSettings m_patrolSettings;

    [System.Serializable]
    public class EnemySightSettings {
        public float sightRange = 30.0f;
        public LayerMask sightLayers; // allows us to only raycast to a certain layer
        public float fieldOfView = 120f;
        public float eyeHeight;
    }
    [SerializeField] EnemySightSettings m_enemySights;

    private float m_currentWaitTime;
    private int m_waypointIndex;
    private Transform m_currentLookTransform;
    private bool m_isWalkingToDestination;

    private float m_forward;

    private Transform m_target;
    private Vector3 m_targetLastKnownPosition;
    private CharacterStats[] m_allCharacters;

    // Use this for initialization
    void Start () {
        m_navMesh = GetComponentInChildren<NavMeshAgent>();

        if (m_navMesh == null) {
            Debug.LogError("We need a navemesh to traverse the world with.");
            enabled = false;
            return;
        }

        if (m_navMesh.transform == this.transform) {
            Debug.LogError("The NavMeshAgent should be a child of the character: " + gameObject.name);
            enabled = false;
            return;
        }

        m_navMesh.speed = 0;
        m_navMesh.acceleration = 0;
        m_navMesh.autoBraking = false;

        if (m_navMesh.stoppingDistance == 0) {
            Debug.Log("This auto-sets stoppingDistance to 1.3f");
            m_navMesh.stoppingDistance = 1.3f;
        }

        GetAllCharacters();
    }

    void GetAllCharacters() {
        m_allCharacters = GameObject.FindObjectsOfType<CharacterStats>();
    }
	
	// Update is called once per frame
	void Update () {
        // TODO: Animate the strafe when being shot at.
        m_charMove.AnimateChar(m_forward, 0);

        m_navMesh.transform.position = transform.position;

        LookForTarget();

		switch(m_AIState) {
            case AIState.Patrol:
                Patrol();
                break;
        }
	}

    void LookForTarget() {
        if (m_allCharacters.Length > 0) {
            foreach(CharacterStats c in m_allCharacters) {
                if (c != m_characterStats && c.m_enemyFaction != m_characterStats.m_enemyFaction && c == ClosestEnemy()) {
                    RaycastHit hit;
                    Vector3 start = transform.position + (transform.up * m_enemySights.eyeHeight);
                    Vector3 dir = (c.transform.position + c.transform.up) - start;

                    Debug.DrawRay(start, dir, Color.blue);
                    Debug.Log(c.name);

                    float distToCharacter = Vector3.Distance(transform.position, c.transform.position);
                    float sightAngle = Vector3.Angle(dir, transform.forward);
                    Debug.Log(sightAngle);

                    if (Physics.Raycast(start, dir, out hit, m_enemySights.sightRange, m_enemySights.sightLayers)
                        && sightAngle < (m_enemySights.fieldOfView * 2)
                        && hit.collider.GetComponent<CharacterStats>()) {
                        Debug.Log("I see the player.");
                        m_target = hit.transform;
                        m_targetLastKnownPosition = Vector3.zero; // resets target's last known position because target was found and we know where it is.
                    } else {
                        if (m_target != null) {
                            m_targetLastKnownPosition = m_target.position;
                            m_target = null;
                        }
                    }
                }
            }
        }
    }

    CharacterStats ClosestEnemy() {
        CharacterStats closestCharacter = null;
        float minDistance = Mathf.Infinity;
        Vector3 currentPos = transform.position;

        foreach (CharacterStats c in m_allCharacters) {
            if (c != m_characterStats && c.m_enemyFaction != m_characterStats.m_enemyFaction) {
                float distToCharacter = Vector3.Distance(c.transform.position, currentPos);

                if (distToCharacter < minDistance) {
                    closestCharacter = c;
                    minDistance = distToCharacter;
                }

                Debug.Log(closestCharacter.name);
            }
        }

        return closestCharacter;
    }

    void Patrol() {
        if (!m_navMesh.isOnNavMesh) {
            Debug.Log("Cannod do 'void Patrol()' because !m_navMesh.isOnNavMesh");
            return;
        }

        if (m_patrolSettings.waypoints.Length == 0) {
            return;
        }

        m_navMesh.SetDestination(m_patrolSettings.waypoints[m_waypointIndex].destination.position);
        LookAtPosition(m_navMesh.steeringTarget);

        if (m_navMesh.remainingDistance <= m_navMesh.stoppingDistance) {
            m_isWalkingToDestination = false;
            m_forward = LerpSpeed(m_forward, 0.0f, 15.0f);
            m_currentWaitTime -= Time.deltaTime;

            if (m_patrolSettings.waypoints[m_waypointIndex].lookAtTarget != null)
                m_currentLookTransform = m_patrolSettings.waypoints[m_waypointIndex].lookAtTarget;

            if (m_currentWaitTime <= 0) {
                m_waypointIndex = (m_waypointIndex + 1) % m_patrolSettings.waypoints.Length;
            }
        }
        else {
            m_isWalkingToDestination = true;
            m_forward = LerpSpeed(m_forward, 0.5f, 15.0f);
            m_currentWaitTime = m_patrolSettings.waypoints[m_waypointIndex].waitTime;
            m_currentLookTransform = null;
        }
    }

    float LerpSpeed(float currentSpeed, float targetSpeed, float time) {
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * time);
        return currentSpeed;
    }

    void LookAtPosition(Vector3 pos) {
        Vector3 dir = pos - transform.position;
        Quaternion lookRot = Quaternion.LookRotation(dir);
        lookRot.x = 0;
        lookRot.z = 0;

        transform.rotation = Quaternion.Lerp(transform.rotation, lookRot, Time.deltaTime * 5);
    }

    private void OnAnimatorIK() {
        if (m_currentLookTransform != null && !m_isWalkingToDestination) {
            m_animator.SetLookAtPosition(m_currentLookTransform.position);
            m_animator.SetLookAtWeight(1.0f, 0.0f, 0.5f, 0.7f);
        } else {
            m_animator.SetLookAtWeight(0);
        }
    }

}

[System.Serializable]
public class WaypointBase {
    public Transform destination;
    public float waitTime;
    public Transform lookAtTarget;
}