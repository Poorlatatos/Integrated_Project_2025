using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonCamera : MonoBehaviour
{
    public Transform playerBody;
    public float mouseSensitivity = 100f;

    float xRotation = 0f;
    const float verticalLimit = 80f; // Prevents looking exactly straight up/down

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
    }
}