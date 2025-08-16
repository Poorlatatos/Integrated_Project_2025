using UnityEngine;
using System.Collections;
using System.Collections.Generic; // Add this for List<>
using UnityEngine.SceneManagement;

public class EndSequenceTrigger : MonoBehaviour
{
    /*
    * Author: Jaasper Lee Zong Hng
    * Date: 5/08/2025
    * Description: Triggers the end sequence when the player enters the trigger zone.
    */
    public PlayerControl playerControl; /// Reference to the PlayerControl script
    public Camera playerCamera; /// Reference to the player camera
    public Transform playerTransform; /// Reference to the player transform
    public float cameraPanDuration = 2f; /// Duration of the camera pan
    public Vector3 cameraOffset = new Vector3(0, 1.5f, -3f); /// Offset for the camera position
    public SceneFader sceneFader; /// Reference to the SceneFader script
    public string nextSceneName;

    [Header("Objects to Delete")]
    public List<GameObject> objectsToDelete; /// Assign in Inspector

    [Header("Objects to Unhide")]
    public List<GameObject> objectsToUnhide; /// Assign in Inspector

    private bool hasTriggered = false;

    void OnTriggerEnter(Collider other) /// Handle trigger events
    {
        if (hasTriggered) return;
        if (other.CompareTag("Player"))
        {
            hasTriggered = true;
            StartCoroutine(EndSequenceRoutine());
        }
    }

    IEnumerator EndSequenceRoutine() /// Handle the end sequence
    {
        // Freeze player movement
        if (playerControl != null)
            playerControl.enabled = false;

        if (objectsToDelete != null)
        {
            foreach (var obj in objectsToDelete)
            {
                if (obj != null)
                    Destroy(obj);
            }
        }

        foreach (var enemy in FindObjectsOfType<PoliceChaser>())
        {
            if (enemy != null)
                Destroy(enemy.gameObject);
        }

        if (objectsToUnhide != null)
        {
            foreach (var obj in objectsToUnhide)
            {
                if (obj != null)
                    obj.SetActive(true);
            }
        }

        // Pan camera behind player
        Vector3 startPos = playerCamera.transform.position;
        Quaternion startRot = playerCamera.transform.rotation;
        Vector3 targetPos = playerTransform.position + playerTransform.TransformDirection(cameraOffset);
        Quaternion targetRot = Quaternion.LookRotation(playerTransform.position - targetPos, Vector3.up);

        float t = 0f;
        while (t < cameraPanDuration)
        {
            t += Time.deltaTime;
            float lerp = Mathf.Clamp01(t / cameraPanDuration);
            playerCamera.transform.position = Vector3.Lerp(startPos, targetPos, lerp);
            playerCamera.transform.rotation = Quaternion.Slerp(startRot, targetRot, lerp);
            yield return null;
        }
        playerCamera.transform.position = targetPos;
        playerCamera.transform.rotation = targetRot;

        // Fade to black
        if (sceneFader != null)
        {
            yield return sceneFader.FadeAndSwitchSceneCoroutine(nextSceneName);
        }
        else
        {
            yield return new WaitForSeconds(1f);
            SceneManager.LoadScene(nextSceneName);
        }
    }
}