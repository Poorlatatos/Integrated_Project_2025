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

        DetectItem();

        // FOV Sprint Effect
        if (cam != null && playerControl != null)
        {
            float targetFOV = playerControl.isSprinting ? sprintFOV : normalFOV;
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * fovTransitionSpeed);
        }
    }

    void DetectItem()
    {
        if (heldItem != null) return; // Already holding something

        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, itemDetectDistance, itemLayer))
        {
            //Debug.Log("Item detected: " + hit.collider.name);

            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                // Pick up the item
                heldItem = hit.collider.gameObject;
                heldItem.GetComponent<Rigidbody>().isKinematic = true;
                heldItem.GetComponent<Collider>().enabled = false;
                heldItem.transform.SetParent(handTransform);
                heldItem.transform.localPosition = Vector3.zero;
                heldItem.transform.localRotation = Quaternion.identity;
                FindObjectOfType<ParanoiaMeter>()?.IncreaseParanoia();
                Debug.Log("Item picked up!");
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