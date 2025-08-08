using UnityEngine;
using TMPro;
using System.Collections;
public class EscapeTrigger : MonoBehaviour
{
    public GameObject[] hudObjects; // Assign all HUD GameObjects (e.g. ChecklistPanel, Heart, SprintUI, etc.)
    public PlayerControl playerControl; // Assign your PlayerControl script
    public ChecklistManager checklistManager; // Assign your ChecklistManager in Inspector

    [Header("Escape Movement Settings")]
    public float newWalkSpeed = 8f;
    public float newSprintSpeed = 14f;

    [Header("Escape Camera Settings")]
    public Camera playerCamera; // Assign the player's camera here
    public float newFOV = 90f; // Desired FOV after escape
    public float fovLerpDuration = 1f; // Duration of FOV transition

    private bool fovChanging = false;
    private float fovLerpTime = 0f;
    private float startFOV = 0f;
    public TextMeshProUGUI escapeText;

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
        if (other.CompareTag("Player"))
        {
            if (checklistManager != null && checklistManager.AreAllItemsCollected())
            {
                if (escapeText != null)
                {
                    StartCoroutine(FlashEscapeText(3f, 4f)); // 3 seconds, flashes 4 times per second
                }

                foreach (var obj in hudObjects)
                    if (obj != null) obj.SetActive(false);

                if (playerControl != null)
                {
                    playerControl.sprintDuration = 99999f;
                    playerControl.sprintTimer = 99999f;
                    playerControl.moveSpeed = newWalkSpeed;
                    playerControl.sprintSpeed = newSprintSpeed;
                }

                // Start FOV change
                if (playerCamera != null)
                {
                    startFOV = playerCamera.fieldOfView;
                    fovLerpTime = 0f;
                    fovChanging = true;
                }

                Destroy(gameObject);
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
}