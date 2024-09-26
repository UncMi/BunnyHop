using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Psychonaut
{
    public class BuildSystem : MonoBehaviour
    {
        public Transform shootingPoint;  // The point from where the player "shoots" or looks
        public GameObject blockObject;   // The block to place

        public Color highlightedColor;   // Color of the preview block
        private GameObject previewBlock; // The preview block instance
        private Vector3 blockScale;      // Scale of the block to snap to the grid

        private float fixedDistanceFromPlayer = 15f;  // Distance at which the block is placed

        private void Start()
        {
            blockScale = blockObject.transform.localScale;

            // Create a preview block and disable it at the start
            previewBlock = Instantiate(blockObject);
            previewBlock.GetComponent<Renderer>().material.color = highlightedColor;  // Set preview color
            previewBlock.SetActive(false);  // Hide the preview block initially
        }

        private void Update()
        {
            UpdatePreviewBlock();  // Update the preview block position every frame

            if (Input.GetMouseButtonDown(0))
            {
                BuildBlock();  // Build the block when left mouse button is clicked
            }

            if (Input.GetMouseButtonDown(1))
            {
                DestroyBlock();
            }
        }


        void DestroyBlock()
        {
            if (Physics.Raycast(shootingPoint.position, shootingPoint.forward, out RaycastHit hitInfo))
            {
                if (hitInfo.transform.tag == "Block")
                {
                    Destroy(hitInfo.transform.gameObject);

                }
            }

        }

        // Update the position of the preview block based on the fixed distance
        void UpdatePreviewBlock()
        {
            // Calculate the spawn position based on the fixed distance
            Vector3 spawnPosition = shootingPoint.position + shootingPoint.forward * fixedDistanceFromPlayer;

            // Snap the position to the grid (optional, depending on how you want placement to work)
            spawnPosition = new Vector3(
                Mathf.Round(spawnPosition.x / blockScale.x) * blockScale.x,
                Mathf.Round(spawnPosition.y / blockScale.y) * blockScale.y,
                Mathf.Round(spawnPosition.z / blockScale.z) * blockScale.z
            );

            // Update the position of the preview block
            previewBlock.transform.position = spawnPosition;
            previewBlock.SetActive(true);  // Show the preview block
        }

        // Build the block at the position of the preview block
        void BuildBlock()
        {
            if (previewBlock.activeSelf)
            {
                Instantiate(blockObject, previewBlock.transform.position, Quaternion.identity);
            }
        }
    }
}
