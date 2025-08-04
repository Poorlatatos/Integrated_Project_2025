using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonCamera : MonoBehaviour
{
    public Transform playerBody;
    public float mouseSensitivity = 100f;

    float xRotation = 0f;
    const float verticalLimit = 80f;

    [Header("Item Detection")]
    public float itemDetectDistance = 3f;
    public LayerMask itemLayer;

    [Header("Item Hold")]
    public Transform handTransform; // Assign a child transform (e.g. "Hand") to the camera in Inspector
    public GameObject heldItem;

    [Header("FOV Sprint Effect")]
    public float normalFOV = 60f;
    public float sprintFOV = 75f;
    public float fovTransitionSpeed = 8f;
    public PlayerControl playerControl; // Assign your PlayerControl in the Inspector

    Camera cam;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        cam = GetComponent<Camera>();
        if (cam == null)
            cam = GetComponentInChildren<Camera>();
        if (cam != null)
            cam.fieldOfView = normalFOV;
    }

    void Update()
    {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue() * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseDelta.y;
        xRotation = Mathf.Clamp(xRotation, -verticalLimit, verticalLimit);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseDelta.x);

        // Item detection and pickup

        DetectItem();

        // FOV Sprint Effect
        if (cam != null && playerControl != null)
        {
            float targetFOV = (playerControl.isSprinting && playerControl.sprintTimer > 0f) ? sprintFOV : normalFOV;
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * fovTransitionSpeed);
        }
    }

    void DetectItem()
    {
        // Prevent pickup if checklist is open
        var checklist = FindFirstObjectByType<ChecklistManager>();
        if (checklist != null && checklist.IsChecklistOpen)
            return;

        if (heldItem != null) return; // Already holding something

        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, itemDetectDistance, itemLayer))
        {
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                GameObject targetItem = hit.collider.gameObject;
                int keyItemLayer = LayerMask.NameToLayer("KeyItem");
                int itemsLayer = LayerMask.NameToLayer("Items");

                // Notify all customers to check if they see the player stealing
                foreach (var customer in FindObjectsOfType<Customer>())
                {
                    customer.TryReportPlayerStealing();
                }

                // If it's a KeyItem, cross off and destroy immediately
                if (targetItem.layer == keyItemLayer)
                {
                    FindFirstObjectByType<ChecklistManager>()?.RegisterAndCrossOff(targetItem.name);
                    Destroy(targetItem);
                    heldItem = null;
                }
                // If it's a regular Item, pick up and hold it
                else if (targetItem.layer == itemsLayer)
                {
                    heldItem = targetItem;
                    heldItem.GetComponent<Rigidbody>().isKinematic = true;
                    heldItem.GetComponent<Collider>().enabled = false;
                    heldItem.transform.SetParent(handTransform);
                    heldItem.transform.localPosition = Vector3.zero;
                    heldItem.transform.localRotation = Quaternion.identity;
                    FindFirstObjectByType<ParanoiaMeter>()?.IncreaseParanoia();
                    Debug.Log("Item picked up!");
                }
                // Otherwise, do nothing (not pick-up-able)
            }
        }
    }

    // Optional: Drop item with another key (e.g. Q)
    void LateUpdate()
    {
        if (heldItem != null && Keyboard.current.qKey.wasPressedThisFrame)
        {
            heldItem.transform.SetParent(null);
            var rb = heldItem.GetComponent<Rigidbody>();
            rb.isKinematic = false;
            heldItem.GetComponent<Collider>().enabled = true;
            heldItem = null;
        }
    }
}