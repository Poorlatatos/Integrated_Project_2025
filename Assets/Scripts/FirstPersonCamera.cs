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

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue() * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseDelta.y;
        xRotation = Mathf.Clamp(xRotation, -verticalLimit, verticalLimit);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseDelta.x);

        DetectItem();
    }

    void DetectItem()
    {
        if (heldItem != null) return; // Already holding something

        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, itemDetectDistance, itemLayer))
        {
            Debug.Log("Item detected: " + hit.collider.name);

            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                // Pick up the item
                heldItem = hit.collider.gameObject;
                heldItem.GetComponent<Rigidbody>().isKinematic = true;
                heldItem.GetComponent<Collider>().enabled = false;
                heldItem.transform.SetParent(handTransform);
                heldItem.transform.localPosition = Vector3.zero;
                heldItem.transform.localRotation = Quaternion.identity;
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