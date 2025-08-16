using UnityEngine;
using UnityEngine.UI;

public class SprintUI : MonoBehaviour
{
    /*
    * Author: Jaasper Lee Zong Hng
    * Date: 26/07/2025
    * Description: Sprint UI script for Unity
      Controls the user interface elements related to sprinting.
    */

    public PlayerControl playerControl; /// Drag your PlayerControl object here
    public Slider sprintSlider;         /// Drag your SprintSlider here
    public CanvasGroup sprintCanvasGroup; /// Drag your CanvasGroup here

    [Range(0f, 1f)]
    public float translucentAlpha = 0.3f; /// Alpha when not sprinting

    void Start()
    {
        sprintSlider.maxValue = playerControl.sprintDuration;
    }

    void Update() /// Update the UI elements
    {
        sprintSlider.value = playerControl.sprintTimer;

        // Set alpha based on sprinting state
        if (playerControl.isSprinting)
            sprintCanvasGroup.alpha = 1f; // Fully visible
        else
            sprintCanvasGroup.alpha = translucentAlpha; // Translucent
    }
}