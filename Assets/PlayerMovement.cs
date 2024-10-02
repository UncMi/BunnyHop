using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using KBCore.Refs;
using Utilities;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class PlayerMovement : MonoBehaviour



{

    [Header("References")]
    [SerializeField, Self] Animator animator;
    [SerializeField, Self] Interactor interactor;
    private Camera mainCamera;


    [SerializeField] float rotationSpeed = 50f;

    [SerializeField]
    private string yAxisInput = "Vertical";

    [SerializeField]
    private string xAxisInput = "Horizontal";

    [SerializeField]
    private string inputMouseX = "Mouse X";

    [SerializeField]
    private string inputMouseY = "Mouse Y";

    [SerializeField]
    private string jumpButton = "Jump";

    [SerializeField]
    private float mouseSensitivity = 1f;

    [SerializeField]
    private float groundAcceleration = 100f;

    [SerializeField]
    private float airAcceleration = 100f;

    [SerializeField]
    private float groundLimit = 12f;

    [SerializeField]
    private float airLimit = 1f;

    [SerializeField]
    private float gravity = 16f;

    [SerializeField]
    private float friction = 6f;

    [SerializeField]
    private float jumpHeight = 6f;

    [SerializeField]
    private float trimpLimit = 5f;

    [SerializeField]
    private float slopeLimit = 45f;

    [SerializeField]
    private bool additiveJump = true;

    [SerializeField]
    private bool autoJump = true;

    [SerializeField]
    private bool clampGroundSpeed;

    [SerializeField]
    private bool disableBunnyHopping;

    private Rigidbody rb;

    private CapsuleCollider capsule;

    private Vector3 vel;

    private Vector3 inputDir;

    private Vector3 _inputRot;

    private Vector3 groundNormal;

    private bool onGround;

    private bool jumpPending;

    private bool ableToJump = true;

    public Vector3 InputRot => _inputRot;
    Vector3 cameraForward;


    private void Awake()
    {
        mainCamera = Camera.main;
    }
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        MouseLook();
        GetMovementInput();
        HandleRotationFixedPart();
    }
    private void LateUpdate()
    {
        HandleRotation();
    }


    public void HandleRotation()
    {
        if (mainCamera != null)
        {
            cameraForward = mainCamera.transform.forward;
            cameraForward.y = 0f;
        }
    }
    public void HandleRotationFixedPart()
    {
        if (cameraForward != Vector3.zero)
        {
            Quaternion newRotation = Quaternion.LookRotation(cameraForward);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, newRotation, rotationSpeed * Time.deltaTime));
        }
    }
    private void FixedUpdate()
    {
        vel = rb.velocity;
        if (disableBunnyHopping && onGround && vel.magnitude > groundLimit)
        {
            vel = vel.normalized * groundLimit;
        }
        if (jumpPending && onGround)
        {
            Jump();
        }
        if (trimpLimit >= 0f && vel.y > trimpLimit)
        {
            onGround = false;
        }
        if (onGround)
        {
            inputDir = Vector3.Cross(Vector3.Cross(groundNormal, inputDir), groundNormal);
            GroundAccelerate();
            ApplyFriction();
        }
        else
        {
            ApplyGravity();
            AirAccelerate();
        }
        rb.velocity = vel;
        onGround = false;
        groundNormal = Vector3.zero;
    }

    private void GetMovementInput()
    {
        float axisRaw = Input.GetAxisRaw(xAxisInput);
        float axisRaw2 = Input.GetAxisRaw(yAxisInput);
        inputDir = base.transform.rotation * new Vector3(axisRaw, 0f, axisRaw2).normalized;
        if (Input.GetButtonDown(jumpButton))
        {
            jumpPending = true;
        }
        if (Input.GetButtonUp(jumpButton))
        {
            jumpPending = false;
        }
    }

    private void MouseLook()
    {
        _inputRot.y += Input.GetAxisRaw(inputMouseX) * mouseSensitivity;
        _inputRot.x -= Input.GetAxisRaw(inputMouseY) * mouseSensitivity;
        if (_inputRot.x > 90f)
        {
            _inputRot.x = 90f;
        }
        if (_inputRot.x < -90f)
        {
            _inputRot.x = -90f;
        }
        base.transform.rotation = Quaternion.Euler(0f, _inputRot.y, 0f);
    }

    private void GroundAccelerate()
    {
        float num = groundLimit - Vector3.Dot(vel, inputDir);
        if (!(num <= 0f))
        {
            float num2 = groundAcceleration * Time.deltaTime;
            if (num2 > num)
            {
                num2 = num;
            }
            vel += num2 * inputDir;
            if (clampGroundSpeed && vel.magnitude > groundLimit)
            {
                vel = vel.normalized * groundLimit;
            }
        }
    }

    private void AirAccelerate()
    {
        Vector3 lhs = vel;
        lhs.y = 0f;
        float num = airLimit - Vector3.Dot(lhs, inputDir);
        if (!(num <= 0f))
        {
            float num2 = airAcceleration * Time.deltaTime;
            if (num2 > num)
            {
                num2 = num;
            }
            vel += num2 * inputDir;
        }
    }

    private void ApplyFriction()
    {
        vel *= Mathf.Clamp01(1f - Time.deltaTime * friction);
    }

    private void Jump()
    {
        if (ableToJump)
        {
            if (vel.y < 0f || !additiveJump)
            {
                vel.y = 0f;
            }
            vel.y += jumpHeight;
            onGround = false;
            if (!autoJump)
            {
                jumpPending = false;
            }
            StartCoroutine(JumpTimer());
        }
    }

    private void ApplyGravity()
    {
        vel.y -= gravity * Time.deltaTime;
    }

    private void OnCollisionStay(Collision other)
    {
        ContactPoint[] contacts = other.contacts;
        for (int i = 0; i < contacts.Length; i++)
        {
            ContactPoint contactPoint = contacts[i];
            if (contactPoint.normal.y > Mathf.Sin(slopeLimit * ((float)Math.PI / 180f) + (float)Math.PI / 2f))
            {
                groundNormal = contactPoint.normal;
                onGround = true;
                break;
            }
        }
    }

    private IEnumerator JumpTimer()
    {
        ableToJump = false;
        yield return new WaitForSeconds(0.1f);
        ableToJump = true;
    }
}
