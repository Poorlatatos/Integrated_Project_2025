using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 5f;

    Vector2 moveInput;
    Rigidbody rb;
    public Transform cameraTransform;

    [Header("Crouch Settings")]
    public float crouchHeight = 0.6f;
    public float standingHeight = 1.8f;
    public float crouchSpeed = 2f;
    CapsuleCollider capsuleCollider;
    bool isCrouching = false;

    [Header("Sprint Settings")]
    public float sprintSpeed = 8f;
    public float sprintDuration = 3f; // seconds of sprint available
    public float sprintRechargeRate = 1f; // seconds to recharge 1 second of sprint
    public float sprintTimer;
    public bool isSprinting = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        sprintTimer = sprintDuration;
    }

    void FixedUpdate()
    {
        float currentSpeed = moveSpeed;
        if (isCrouching)
            currentSpeed = crouchSpeed;
        else if (isSprinting && sprintTimer > 0f)
            currentSpeed = sprintSpeed;

        Vector3 camForward = Vector3.Scale(cameraTransform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 camRight = Vector3.Scale(cameraTransform.right, new Vector3(1, 0, 1)).normalized;

        Vector3 move = (camForward * moveInput.y + camRight * moveInput.x) * currentSpeed;
        Vector3 velocity = rb.linearVelocity;
        velocity.x = move.x;
        velocity.z = move.z;
        rb.linearVelocity = velocity;
    }

    void Update()
    {
        // Sprint timer logic
        if (isSprinting && !isCrouching && moveInput.magnitude > 0f)
        {
            sprintTimer -= Time.deltaTime;
            if (sprintTimer < 0f)
                sprintTimer = 0f;
        }
        else
        {
            if (sprintTimer < sprintDuration)
                sprintTimer += Time.deltaTime / sprintRechargeRate;
            if (sprintTimer > sprintDuration)
                sprintTimer = sprintDuration;
        }
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed && Mathf.Abs(rb.linearVelocity.y) < 0.01f)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    public void OnCrouch(InputValue value)
    {
        if (value.isPressed && !isCrouching)
        {
            capsuleCollider.height = crouchHeight;
            Vector3 pos = cameraTransform.localPosition;
            pos.y = crouchHeight / 2f;
            cameraTransform.localPosition = pos;
            isCrouching = true;
        }
        else if (!value.isPressed && isCrouching)
        {
            capsuleCollider.height = standingHeight;
            Vector3 pos = cameraTransform.localPosition;
            pos.y = standingHeight / 2f;
            cameraTransform.localPosition = pos;
            isCrouching = false;
        }
    }

    public void OnSprint(InputValue value)
    {
        isSprinting = value.isPressed && sprintTimer > 0f;
    }
}