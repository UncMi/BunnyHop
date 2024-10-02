using UnityEngine;

public class BuildSystem : MonoBehaviour
{
    public Transform shootingPoint;
    public GameObject[] blockPrefabs;
    public GameObject ghostBlockPrefab;
    public float rayDistance = 15f;
    public Camera playerCamera; // Add a reference to the camera

    private GameObject currentGhostBlock;
    private GameObject currentBlockPrefab;
    private int currentBlockIndex = 0;

    private void Start()
    {
        currentGhostBlock = Instantiate(ghostBlockPrefab);
        currentGhostBlock.SetActive(false);
        currentBlockPrefab = blockPrefabs[currentBlockIndex];
        UpdateGhostBlockAppearance();
    }

    void Update()
    {
        UpdateGhostBlockPosition();

        // Handle changing block type with number keys
        for (int i = 0; i < blockPrefabs.Length; i++)
        {
            if (Input.GetKeyDown((i + 1).ToString()))
            {
                currentBlockIndex = i;
                currentBlockPrefab = blockPrefabs[currentBlockIndex];
                UpdateGhostBlockAppearance();
            }
        }

        // Place the block with left mouse click
        if (Input.GetMouseButtonDown(0))
        {
            PlaceBlockAtGhostPosition();
        }

        // Destroy a block with right mouse click
        if (Input.GetMouseButtonDown(1))
        {
            DestroyBlock();
        }
    }


    void UpdateGhostBlockPosition()
    {
        // Calculate the position exactly 15f away from the player
        Vector3 targetPosition = shootingPoint.position + shootingPoint.forward * rayDistance;

        // Move the ghost block slightly down so that it's positioned lower than the camera
        targetPosition += Vector3.down * 2.0f; // Adjust the downward offset (you can tweak this value)

        // Combine the block's rotation with the camera's rotation
        Quaternion ghostRotation = Quaternion.Euler(currentBlockPrefab.transform.rotation.eulerAngles + playerCamera.transform.rotation.eulerAngles);

        // Tilt the block upwards, as if looking from below (reverse tilt)
        ghostRotation *= Quaternion.Euler(-30, 0, 0); // Tilt the block by -30 degrees around the X-axis

        // Update the ghost block's position, rotation, and scale
        currentGhostBlock.transform.position = targetPosition;
        currentGhostBlock.transform.rotation = ghostRotation; // Apply the combined rotation and tilt
        currentGhostBlock.transform.localScale = currentBlockPrefab.transform.localScale; // Match scale
        currentGhostBlock.SetActive(true);
    }



    void UpdateGhostBlockAppearance()
    {
        MeshRenderer ghostRenderer = currentGhostBlock.GetComponent<MeshRenderer>();
        MeshRenderer blockRenderer = blockPrefabs[currentBlockIndex].GetComponent<MeshRenderer>();

        if (ghostRenderer && blockRenderer)
        {
            ghostRenderer.sharedMaterial = blockRenderer.sharedMaterial;
        }
    }

    void PlaceBlockAtGhostPosition()
    {
        // Use the ghost block's exact position and rotation for placement
        Instantiate(currentBlockPrefab, currentGhostBlock.transform.position, currentGhostBlock.transform.rotation);
    }

    void DestroyBlock()
    {
        // Raycast for detecting blocks to destroy, limited to 15f
        if (Physics.Raycast(shootingPoint.position, shootingPoint.forward, out RaycastHit hitInfo, rayDistance))
        {
            // Ignore the ghost block and only target blocks with the "Block" tag
            if (hitInfo.transform.CompareTag("Block") && hitInfo.transform.gameObject != currentGhostBlock)
            {
                Destroy(hitInfo.transform.gameObject);
            }
        }
    }
}
