using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Psychonaut
{
    public class BuildSystem : MonoBehaviour
    {
        public Transform shootingPoint;
        public GameObject[] blockPrefabs;    // Array for different block types
        public GameObject ghostBlockPrefab;   // A transparent block for preview
        public float rayDistance = 15f;       // Fixed distance of 15f

        private GameObject currentGhostBlock;
        private GameObject currentBlockPrefab;
        private int currentBlockIndex = 0;

        private void Start()
        {
            // Instantiate the ghost block but make it invisible initially
            currentGhostBlock = Instantiate(ghostBlockPrefab);
            currentGhostBlock.SetActive(false); // Initially hidden

            // Set the initial block prefab
            currentBlockPrefab = blockPrefabs[currentBlockIndex];
            UpdateGhostBlockAppearance(); // Initialize ghost block appearance
        }

        private void Update()
        {
            // Update the ghost block's position, rotation, and scale
            UpdateGhostBlockPosition();

            // Change block type when number keys are pressed
            for (int i = 0; i < blockPrefabs.Length; i++)
            {
                if (Input.GetKeyDown((i + 1).ToString()))
                {
                    currentBlockIndex = i;
                    currentBlockPrefab = blockPrefabs[currentBlockIndex];
                    UpdateGhostBlockAppearance();
                }
            }

            // Place the block when the left mouse button is clicked
            if (Input.GetMouseButtonDown(0))
            {
                PlaceBlockAtGhostPosition();
            }
        }

        void UpdateGhostBlockPosition()
        {
            // Calculate the position exactly 15f away from the player (no snapping)
            Vector3 targetPosition = shootingPoint.position + shootingPoint.forward * rayDistance;

            // Update the ghost block position
            currentGhostBlock.transform.position = targetPosition;

            // Rotate the ghost block to match the camera's rotation
            currentGhostBlock.transform.rotation = mainCamera.transform.rotation; // Match camera rotation

            // Keep the scale the same
            currentGhostBlock.transform.localScale = currentBlockPrefab.transform.localScale; // Match scale
            currentGhostBlock.SetActive(true);
        }

        void UpdateGhostBlockAppearance()
        {
            // Change ghost block's appearance to match the current block type
            MeshRenderer ghostRenderer = currentGhostBlock.GetComponent<MeshRenderer>();
            MeshRenderer blockRenderer = blockPrefabs[currentBlockIndex].GetComponent<MeshRenderer>();

            if (ghostRenderer && blockRenderer)
            {
                ghostRenderer.sharedMaterial = blockRenderer.sharedMaterial;
            }
        }

        void PlaceBlockAtGhostPosition()
        {
            // Place the actual block at the position of the ghost block with its rotation and scale
            Instantiate(currentBlockPrefab, currentGhostBlock.transform.position, currentGhostBlock.transform.rotation); // Use ghost block's rotation
        }

        void DestroyBlock()
        {
            // Raycast for detecting blocks to destroy, limited to 15f
            if (Physics.Raycast(shootingPoint.position, shootingPoint.forward, out RaycastHit hitInfo, rayDistance))
            {
                if (hitInfo.transform.CompareTag("Block"))
                {
                    Destroy(hitInfo.transform.gameObject);
                }
            }
        }
    }
}
