using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class EmissionFlicker : MonoBehaviour
{
    /*
    * Author: Jaasper Lee Zong Hng
    * Date: 1/08/2025
    * Description: Flickering emission effect for Unity materials
    */

    public Color emissionColor = Color.white; /// Emission color
    public float minIntensity = 0.1f; /// Minimum intensity
    public float maxIntensity = 2.0f; /// Maximum intensity
    public float flickerSpeed = 10f; /// Flicker speed
    public float randomFlickerAmount = 0.5f; /// Random flicker amount

    private Material mat;
    private float baseIntensity;

    void Start()
    {
        mat = GetComponent<Renderer>().material;
        baseIntensity = maxIntensity;
        mat.EnableKeyword("_EMISSION");
    }

    void Update() /// Handle flickering effect
    {
        // Flicker using Perlin noise and random bursts
        float noise = Mathf.PerlinNoise(Time.time * flickerSpeed, 0.0f);
        float randomBurst = Random.Range(-randomFlickerAmount, randomFlickerAmount);
        float intensity = Mathf.Clamp(noise * baseIntensity + randomBurst, minIntensity, maxIntensity);

        mat.SetColor("_EmissionColor", emissionColor * intensity);
    }
}