using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace Psychonaut
{
    public class BuildSystem : MonoBehaviour
    {

        public Transform shootingPoint;
        public GameObject blockObject;

        public Color normalColor;
        public Color highlightedColor;

        GameObject lastHighlightedBlock;
        Vector3 blockScale;

        private void Start()
        {
            blockScale = blockObject.transform.localScale;
        }

        /*
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                BuildBlock(blockObject);
            }

            if (Input.GetMouseButtonDown(1))
            {
                DestroyBlock();
            }
        }
        */
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
        void BuildBlock(GameObject block)
        {
            if (Physics.Raycast(shootingPoint.position, shootingPoint.forward, out RaycastHit hitInfo))
            {
                // Get the block scale to determine proper snapping
                Vector3 spawnPosition;

                // If we hit a block, snap the new block to the face of the hit block
                if (hitInfo.transform.CompareTag("Block"))
                {
                    // Calculate the position by snapping the block to the surface of the hit block
                    Vector3 hitBlockPosition = hitInfo.transform.position;
                    Vector3 normal = hitInfo.normal;

                    spawnPosition = new Vector3(
                        Mathf.Round(hitBlockPosition.x + normal.x * blockScale.x),
                        Mathf.Round(hitBlockPosition.y + normal.y * blockScale.y),
                        Mathf.Round(hitBlockPosition.z + normal.z * blockScale.z)
                    );
                }
                else
                {
                    // For other surfaces, use the same method but adjusted for scale
                    spawnPosition = new Vector3(
                        Mathf.Round(hitInfo.point.x / blockScale.x) * blockScale.x,
                        Mathf.Round(hitInfo.point.y / blockScale.y) * blockScale.y,
                        Mathf.Round(hitInfo.point.z / blockScale.z) * blockScale.z
                    );
                }

                // Instantiate the block at the snapped position
                Instantiate(block, spawnPosition, Quaternion.identity);
            }
        }

    }

}
