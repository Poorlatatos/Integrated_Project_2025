using UnityEngine;
using UnityEngine.SceneManagement;

public class JumpscareManager : MonoBehaviour
{
    /*
    * Author: Jaasper Lee Zong Hng
    * Date: 1/08/2025
    * Description: Jumpscare manager script for Unity
      Controls the behavior of jumpscares, including camera shaking and UI display.
    */

    public Camera playerCamera;
    public float shakeDuration = 1f; /// Duration of the shake
    public float shakeMagnitude = 0.5f; /// Magnitude of the shake
    public GameObject jumpscareUI; /// Assign a UI panel or image for jumpscare
    public MonoBehaviour playerControlScript; /// Assign the movement script in the Inspector
    public MonoBehaviour cameraLookScript; /// Assign the camera look script in the Inspector
    public Transform enemyTransform; /// Assign an enemy transform to shake camera towards
    private Vector3 originalCamPos;
    private bool isShaking = false;
    private float shakeTimer = 0f;

    [Header("Jumpscare Audio")]
    public AudioSource jumpscareAudioSource; /// Assign in Inspector
    public AudioClip jumpscareClip; /// Assign in Inspector

    void Start()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;
        if (jumpscareUI != null)
            jumpscareUI.SetActive(false);
    }

    public void TriggerJumpscare() /// Trigger the jumpscare sequence
    {
        if (isShaking) return;
        if (jumpscareUI != null)
            jumpscareUI.SetActive(true);
        originalCamPos = playerCamera.transform.localPosition;

        // Play jumpscare audio
        if (jumpscareAudioSource != null && jumpscareClip != null)
            jumpscareAudioSource.PlayOneShot(jumpscareClip);

        // Instantly lock camera to enemy's head
        if (enemyTransform != null)
        {
            Vector3 dir = (enemyTransform.position - playerCamera.transform.position).normalized;
            Quaternion lookRot = Quaternion.LookRotation(dir, Vector3.up);
            playerCamera.transform.rotation = lookRot;
        }

        isShaking = true;
        shakeTimer = shakeDuration;

        // Freeze player controls
        if (playerControlScript != null)
            playerControlScript.enabled = false;
        if (cameraLookScript != null)
            cameraLookScript.enabled = false;

        Invoke(nameof(GameOver), shakeDuration);
    }

    void Update()
    {
        if (isShaking)
        {
            if (enemyTransform != null)
            {
                // Look at the enemy (lock camera)
                Vector3 dir = (enemyTransform.position - playerCamera.transform.position).normalized;
                Quaternion lookRot = Quaternion.LookRotation(dir, Vector3.up);
                playerCamera.transform.rotation = Quaternion.Slerp(playerCamera.transform.rotation, lookRot, Time.deltaTime * 8f);
            }

            if (shakeTimer > 0)
            {
                playerCamera.transform.localPosition = originalCamPos + Random.insideUnitSphere * shakeMagnitude;
                shakeTimer -= Time.deltaTime;
            }
            else
            {
                playerCamera.transform.localPosition = originalCamPos;
                isShaking = false;
            }
        }
    }

    void GameOver() /// Handle game over state
    {
        // Optionally: show game over UI, reload scene, etc.
        // For now, reload the current scene:
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}