using UnityEngine;
using UnityEngine.SceneManagement;

public class JumpscareManager : MonoBehaviour
{
    public Camera playerCamera;
    public float shakeDuration = 1f;
    public float shakeMagnitude = 0.5f;
    public GameObject jumpscareUI; // Assign a UI panel or image for jumpscare
    public MonoBehaviour playerControlScript; // Assign the movement script in the Inspector
    public Transform enemyTransform; // Assign an enemy transform to shake camera towards
    private Vector3 originalCamPos;
    private bool isShaking = false;
    private float shakeTimer = 0f;

    void Start()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;
        if (jumpscareUI != null)
            jumpscareUI.SetActive(false);
    }

    public void TriggerJumpscare()
    {
        if (isShaking) return;
        if (jumpscareUI != null)
            jumpscareUI.SetActive(true);
        originalCamPos = playerCamera.transform.localPosition;
        isShaking = true;
        shakeTimer = shakeDuration;

        // Freeze player controls
        if (playerControlScript != null)
            playerControlScript.enabled = false;

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

    void GameOver()
    {
        // Optionally: show game over UI, reload scene, etc.
        // For now, reload the current scene:
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}