﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Animator))]
[RequireComponent (typeof(CharacterController))]
public class CharacterMovement : MonoBehaviour {
    private Animator m_animator { get { return GetComponent<Animator>(); } set { m_animator = value; } }
    private CharacterController m_characterController { get { return GetComponent<CharacterController>(); } set { m_characterController = value; } }

    [System.Serializable]
    public class AnimationSettings {
        public string verticalVelocityFloat = "Forward";
        public string horizontalVelocityFloat = "Strafe";
        public string groundedBool = "IsGrounded";
        public string jumpBool = "IsJumping";
    }
    [SerializeField] public AnimationSettings m_animations;

    [System.Serializable]
    public class PhysicsSettings {
        public float gravityModifier = 9.81f;
        public float baseGravity = 50.0f;
        public float resetGravityValue = 1.2f;  // value to use when resetting gravity when jumping off the ground, force of gravity will start by 5 units.

        public LayerMask groundLayers;
        public float airSpeed = 2.5f;
    }
    [SerializeField] public PhysicsSettings m_physics;

    [System.Serializable]
    public class MovementSettings {
        public float jumpSpeed = 6.0f;
        public float jumpTime = 0.25f;
    }
    [SerializeField] public MovementSettings m_movement;

    Vector3 m_airControl;
    float m_forward;
    float m_strafe;

    bool m_isJumping;
    bool m_resetGravity;
    float m_gravity;

    float m_speed = 1.0f;

    //bool m_isGrounded = true;
    bool IsGrounded() {
        RaycastHit hit;
        Vector3 start = transform.position + transform.up;
        Vector3 dir = Vector3.down;
        float radius = m_characterController.radius;

        if (Physics.SphereCast(start, radius, dir, out hit, m_characterController.height/2, m_physics.groundLayers)) {
            return true;
        }

        return false;
    }

    private void Awake() {
        // replaced by { get { return GetComponent<>(); } set { m_variableName = value; } } design pattern
        //m_animator = GetComponent<Animator>();

        SetupAnimator();
    }

    // Use this for initialization
    void Start () {
        // replaced by { get { return GetComponent<>(); } set { m_variableName = value; } } design pattern
        //m_characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update () {
        AirControl(m_forward, m_strafe);

        ApplyGravity();
        //m_isGrounded = m_characterController.isGrounded;
    }

    /// <summary>
    /// This is the code for generic/non-humanoid animations, that don't have a character controller.
    /// </summary>
    //public void Move(float forward, float strafe) {
    //    ApplyGravity();
    //    AnimateChar(forward, strafe);
    //    Vector3 moveVector = new Vector3(strafe, -m_gravity, forward);
    //    moveVector.z *= Time.deltaTime;
    //    moveVector.x *= Time.deltaTime;
    //    moveVector *= m_speed;
    //    m_characterController.Move(moveVector);
    //}

    // sets up the animator with the child game object avatar.
    void SetupAnimator() {
        //Animator[] animators = GetComponentsInChildren<Animator>();

        //if (animators.Length > 0) {
        //    for (int i = 0; i < animators.Length; i++) {
        //        Animator myAnim = animators[i];
        //        Avatar myAvatar = m_animator.avatar;

        //        // if the animator is not the same as the one in the parent object, change the avatar.
        //        if (myAnim != m_animator) {
        //            m_animator.avatar =  myAvatar;
        //            Destroy(myAnim);
        //        }
        //    }
        //}
        Animator wantedAnim = GetComponentsInChildren<Animator>()[1];
        Avatar wantedAvatar = wantedAnim.avatar;

        m_animator.avatar = wantedAvatar;
        Destroy(wantedAnim);
    }

    // animates the character and root motion handles the movement
    public void AnimateChar(float forward, float strafe) {
        this.m_forward = forward;
        this.m_strafe = strafe;

        m_animator.SetFloat(m_animations.verticalVelocityFloat, forward);
        m_animator.SetFloat(m_animations.horizontalVelocityFloat, strafe);

        m_animator.SetBool(m_animations.groundedBool, IsGrounded());
        m_animator.SetBool(m_animations.jumpBool, m_isJumping);
    }

    void AirControl(float forward, float strafe) {
        if (IsGrounded() == false) {
            m_airControl.x = strafe;
            m_airControl.z = forward;

            // converts air control vector from world space to local space so we'll move depending where we're facing.
            m_airControl = transform.TransformDirection(m_airControl);

            m_airControl *= m_physics.airSpeed;

            m_characterController.Move(m_airControl * Time.deltaTime);
        }
    }

    void ApplyGravity() {
        if (!IsGrounded()) {
            if (!m_resetGravity) {
                m_gravity = m_physics.resetGravityValue;
                m_resetGravity = true;
            }

            m_gravity += Time.deltaTime * m_physics.gravityModifier;
        } else {
            m_gravity = m_physics.baseGravity;
            m_resetGravity = false;
        }

        Vector3 gravityVector = new Vector3();

        if (!m_isJumping) {
            gravityVector.y -= m_gravity;
        } else {
            gravityVector.y = m_movement.jumpSpeed;
        }

        m_characterController.Move(gravityVector * Time.deltaTime);
    }

    public void Jump() {
        if (m_isJumping)
            return;

        if (IsGrounded()) {
            m_isJumping = true;
            StartCoroutine(StopJump());
        }
    }

    IEnumerator StopJump() {
        yield return new WaitForSeconds(m_movement.jumpTime);
        m_isJumping = false;
    }

    public float ToggleWalkRun() {
        return m_speed = m_speed == 1.0f ? 0.5f : 1.0f;
    }

}