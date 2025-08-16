using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraPanSequence : MonoBehaviour
{
    /*
    * Author: Jaasper Lee Zong Hng
    * Date: 28/07/2025
    * Description: Camera pan sequence script for Unity
      Controls the panning and teleporting of the camera during specific sequences.
    */
    public Camera playerCamera; /// Assign in Inspector
    public Transform teleportPosition; /// Where to instantly move the camera
    public Transform panTargetPosition; /// Where to slowly pan the camera to
    public float panDuration = 2f;
    public Transform playerCameraParent; /// The original parent (e.g. player's head/camera holder)
    public Vector3 playerCameraLocalPos; /// The original local position
    public Quaternion playerCameraLocalRot; /// The original local rotation

    private List<MonoBehaviour> scriptsToRestore = new List<MonoBehaviour>();
    private bool isRunning = false;

    void Start()
    {
        TriggerSequence();
    }

    public void TriggerSequence()
    {
        if (!isRunning)
            StartCoroutine(PanRoutine());
    }

    IEnumerator PanRoutine() /// Coroutine for panning the camera
    {
        isRunning = true;

        // --- Disable all active scripts except this one ---
        MonoBehaviour[] allScripts = FindObjectsOfType<MonoBehaviour>();
        scriptsToRestore.Clear();
        foreach (var script in allScripts)
        {
            if (script != this && script.enabled)
            {
                script.enabled = false;
                scriptsToRestore.Add(script);
            }
        }

        // --- Save camera's original parent, position, rotation ---
        playerCameraLocalPos = playerCamera.transform.localPosition;
        playerCameraLocalRot = playerCamera.transform.localRotation;
        playerCameraParent = playerCamera.transform.parent;

        // --- Detach and teleport camera ---
        playerCamera.transform.SetParent(null);
        if (teleportPosition != null)
        {
            playerCamera.transform.position = teleportPosition.position;
            playerCamera.transform.rotation = teleportPosition.rotation;
        }

        // --- Pan camera to target position ---
        if (panTargetPosition != null)
        {
            Vector3 startPos = playerCamera.transform.position;
            Quaternion startRot = playerCamera.transform.rotation;
            Vector3 endPos = panTargetPosition.position;
            Quaternion endRot = panTargetPosition.rotation;

            float t = 0f;
            while (t < panDuration)
            {
                t += Time.deltaTime;
                float lerp = Mathf.Clamp01(t / panDuration);
                playerCamera.transform.position = Vector3.Lerp(startPos, endPos, lerp);
                // Make the camera look at a target point (e.g., playerCameraParent.position)
                if (playerCameraParent != null)
                    playerCamera.transform.LookAt(playerCameraParent.position);
                yield return null;
            }
            playerCamera.transform.position = endPos;
            playerCamera.transform.rotation = endRot;
        }

        // --- Wait for a moment at the target ---
        yield return new WaitForSeconds(1f);

        // --- Restore camera to player ---
        playerCamera.transform.SetParent(playerCameraParent);
        playerCamera.transform.localPosition = playerCameraLocalPos;
        playerCamera.transform.localRotation = playerCameraLocalRot;

        // --- Re-enable all previously active scripts ---
        foreach (var script in scriptsToRestore)
        {
            if (script != null)
                script.enabled = true;
        }

        isRunning = false;
    }
}