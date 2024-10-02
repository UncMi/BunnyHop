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

        private Camera mainCamera;


        [Header("Noclip Settings")]
        [SerializeField] float noclipMoveSpeed = 100f;
        [SerializeField] string noclipButton = "Noclip";

        [SerializeField]
        private string yAxisInput = "Vertical";

        [SerializeField]
        private string xAxisInput = "Horizontal";

        private bool isNoclipActive = false;
        private Vector3 inputDir;
        private Vector3 playerVelocity;

        void Awake()
        {
            mainCamera = Camera.main;
            rb.freezeRotation = true;
        }

        void Update()
        {
            HandleNoclipMovement();
        }


        private void HandleNoclipMovement()
        {
            float moveX = Input.GetAxis(xAxisInput);
            float moveZ = Input.GetAxis(yAxisInput);
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

            Vector3 moveDirection = new Vector3(moveX, moveY, moveZ).normalized;
            Vector3 moveVelocity = moveDirection * noclipMoveSpeed * Time.deltaTime;

            transform.position += mainCamera.transform.TransformDirection(moveVelocity);
        }
    }
}
