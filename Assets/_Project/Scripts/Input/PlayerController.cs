using System.Collections.Generic;
using UnityEngine;
using KBCore.Refs;
using Utilities;
using UnityEditor.Rendering;
using System.Diagnostics.CodeAnalysis;
using System;
using System.Collections;
using TMPro;

interface IInteractive
{
    public void Interact();
}

namespace Psychonaut
{
    public class PlayerController : ValidatedMonoBehaviour
    {
        [Header("References")]
        [SerializeField, Self] Rigidbody rb;
        [SerializeField, Self] GroundChecker groundChecker;
        [SerializeField, Self] Animator animator;
        [SerializeField, Self] Interactor interactor;
        [SerializeField, Self] BuildSystem builder;

        private Camera mainCamera;

        [Header("Movement Settings")]
        //[SerializeField] float moveSpeed = 450f;
        [SerializeField] float rotationSpeed = 50f;
        [SerializeField] float smoothTime = 0.2f;


        [Header("Jump Settings")]
        //[SerializeField] float jumpForce = 10f;
        //[SerializeField] float jumpDuration = 0.5f;
        //[SerializeField] float jumpCooldown = 0f;
        //[SerializeField] float gravityMultiplier = 8f;

        [Header("Interactor Settings")]
        [SerializeField] float interactDuration = 3f;
        [SerializeField] float interactRange = 5f;
        [SerializeField] LayerMask interactableMask;

        [Header("Camera Y-Axis Control Settings")]
        [SerializeField] Transform HeadAimTargetObject;
        [SerializeField] Transform BodyAimTargetObject;
        [SerializeField] Transform HandAimTargetObject;
        [SerializeField] float HeadAim_minYPosition = -5f;
        [SerializeField] float BodyAim_minYPosition = -5f;
        [SerializeField] float HeadAim_maxYPosition = 5f;
        [SerializeField] float BodyAim_maxYPosition = 5f;
        [SerializeField] float cameraPitchSensitivity = 0.1f;

        [Header("Interaction References")]
        [SerializeField] Transform HoldPosition;

        [Header("Noclip Settings")]
        [SerializeField] float noclipMoveSpeed = 100f;
        
        [Header("Menu Settings")]
        [SerializeField] GameObject PauseMenu;

        private BaseItem interactItem;
        private Rigidbody interactItemRB;
        private BoxCollider interactItemCollider;
        private IItemProperty interactItemProperty;
        private IInteractable interactItemInteractable;
        
        

        


        private List<Collider> RagdollParts = new List<Collider>();


        [SerializeField] Transform InteractorSource;

        Transform mainCam;


        float currentSpeed;
        float velocity;
        float jumpVelocity;
        bool isInteracting;
        private bool isJumpHeld;
        private bool ableToJump = true;



        Vector3 movement;
        Vector3 playerVelocity;
        [SerializeField] private bool clampGroundSpeed;


        private Vector3 inputDir;

        private Vector3 _inputRot;
        public Vector3 InputRot => _inputRot;

        private float fallSpeed = 0;
        private float decayFactor = 0.3f;
        private float fallSpeedBonus;
        private float maxFallSpeedBonus = 20;

        private Vector3 groundNormal;
        [SerializeField] private float mouseSensitivity = 1f;
        [SerializeField] private float groundAcceleration = 100f;
        [SerializeField] private float airAcceleration = 100f;
        [SerializeField] private float groundLimit = 12f;
        [SerializeField] private float airLimit = 2f;
        [SerializeField] private float gravity = 4f;
        [SerializeField] private float jumpHeight = 6f;
        [SerializeField] private float slopeLimit = 20f;
        [SerializeField] private float friction = 6f;
        [SerializeField] private float trimpLimit = 5f;
        [SerializeField] private float groundDistance = 0.4f;
        [SerializeField] private float clipDistance = 0.5f;
        [SerializeField] private float fallSpeedConversionRate = 0.2f;

        [SerializeField]
        private string yAxisInput = "Vertical";

        [SerializeField]
        private string xAxisInput = "Horizontal";

        [SerializeField]
        private string jumpButton = "Jump";

        [SerializeField]
        private string restartButton = "Restart";



        private bool onGround;
        private bool isNoclipActive = false;

        [SerializeField] private TMP_Text Text_VerticalSpeed;


        List<Timer> timers;
        CountdownTimer jumpCooldownTimer;


        StateMachine stateMachine;

        StateMachine LowerBodyStateMachine;


        static readonly int Speed = Animator.StringToHash("Speed");
        static readonly int JumpSpeed = Animator.StringToHash("JumpSpeed");


        void SetupStateMachine()
        {

            LowerBodyStateMachine = new StateMachine();


            var locomotionState = new LocomotionState(this, animator);
            var jumpState = new JumpState(this, animator);


            LowerAt(locomotionState, jumpState, new FuncPredicate(() => !onGround));
            LowerAny(locomotionState, new FuncPredicate(ReturnToLocomotionState));


            LowerBodyStateMachine.SetState(locomotionState);

        }

