using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class EmissionFlicker : MonoBehaviour
{
    public Color emissionColor = Color.white;
    public float minIntensity = 0.1f;
    public float maxIntensity = 2.0f;
    public float flickerSpeed = 10f;
    public float randomFlickerAmount = 0.5f;

    private Material mat;
    private float baseIntensity;

    void Start()
    {
        mat = GetComponent<Renderer>().material;
        baseIntensity = maxIntensity;
        mat.EnableKeyword("_EMISSION");
    }

    void Update()
    {
        // Flicker using Perlin noise and random bursts
        float noise = Mathf.PerlinNoise(Time.time * flickerSpeed, 0.0f);
        float randomBurst = Random.Range(-randomFlickerAmount, randomFlickerAmount);
        float intensity = Mathf.Clamp(noise * baseIntensity + randomBurst, minIntensity, maxIntensity);

        mat.SetColor("_EmissionColor", emissionColor * intensity);
    }
}