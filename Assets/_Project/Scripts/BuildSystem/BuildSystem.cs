using UnityEngine;

public class BuildSystem : MonoBehaviour
{
    public Transform shootingPoint;
    public GameObject[] blockPrefabs;
    public GameObject ghostBlockPrefab;
    public float rayDistance = 30f;
    public Camera playerCamera; 

    private GameObject currentGhostBlock;
    private GameObject currentBlockPrefab;
    private int currentBlockIndex = 0;
    private bool isXRotationLocked = false; 
    private bool isRotationMode = false;
    private bool isDistanceMode = false;


    private float rotationSensitivity = 10f;
    private float distanceSensitivity = 4f;

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
        for (int i = 0; i < blockPrefabs.Length; i++)
        {
            if (Input.GetKeyDown((i + 1).ToString()))
            {
                currentBlockIndex = i;
                currentBlockPrefab = blockPrefabs[currentBlockIndex];
                UpdateGhostBlockAppearance();
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            PlaceBlockAtGhostPosition();
        }

        if (Input.GetMouseButtonDown(1))
        {
            DestroyBlock();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            isXRotationLocked = !isXRotationLocked;
            if (isXRotationLocked)
            {
                AddedRotation.x = 0f;           
            }
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            isDistanceMode = false;
            isRotationMode = !isRotationMode;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            isRotationMode = false;
            isDistanceMode = !isDistanceMode;
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            AddedRotation = new Vector3(0f, 0f, 0f);
        }
        ChangeGhostRotation();
        ChangeGhostDistance();
    }

    void UpdateGhostBlockPosition()
    {
        Vector3 targetDirection = (shootingPoint.position + shootingPoint.forward * (rayDistance + AddedDistance)) - shootingPoint.position;
        Vector3 targetPosition = shootingPoint.position + targetDirection.normalized * (rayDistance + AddedDistance);

        targetPosition += Vector3.down * 5.0f; 

        Quaternion ghostRotation = Quaternion.LookRotation(-playerCamera.transform.forward, Vector3.up);

        if (isXRotationLocked)
        {
            ghostRotation.eulerAngles = new Vector3(0, ghostRotation.eulerAngles.y, ghostRotation.eulerAngles.z);
        }

        Quaternion addedRotationQuaternion = Quaternion.Euler(AddedRotation);
        ghostRotation *= addedRotationQuaternion;

        currentGhostBlock.transform.position = targetPosition;
        currentGhostBlock.transform.rotation = ghostRotation; // Apply the new rotation
        currentGhostBlock.transform.localScale = currentBlockPrefab.transform.localScale; // Match scale
        currentGhostBlock.SetActive(true);
    }

    Vector3 AddedRotation = new Vector3 (0f, 0f, 0f);
    float AddedDistance = 0f;

    void ChangeGhostRotation()
    {
        if (isRotationMode)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");


            AddedRotation += new Vector3(mouseY * rotationSensitivity, mouseX * rotationSensitivity, 0f);
        }

        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = false;
        }
    }

    void ChangeGhostDistance()
    {

        if (isDistanceMode)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            float mouseY = Input.GetAxis("Mouse Y");

            AddedDistance += mouseY * distanceSensitivity;
        }

        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = false;
        }
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
        Instantiate(currentBlockPrefab, currentGhostBlock.transform.position, currentGhostBlock.transform.rotation);
    }

    void DestroyBlock()
    {
        if (Physics.Raycast(shootingPoint.position, shootingPoint.forward, out RaycastHit hitInfo, rayDistance))
        {
            if (hitInfo.transform.CompareTag("Block") && hitInfo.transform.gameObject != currentGhostBlock)
            {
                Destroy(hitInfo.transform.gameObject);
            }
        }
    }
}
