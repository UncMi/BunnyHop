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
    private bool isScaleMode = false;
    private bool isScaleMode_X = true;
    private bool isScaleMode_Y = false;
    private bool isScaleMode_Z = false;


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
            if (!isScaleMode)
            {
                isXRotationLocked = !isXRotationLocked;
                if (isXRotationLocked)
                {
                    AddedRotation.x = 0f;
                }
            }
            else if (isScaleMode)
            {
                isScaleMode_X = true;
                isScaleMode_Y = false;
                isScaleMode_Z = false;
            }
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (isScaleMode)
            {
                isScaleMode_X = false;
                isScaleMode_Y = false;
                isScaleMode_Z = true;
            }
        }
       if (Input.GetKeyDown(KeyCode.C))
        {
            if (isScaleMode)
            {
                isScaleMode_X = false;
                isScaleMode_Y = true;
                isScaleMode_Z = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            isDistanceMode = false;
            isScaleMode = false;
            isRotationMode = !isRotationMode;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            isRotationMode = false;
            isScaleMode = false;
            isDistanceMode = !isDistanceMode;
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            isRotationMode = false;
            isDistanceMode = false;
            isScaleMode = !isScaleMode;
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            AddedRotation = new Vector3(0f, 0f, 0f);
            AddedScale = new Vector3(0f, 0f, 0f);
        }
        ChangeGhostRotation();
        ChangeGhostDistance();
        ChangeGhostScale();
    }

    void UpdateGhostBlockPosition()
    {
        Vector3 targetDirection = (shootingPoint.position + shootingPoint.forward * (rayDistance + AddedDistance)) - shootingPoint.position;
        Vector3 targetPosition = shootingPoint.position + targetDirection.normalized * (rayDistance + AddedDistance);
        Vector3 targetScale = currentBlockPrefab.transform.localScale + AddedScale;

        targetPosition += Vector3.down * 5.0f; 

        Quaternion ghostRotation = Quaternion.LookRotation(-playerCamera.transform.forward, Vector3.up);

        if (isXRotationLocked)
        {
            ghostRotation.eulerAngles = new Vector3(0, ghostRotation.eulerAngles.y, ghostRotation.eulerAngles.z);
        }

        Quaternion addedRotationQuaternion = Quaternion.Euler(AddedRotation);
        ghostRotation *= addedRotationQuaternion;

        currentGhostBlock.transform.position = targetPosition;
        currentGhostBlock.transform.rotation = ghostRotation;
        currentGhostBlock.transform.localScale = targetScale;
        currentGhostBlock.SetActive(true);
    }

    Vector3 AddedRotation = new Vector3 (0f, 0f, 0f);
    float AddedDistance = 0f;

    void ChangeGhostRotation()
    {
        if (isRotationMode)
        {

            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");


            AddedRotation += new Vector3(mouseY * rotationSensitivity, mouseX * rotationSensitivity, 0f);
        }

      
    }

    void ChangeGhostDistance()
    {

        if (isDistanceMode)
        {
            float mouseY = Input.GetAxis("Mouse Y");

            AddedDistance += mouseY * distanceSensitivity;
        }

      
    }

    private Vector3 AddedScale = new Vector3(0, 0, 0);

    void ChangeGhostScale()
    {
        if (isScaleMode)
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            if (isScaleMode_X)
            {
                AddedScale += new Vector3(mouseX * distanceSensitivity, 0, 0);
            }
            if (isScaleMode_Y)
            {
                AddedScale += new Vector3(0, mouseX * distanceSensitivity, 0);
            }
            if (isScaleMode_Z)
            {
                AddedScale += new Vector3(0, 0, mouseX * distanceSensitivity);
            }

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
        GameObject placedBlock = Instantiate(currentBlockPrefab, currentGhostBlock.transform.position, currentGhostBlock.transform.rotation);
        placedBlock.transform.localScale = currentGhostBlock.transform.localScale;
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
