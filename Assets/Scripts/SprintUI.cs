using UnityEngine;
using UnityEngine.UI;

public class SprintUI : MonoBehaviour
{
    public PlayerControl playerControl; // Drag your PlayerControl object here
    public Slider sprintSlider;         // Drag your SprintSlider here
    public CanvasGroup sprintCanvasGroup; // Drag your CanvasGroup here

    [Range(0f, 1f)]
    public float translucentAlpha = 0.3f; // Alpha when not sprinting

    void Start()
    {
        sprintSlider.maxValue = playerControl.sprintDuration;
    }

    void Update()
    {
        sprintSlider.value = playerControl.sprintTimer;

        // Set alpha based on sprinting state
        if (playerControl.isSprinting)
            sprintCanvasGroup.alpha = 1f; // Fully visible
        else
            sprintCanvasGroup.alpha = translucentAlpha; // Translucent
    }
}