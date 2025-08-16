using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    /*
    * Author: Jaasper Lee Zong Hng
    * Date: 24/07/2025
    * Description: Player control script for Unity
      Controls the behavior of the player character, including movement, jumping, and crouching.
    */

    [Header("Movement Settings")]
    public float moveSpeed = 5f; /// Speed of the player movement
    public float jumpForce = 5f; /// Force applied when the player jumps

    Vector2 moveInput;
    Rigidbody rb;
    public Transform cameraTransform;

    [Header("Crouch Settings")]
    public float crouchHeight = 0.6f; /// Height of the player when crouching
    public float standingHeight = 1.8f; /// Height of the player when standing
    public float crouchSpeed = 2f; /// Speed of the player when crouching
    CapsuleCollider capsuleCollider;
    bool isCrouching = false;

    [Header("Sprint Settings")]
    public float sprintSpeed = 8f; /// Speed of the player when sprinting
    public float sprintDuration = 3f; /// Seconds of sprint available
    public float sprintRechargeRate = 1f; /// Seconds to recharge 1 second of sprint
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
        
        if (isSprinting)
        {
            sprintTimer -= Time.deltaTime;
            if (sprintTimer <= 0f)
            {
                sprintTimer = 0f;
                isSprinting = false; // Force stop sprinting
            }
        }
        else
        {
            // Regenerate stamina if not sprinting (optional)
            if (sprintTimer < sprintDuration)
                sprintTimer += Time.deltaTime;
        }

    }

    public void OnMove(InputValue value) /// Handle player movement input
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnJump(InputValue value) /// Handle player jump input
    {
        if (value.isPressed && Mathf.Abs(rb.linearVelocity.y) < 0.01f)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    public void OnCrouch(InputValue value) /// Handle player crouch input
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

    public void OnSprint(InputValue value) /// Handle player sprint input
    {
        isSprinting = value.isPressed && sprintTimer > 0f;
    }
}