        void LowerAt(IState from, IState to, IPredicate condition) => LowerBodyStateMachine.AddTransition(from, to, condition);
        void LowerAny(IState to, IPredicate condition) => LowerBodyStateMachine.AddAnyTransition(to, condition);

        bool ReturnToLocomotionState()
        {
            return onGround
                   && !isJumpHeld;
        }


        void Awake()
        {
            mainCam = Camera.main.transform;

            rb.freezeRotation = true;
            SetupStateMachine();
            //SetRagdollParts();
            mainCamera = Camera.main;

        }



        void Update()
        {

            HandlePauseMenu();
            //movement = new Vector3(input.Direction.x, 0f, input.Direction.y);

            HandleMovement();
            UpdateAnimator();
            

            HandleRotationFixedPart();
            CheckPosition();
            LowerBodyStateMachine.Update();
            float verticalSpeed = rb.velocity.y;


        }

        void FixedUpdate()
        {
            //HandleCameraYMovement();

            playerVelocity = rb.velocity;


            if (onGround && playerVelocity.magnitude > groundLimit)
            {
                playerVelocity = playerVelocity.normalized * groundLimit;
            }
            if (isJumpHeld && groundChecker.GetGroundDistance() <= groundDistance)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, Vector3.down, out hit, groundDistance + clipDistance, ~LayerMask.GetMask("Player")))
                {
                    groundNormal = hit.normal;
                    if (Vector3.Angle(hit.normal, Vector3.up) > slopeLimit)
                    {
                        onGround = false;
                        ableToJump = false;
                    }
                    else if (playerVelocity.y <= 0)
                    {
                        onGround = true;
                        ableToJump = true;
                    }
                }
                HandleJump();
            }

            if (trimpLimit >= 0f && playerVelocity.y > trimpLimit)
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
                AirAccelerate();
                ApplyImmediateGravity();
            }

            if (playerVelocity.y < -20f)
                fallSpeed = playerVelocity.y;
            else if (playerVelocity.y > 0f)
                fallSpeed = 0f;

            rb.velocity = playerVelocity;
            animator.SetFloat("Speed", playerVelocity.x);
            onGround = false;
            groundNormal = Vector3.zero;
        }
        private void LateUpdate()
        {
            HandleRotation();

        }

        private void OnCollisionStay(Collision other)
        {
            ContactPoint[] contacts = other.contacts;
            for (int i = 0; i < contacts.Length; i++)
            {
                ContactPoint contactPoint = contacts[i];

                // Check if the surface is walkable (i.e., ground, not a wall)
                if (contactPoint.normal.y > Mathf.Cos(slopeLimit * Mathf.Deg2Rad)) // Slope limit comparison
                {
                    groundNormal = contactPoint.normal;
                    onGround = true;
                    ableToJump = true;

                    if (isJumpHeld)
                    {
                        HandleJump();
                    }

                    break;
                }
                else
                {
                    onGround = false;
                    ableToJump = false;
                    Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
                    rb.velocity = new Vector3(horizontalVelocity.x, rb.velocity.y, horizontalVelocity.z);
                }
            }

        }

        private void OnCollisionEnter(Collision collision)
        {

            if (IsClimbableObject(collision))
            {
                ClipToTopOfObject(collision);
            }
        }

        // Check if the object height is less than or equal to 3f
        private bool IsClimbableObject(Collision collision)
        {
            Vector3 objectContactPoint = collision.contacts[0].point;
            Collider objectCollider = collision.collider;
            float objectTopY = objectCollider.bounds.max.y;

            float distanceFromTop = objectTopY - objectContactPoint.y;
            return distanceFromTop <= 0.5f;
        }
        // Clip the player on top of the object without losing horizontal momentum
        private void ClipToTopOfObject(Collision collision)
        {
            Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            float horizontalSpeed = horizontalVelocity.magnitude;

            Collider playerCollider = GetComponent<Collider>();
            Vector3 playerContactPoint = collision.contacts[0].point;
            Collider objectCollider = collision.collider;
            float objectTopY = objectCollider.bounds.max.y;

            float distanceFromTop = objectTopY - playerContactPoint.y;

            // Check if the player is near the edge of the object
            bool isNearEdge = Mathf.Abs(playerContactPoint.x - objectCollider.bounds.max.x) < 0.5f ||
                              Mathf.Abs(playerContactPoint.z - objectCollider.bounds.max.z) < 0.5f;

            if (playerContactPoint.y <= playerCollider.bounds.min.y + 0.5f && isNearEdge)
            {
                if (horizontalSpeed > 5f)
                {
                    transform.position = new Vector3(transform.position.x, objectTopY + 0.4f, transform.position.z);
                    playerVelocity.y += jumpHeight;
                    onGround = false;

                    StartCoroutine(JumpTimer());

                    rb.velocity = new Vector3(horizontalVelocity.x, rb.velocity.y, horizontalVelocity.z);
                }
            }
        }

        private bool isPaused = false;
        void HandlePauseMenu()
        {
            if (Input.GetButtonDown("Escape"))
            {
                isPaused = !isPaused; // Toggle the pause state

                if (isPaused)
                {
                    // Open the pause menu and pause the game
                    PauseMenu.SetActive(true);
                    Time.timeScale = 0f; // Pause the game
                    Cursor.lockState = CursorLockMode.None; // Unlock the cursor
                    Cursor.visible = true; // Make the cursor visible
                }
                else
                {
                    // Close the pause menu and resume the game
                    PauseMenu.SetActive(false);
                    Time.timeScale = 1f; // Resume the game
                    Cursor.lockState = CursorLockMode.Locked; // Lock the cursor again
                    Cursor.visible = false; // Hide the cursor
                }
            }
        }



        void ApplyImmediateGravity()
        {
            if (!onGround || rb.velocity.y < 0)
            {
                playerVelocity.y -= gravity * Time.deltaTime;
            }
        }

        private void GroundAccelerate()
        {
            float num = groundLimit - Vector3.Dot(playerVelocity, inputDir);
            if (!(num <= 0f))
            {
                float num2 = groundAcceleration * Time.deltaTime;
                if (num2 > num)
                {
                    num2 = num;
                }
                playerVelocity += num2 * inputDir;
                if (clampGroundSpeed && playerVelocity.magnitude > groundLimit)
                {
                    playerVelocity = playerVelocity.normalized * groundLimit;
                }
            }
        }

        private void ApplyFriction()
        {
            playerVelocity *= Mathf.Clamp01(1f - Time.deltaTime * friction);
        }

        private void AirAccelerate()
        {
            Vector3 lhs = playerVelocity;
            lhs.y = 0f;
            float num = airLimit - Vector3.Dot(lhs, inputDir);
            if (!(num <= 0f))
            {
                float num2 = airAcceleration * Time.deltaTime;
                if (num2 > num)
                {
                    num2 = num;
                }
                playerVelocity += num2 * inputDir;
            }
        }


        void UpdateAnimator()
        {
            animator.SetFloat(Speed, currentSpeed);
            animator.SetFloat(JumpSpeed, rb.velocity.y);
        }
        public void HandleMovement()
        {
            float axisRaw = Input.GetAxisRaw(xAxisInput);
            float axisRaw2 = Input.GetAxisRaw(yAxisInput);
            inputDir = base.transform.rotation * new Vector3(axisRaw, 0f, axisRaw2).normalized;

            if (Input.GetButtonDown(jumpButton))
            {
                isJumpHeld = true;

            }
            if (Input.GetButtonUp(jumpButton))
            {
                isJumpHeld = false;
            }

        }



        void HandleJump()
        {
            if (ableToJump)
            {
                if (playerVelocity.y < 0f)
                {
                    playerVelocity.y = 0f; // Preserve horizontal momentum
                }

                fallSpeedBonus = Mathf.Clamp(fallSpeedBonus * decayFactor, 0, maxFallSpeedBonus);
                fallSpeedBonus = Math.Abs(fallSpeed) * fallSpeedConversionRate * decayFactor;
                playerVelocity.y += jumpHeight + fallSpeedBonus;

                onGround = false;

                StartCoroutine(JumpTimer());
            }
        }



        Vector3 cameraForward;

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
                // Calculate the new rotation based on the camera's forward vector
                Quaternion newRotation = Quaternion.LookRotation(cameraForward);

                // Rotate the player smoothly
                rb.MoveRotation(Quaternion.Slerp(rb.rotation, newRotation, rotationSpeed * Time.deltaTime));

                // Get the current velocity
                Vector3 currentVelocity = rb.velocity;

                // Preserve vertical velocity (y-axis) and only adjust horizontal velocity
                float verticalVelocity = currentVelocity.y;  // Store the vertical velocity

                // Calculate new horizontal velocity direction based on the character's forward direction
                Vector3 horizontalVelocity = rb.transform.forward * new Vector3(currentVelocity.x, 0f, currentVelocity.z).magnitude;

                // Apply the horizontal velocity and keep the vertical velocity intact
                rb.velocity = new Vector3(horizontalVelocity.x, verticalVelocity, horizontalVelocity.z);
            }
        }



        private IEnumerator JumpTimer()
        {
            ableToJump = false;
            yield return new WaitForSeconds(0.1f);
            ableToJump = true;
        }

        void CheckPosition()
        {
            if (transform.position.y < -700f || Input.GetButtonDown(restartButton))
            {
                // Reset position to (0, 0, 0)
                ResetPosition();
            }
        }

        void ResetPosition()
        {
            // Reset the player's position to (0, 0, 0)
            transform.position = Vector3.zero;

            // Reset Rigidbody velocity
            if (rb != null)
            {

                rb.velocity = Vector3.zero;
                playerVelocity = Vector3.zero;  // Reset the stored player velocity

                // Reset fall speed and any jump-related state
                fallSpeed = 0f;
                isJumpHeld = false;
                ableToJump = true;
            }
        }


    }
}

