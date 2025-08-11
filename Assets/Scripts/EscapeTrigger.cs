using UnityEngine;
using TMPro;
using System.Collections;
public class EscapeTrigger : MonoBehaviour
{
    public GameObject enemyTextbox;
    public GameObject[] hudObjects; // Assign all HUD GameObjects (e.g. ChecklistPanel, Heart, SprintUI, etc.)
    public PlayerControl playerControl; // Assign your PlayerControl script
    public ChecklistManager checklistManager; // Assign your ChecklistManager in Inspector
    private bool hasTriggered = false; // Add this line
    public AudioSource triggerAudioSource;
    public AudioClip triggerClip;

    [Header("Escape Movement Settings")]
    public float newWalkSpeed = 8f;
    public float newSprintSpeed = 14f;
    public float newJumpForce = 10f;

    [Header("Escape Camera Settings")]
    public Camera playerCamera; // Assign the player's camera here
    public float newFOV = 90f; // Desired FOV after escape
    public float fovLerpDuration = 1f; // Duration of FOV transition

    private bool fovChanging = false;
    private float fovLerpTime = 0f;
    private float startFOV = 0f;
    public TextMeshProUGUI escapeText;

    [Header("Camera Swing Settings")]
    public Transform enemyTransform; // Assign in Inspector
    public float cameraOffsetDistance = 1.5f; // Distance in front of enemy's face
    public float holdDuration = 1f;
    private bool isTeleporting = false;

    [Header("Escape Enemy Spawn")]
    public GameObject escapeEnemyPrefab; // Assign your new enemy prefab in Inspector
    public Transform escapeEnemySpawnPoint;

    void Start()
    {
        if (escapeText != null)
        {
            var c = escapeText.color;
            c.a = 0f;
            escapeText.color = c;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return; // Prevent re-triggering

        if (triggerAudioSource != null && triggerClip != null)
        {
            triggerAudioSource.PlayOneShot(triggerClip);
        }
        
        if (other.CompareTag("Player"))
        {
            if (checklistManager != null && checklistManager.AreAllItemsCollected())
            {
                hasTriggered = true; // Mark as triggered

                // Optionally disable the collider to prevent further triggers
                var col = GetComponent<Collider>();
                if (col != null)
                    col.enabled = false;

                if (escapeText != null)
                {
                    StartCoroutine(FlashEscapeText(3f, 4f));
                }

                foreach (var obj in hudObjects)
                    if (obj != null) obj.SetActive(false);

                if (playerControl != null)
                {
                    playerControl.sprintDuration = 99999f;
                    playerControl.sprintTimer = 99999f;
                    playerControl.moveSpeed = newWalkSpeed; // Sets the new walk speed
                    playerControl.sprintSpeed = newSprintSpeed; // Sets the new sprint speed
                    playerControl.jumpForce = newJumpForce; // Sets the new jump force
                }

                // Start FOV change
                if (playerCamera != null)
                {
                    startFOV = playerCamera.fieldOfView;
                    fovLerpTime = 0f;
                    fovChanging = true;
                }

                if (playerCamera != null && enemyTransform != null && !isTeleporting)
                {
                    StartCoroutine(TeleportCameraInFrontOfEnemy());
                }

                // Do NOT destroy(gameObject) here!
            }
        }
    }

    void Update()
    {
        // Smoothly lerp FOV if triggered
        if (fovChanging && playerCamera != null)
        {
            fovLerpTime += Time.deltaTime;
            float t = Mathf.Clamp01(fovLerpTime / fovLerpDuration);
            playerCamera.fieldOfView = Mathf.Lerp(startFOV, newFOV, t);
            if (t >= 1f)
                fovChanging = false;
        }
    }

    private IEnumerator FlashEscapeText(float duration, float flashSpeed)
    {
        if (escapeText == null) yield break;
        float timer = 0f;
        while (timer < duration)
        {
            // Flash alpha between 0.2 and 1
            float alpha = Mathf.Lerp(0.2f, 1f, Mathf.PingPong(Time.time * flashSpeed, 1f));
            var color = escapeText.color;
            color.a = alpha;
            escapeText.color = color;
            timer += Time.deltaTime;
            yield return null;
        }
        // Hide after flashing
        var c = escapeText.color;
        c.a = 0f;
        escapeText.color = c;
    }

    private IEnumerator TeleportCameraInFrontOfEnemy()
    {
        isTeleporting = true;
        Transform camTransform = playerCamera.transform;

        // Save original local position and rotation
        Vector3 originalLocalPos = camTransform.localPosition;
        Quaternion originalLocalRot = camTransform.localRotation;
        Transform originalParent = camTransform.parent;

        // Detach camera from player (so we can move it freely)
        camTransform.SetParent(null);

        // Calculate position in front of enemy's face
        Vector3 enemyForward = enemyTransform.forward;
        Vector3 targetPos = enemyTransform.position + enemyForward * cameraOffsetDistance;
        Quaternion targetRot = Quaternion.LookRotation(-enemyForward, Vector3.up);

        // Teleport camera
        camTransform.position = targetPos;
        camTransform.rotation = targetRot;

        // Show textbox
        if (enemyTextbox != null)
            enemyTextbox.SetActive(true);

        // Hold for a moment
        yield return new WaitForSeconds(holdDuration);

        // Hide textbox
        if (enemyTextbox != null)
            enemyTextbox.SetActive(false);

        // Reattach camera to player and restore local position/rotation
        camTransform.SetParent(originalParent);
        camTransform.localPosition = originalLocalPos;
        camTransform.localRotation = originalLocalRot;

        isTeleporting = false;

        // --- SPAWN ESCAPE ENEMY ---
        if (escapeEnemyPrefab != null && escapeEnemySpawnPoint != null && playerControl != null)
        {
            Debug.Log("Spawning escape enemy...");
            GameObject newEnemy = Instantiate(escapeEnemyPrefab, escapeEnemySpawnPoint.position, escapeEnemySpawnPoint.rotation);

            // If it's a PoliceChaser, set player and play siren
            var policeChaser = newEnemy.GetComponent<PoliceChaser>();
            if (policeChaser != null)
            {
                policeChaser.player = playerControl.transform;
                policeChaser.PlaySiren();
            }

            // If using EnemyBasics for other logic, keep this as well
            var enemyAI = newEnemy.GetComponent<EnemyBasics>();
            if (enemyAI != null)
            {
                enemyAI.player = playerControl.transform;
                enemyAI.currentState = EnemyBasics.State.Chasing;
            }
        }
    }
}