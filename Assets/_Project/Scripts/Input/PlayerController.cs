using System.Collections.Generic;
using UnityEngine;
using KBCore.Refs;
using Utilities;
using UnityEditor.Rendering;
using System.Diagnostics.CodeAnalysis;
using System;

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
        [SerializeField, Self] Inventory inventory;
        [SerializeField, Anywhere] InputReader input;
        private Camera mainCamera;

        [Header("Movement Settings")]
        [SerializeField] float moveSpeed = 5f;
        [SerializeField] float rotationSpeed = 50f;
        [SerializeField] float smoothTime = 0.2f;

        [Header("Jump Settings")]
        [SerializeField] float jumpForce = 10f;
        [SerializeField] float jumpDuration = 0.5f;
        [SerializeField] float jumpCooldown = 0f;
        [SerializeField] float gravityMultiplier = 3f;

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

        Vector3 movement;

        List<Timer> timers;
        CountdownTimer jumpTimer;
        CountdownTimer jumpCooldownTimer;


        StateMachine stateMachine;

        StateMachine UpperBodyStateMachine;
        StateMachine LowerBodyStateMachine;

        static readonly int Speed = Animator.StringToHash("Speed");

        void Awake()
        {
            mainCam = Camera.main.transform;

            rb.freezeRotation = true;
            SetupTimers();
            SetupStateMachine();

            mainCamera = Camera.main;

        }
        void SetupStateMachine()
        {
            //stateMachine = new StateMachine();

            UpperBodyStateMachine = new StateMachine();
            LowerBodyStateMachine = new StateMachine();


            var locomotionState = new LocomotionState(this, animator);
            var jumpState = new JumpState(this, animator);

            var interactState = new InteractState(this, animator);
            var noInteractState = new NoInteractState(this, animator);

            // LowerBody transition definitions
            LowerAt(locomotionState, jumpState, new FuncPredicate(() => jumpTimer.IsRunning));
            LowerAny(locomotionState, new FuncPredicate(ReturnToLocomotionState));

            // UpperBody transition definitions
            UpperAt(interactState, noInteractState, new FuncPredicate(() => !isInteracting)); // REMINDER change this when interact system changes
            UpperAt(noInteractState, interactState, new FuncPredicate(() => isInteracting)); // REMINDER change this when interact system changes

            //stateMachine.SetState(locomotionState);

            LowerBodyStateMachine.SetState(locomotionState);
            UpperBodyStateMachine.SetState(noInteractState);

        }


        bool ReturnToLocomotionState()
        {
            return groundChecker.IsGrounded
                   && !jumpTimer.IsRunning;
        }


        //void At(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);
        //void Any(IState to, IPredicate condition) => stateMachine.AddAnyTransition(to, condition);

        void LowerAt(IState from, IState to, IPredicate condition) => LowerBodyStateMachine.AddTransition(from, to, condition);
        void LowerAny(IState to, IPredicate condition) => LowerBodyStateMachine.AddAnyTransition(to, condition);

        void UpperAt(IState from, IState to, IPredicate condition) => UpperBodyStateMachine.AddTransition(from, to, condition);
        void UpperAny(IState to, IPredicate condition) => UpperBodyStateMachine.AddAnyTransition(to, condition);
        void SetupTimers()
        {
            // Setup timers
            jumpTimer = new CountdownTimer(jumpDuration);
            jumpCooldownTimer = new CountdownTimer(jumpCooldown);

            jumpTimer.OnTimerStart += () => jumpVelocity = jumpForce;
            jumpTimer.OnTimerStop += () => jumpCooldownTimer.Start();

            timers = new(2) { jumpTimer, jumpCooldownTimer };

        }

        void OnEnable()
        {
            input.Jump += OnJump;
            input.Interact += OnInteract;
        }

        void OnDisable()
        {
            input.Jump -= OnJump;
            input.Interact -= OnInteract;
        }

        void Start() => input.EnablePlayerActions();

        void Update()
        {
            movement = new Vector3(input.Direction.x, 0f, input.Direction.y);
            HandleRotationFixedPart();
            HandleTimers();

            //stateMachine.Update();
            UpperBodyStateMachine.Update();
            LowerBodyStateMachine.Update();

        }

        void FixedUpdate()
        {
            HandleJump();
            HandleCameraYMovement();
            UpdateAnimator();
            HandleMovement();

        }

        private void LateUpdate()
        {
            HandleRotation();

        }
        void UpdateAnimator()
        {
            animator.SetFloat(Speed, currentSpeed);
        }

        public void HandleMovement()
        {
            //Rotation movement direction to match camera rotation
            var adjustedDirection = Quaternion.AngleAxis(mainCam.eulerAngles.y, Vector3.up) * movement;

            if (adjustedDirection.magnitude > 0f)
            {
                HandleHorizontalMovement(adjustedDirection);
                SmoothSpeed(adjustedDirection.magnitude);
            }
            else
            {
                SmoothSpeed(0f);

                rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
            }
        }


        void HandleHorizontalMovement(Vector3 adjustedDirection)
        {

            Vector3 velocity = adjustedDirection * moveSpeed * Time.deltaTime;
            rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.z);
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
                Quaternion newRotation = Quaternion.LookRotation(cameraForward);
                rb.MoveRotation(Quaternion.Slerp(rb.rotation, newRotation, rotationSpeed * Time.deltaTime));
            }
        }


        void HandleCameraYMovement()
        {
            if (HeadAimTargetObject == null) return;
            if (BodyAimTargetObject == null) return;

            float cameraPitch = mainCam.eulerAngles.x;

            // Convert pitch from 0-360 to -180 to 180 (since pitch resets from 360 to 0 when looking straight down)
            if (cameraPitch > 180f) cameraPitch -= 360f;

            float HeadAim_TargetYPosition = Mathf.Clamp(-cameraPitch * cameraPitchSensitivity, HeadAim_minYPosition, HeadAim_maxYPosition);
            float BodyAim_TargetYPosition = Mathf.Clamp(-cameraPitch * cameraPitchSensitivity, BodyAim_minYPosition, BodyAim_maxYPosition);


            Vector3 Head_CurrentPosition = HeadAimTargetObject.position;
            Head_CurrentPosition.y = HeadAim_TargetYPosition;
            HeadAimTargetObject.position = Head_CurrentPosition;

            Vector3 Body_CurrentPosition = BodyAimTargetObject.position;
            Body_CurrentPosition.y = BodyAim_TargetYPosition;
            BodyAimTargetObject.position = Body_CurrentPosition;

        }


        public void HandleJump()
        {
            if (!jumpTimer.IsRunning && groundChecker.IsGrounded)
            {
                jumpVelocity = 0f;
                return;
            }

            if (!jumpTimer.IsRunning)
            {
                jumpVelocity += Physics.gravity.y * gravityMultiplier * Time.deltaTime;
            }
            Debug.Log("jumptimer running");

            rb.velocity = new Vector3(rb.velocity.x, jumpVelocity, rb.velocity.z);
        }




        void OnInteract(bool performed)
        {
            if (performed)
            {
                isInteracting = !isInteracting;

                if (isInteracting)
                {
                    HandleInteract();
                }
            }
        }
        public void HandleInteract()
        {
            if (isInteracting && interactor.interactable != null)
            {
                interactor.Interact();
            }
        }



        void OnJump(bool performed)
        {
            if (performed && !jumpTimer.IsRunning && !jumpCooldownTimer.IsRunning && groundChecker.IsGrounded)
            {
                jumpTimer.Reset(0.1f);
                jumpTimer.Start();
            }
            else if (!performed && jumpTimer.IsRunning)
            {
                jumpTimer.Stop();
            }
        }

        void HandleTimers()
        {
            foreach (var timer in timers) 
            {
                timer.Tick(Time.deltaTime);
            }
        }



        void SmoothSpeed(float value)
        {
            currentSpeed = Mathf.SmoothDamp(currentSpeed, value, ref velocity, smoothTime);
        }

    }
}
