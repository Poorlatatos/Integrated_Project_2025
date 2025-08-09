using UnityEngine;
using System.Collections;
public class HeartBeat : MonoBehaviour
{
    [Header("Heart Beat Settings")]
    public float minBeatSpeed = 0.5f; // Slowest beat (seconds per beat)
    public float maxBeatSpeed = 0.1f; // Fastest beat (seconds per beat)
    public float minScale = 1f;
    public float maxScale = 1.2f;

    [Header("References")]
    public ParanoiaMeter paranoiaMeter; // Assign in Inspector
    public CanvasGroup paranoiaFlashGroup; // Assign your ParanoiaFlashImage's CanvasGroup here
    public float paranoiaFlashThreshold = 0.6f; // 60% paranoia
    public float flashAlpha = 0.3f; // How visible the flash is
    public float flashFadeTime = 0.15f; // How quickly the flash fades out
    private float beatTimer = 0f;
    private bool isBeating = false;

    [Header("Flash Settings")]
    public float flashInterval = 1.0f; // How often to flash (seconds)
    private Coroutine flashCoroutine;

    [Header("Audio")]
    public AudioSource audioSource; // Assign in Inspector
    public AudioClip heartBeatClip; // Assign in Inspector

    void Start()
    {
        if (paranoiaFlashGroup != null)
            paranoiaFlashGroup.alpha = 0f;
    }
    void Update()
    {
        if (paranoiaMeter == null) return;

        float paranoiaPercent = paranoiaMeter.paranoia / paranoiaMeter.maxParanoia;

        // Heartbeat logic (unchanged)
        float beatSpeed = Mathf.Lerp(minBeatSpeed, maxBeatSpeed, paranoiaPercent);
        float targetScale = Mathf.Lerp(minScale, maxScale, paranoiaPercent);

        beatTimer += Time.deltaTime;
        if (beatTimer >= beatSpeed)
        {
            beatTimer = 0f;
            if (!isBeating)
                StartCoroutine(Beat(targetScale));
        }

        // Flashing logic
        if (paranoiaPercent >= paranoiaFlashThreshold)
        {
            if (flashCoroutine == null)
                flashCoroutine = StartCoroutine(FlashingRoutine());
        }
        else
        {
            if (flashCoroutine != null)
            {
                StopCoroutine(flashCoroutine);
                flashCoroutine = null;
                if (paranoiaFlashGroup != null)
                    paranoiaFlashGroup.alpha = 0f;
            }
        }
    }

    IEnumerator Beat(float targetScale)
    {
        isBeating = true;
        Vector3 originalScale = transform.localScale;
        Vector3 beatScale = Vector3.one * targetScale;

        // Play heartbeat sound
        if (audioSource != null && heartBeatClip != null)
            audioSource.PlayOneShot(heartBeatClip);

        // Scale up
        float t = 0f;
        while (t < 0.1f)
        {
            transform.localScale = Vector3.Lerp(originalScale, beatScale, t / 0.1f);
            t += Time.deltaTime;
            yield return null;
        }
        transform.localScale = beatScale;

        // Scale down
        t = 0f;
        while (t < 0.15f)
        {
            transform.localScale = Vector3.Lerp(beatScale, originalScale, t / 0.15f);
            t += Time.deltaTime;
            yield return null;
        }
        transform.localScale = originalScale;

        isBeating = false;
    }

    private IEnumerator FlashParanoiaImage()
    {
        paranoiaFlashGroup.alpha = flashAlpha;
        float t = 0f;
        while (t < flashFadeTime)
        {
            paranoiaFlashGroup.alpha = Mathf.Lerp(flashAlpha, 0f, t / flashFadeTime);
            t += Time.deltaTime;
            yield return null;
        }
        paranoiaFlashGroup.alpha = 0f;
    }

    private IEnumerator FlashingRoutine()
    {
        while (true)
        {
            if (paranoiaFlashGroup != null)
            {
                paranoiaFlashGroup.alpha = flashAlpha;
                float t = 0f;
                while (t < flashFadeTime)
                {
                    paranoiaFlashGroup.alpha = Mathf.Lerp(flashAlpha, 0f, t / flashFadeTime);
                    t += Time.deltaTime;
                    yield return null;
                }
                paranoiaFlashGroup.alpha = 0f;
            }
            yield return new WaitForSeconds(flashInterval);
        }
    }
}