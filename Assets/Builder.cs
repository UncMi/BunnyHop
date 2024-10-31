using System.Collections.Generic;
using UnityEngine;
using KBCore.Refs;
using Utilities;
using System.Collections;
using TMPro;

namespace Psychonaut
{
    public class Builder : ValidatedMonoBehaviour
    {
        [Header("References")]
        [SerializeField, Self] Rigidbody rb;
        [SerializeField, Self] GroundChecker groundChecker;
        [SerializeField, Self] Animator animator;
        [SerializeField, Self] Interactor interactor;
        [SerializeField, Self] BuildSystem buildSystem;
        [SerializeField, Self] PlayerController playerController;

        private Camera mainCamera;

        [Header("Noclip Settings")]
        [SerializeField] float noclipMoveSpeed = 200f;
        [SerializeField] float sprintMoveSpeed = 800f;  // Sprint speed
        [SerializeField] string noclipButton = "Noclip";
        [SerializeField] private string sprintButton = "Sprint"; // Sprint input
        [SerializeField] private string yAxisInput = "Vertical";
        [SerializeField] private string xAxisInput = "Horizontal";

        [Header("Menu Settings")]
        [SerializeField] GameObject PauseMenu;

        private bool isNoclipActive = false;
        private bool isSprinting = false;  // Sprint state
        private Vector3 inputDir;
        private Vector3 playerVelocity;

        void Awake()
        {
            mainCamera = Camera.main;
            rb.freezeRotation = true;
        }

        void Update()
        {
            HandleSprintToggle();  // Check for sprint toggle
            HandleNoclipMovement();
        }

        private void HandleSprintToggle()
        {
            // Check if the Sprint button is pressed
            if (Input.GetButtonDown(sprintButton))
            {
                isSprinting = !isSprinting;  // Toggle sprint mode
                if (isSprinting)
                {
                    noclipMoveSpeed = sprintMoveSpeed;  // Set to sprint speed
                }
                else
                {
                    noclipMoveSpeed = noclipMoveSpeed;  // Reset to normal speed
                }
            }
        }

        private void HandleNoclipMovement()
        {
            float moveX = Input.GetAxisRaw(xAxisInput);  // Get raw input for immediate response
            float moveZ = Input.GetAxisRaw(yAxisInput);  // Get raw input for immediate response
            float moveY = 0f;

            // Move up or down using jump button or crouch key
            if (Input.GetButton("Jump"))
            {
                moveY = 1f;
            }
            else if (Input.GetButton("Crouch"))
            {
                moveY = -1f;
            }

            // Calculate movement direction and velocity
            Vector3 moveDirection = new Vector3(moveX, moveY, moveZ).normalized;
            Vector3 moveVelocity = moveDirection * noclipMoveSpeed;

            // Move the player based on camera direction
            transform.position += mainCamera.transform.TransformDirection(moveVelocity) * Time.deltaTime;
        }

       


    }
}
