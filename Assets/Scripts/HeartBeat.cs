using UnityEngine;

public class HeartBeat : MonoBehaviour
{
    [Header("Heart Beat Settings")]
    public float minBeatSpeed = 0.5f; // Slowest beat (seconds per beat)
    public float maxBeatSpeed = 0.1f; // Fastest beat (seconds per beat)
    public float minScale = 1f;
    public float maxScale = 1.2f;

    [Header("References")]
    public ParanoiaMeter paranoiaMeter; // Assign in Inspector

    private float beatTimer = 0f;
    private bool isBeating = false;

    void Update()
    {
        if (paranoiaMeter == null) return;

        // Calculate paranoia percent (0 to 1)
        float paranoiaPercent = paranoiaMeter.paranoia / paranoiaMeter.maxParanoia;

        // Interpolate beat speed and scale
        float beatSpeed = Mathf.Lerp(minBeatSpeed, maxBeatSpeed, paranoiaPercent);
        float targetScale = Mathf.Lerp(minScale, maxScale, paranoiaPercent);

        beatTimer += Time.deltaTime;
        if (beatTimer >= beatSpeed)
        {
            beatTimer = 0f;
            if (!isBeating)
                StartCoroutine(Beat(targetScale));
        }
    }

    System.Collections.IEnumerator Beat(float targetScale)
    {
        isBeating = true;
        Vector3 originalScale = transform.localScale;
        Vector3 beatScale = Vector3.one * targetScale;

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
}