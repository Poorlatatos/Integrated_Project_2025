using UnityEngine;

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

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (checklistManager != null && checklistManager.AreAllItemsCollected())
            {
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
}