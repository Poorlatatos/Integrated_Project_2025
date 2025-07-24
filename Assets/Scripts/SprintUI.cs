using UnityEngine;
using UnityEngine.UI;

public class SprintUI : MonoBehaviour
{
    public PlayerControl playerControl; // Drag your PlayerControl object here
    public Slider sprintSlider;         // Drag your SprintSlider here

    void Start()
    {
        sprintSlider.maxValue = playerControl.sprintDuration;
    }

    void Update()
    {
        sprintSlider.value = playerControl.sprintTimer;
    }
}