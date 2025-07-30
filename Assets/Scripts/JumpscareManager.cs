using UnityEngine;
using UnityEngine.SceneManagement;

public class JumpscareManager : MonoBehaviour
{
    public Camera playerCamera;
    public float shakeDuration = 1f;
    public float shakeMagnitude = 0.5f;
    public GameObject jumpscareUI; // Assign a UI panel or image for jumpscare

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
        // Optionally: freeze player controls here
        Invoke(nameof(GameOver), shakeDuration);
    }

    void Update()
    {
        if (isShaking)
        {
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