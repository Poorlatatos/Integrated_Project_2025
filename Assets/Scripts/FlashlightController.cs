using UnityEngine;
using UnityEngine.InputSystem;

public class FlashlightController : MonoBehaviour
{
    /*
    * Author: Jaasper Lee Zong Hng
    * Date: 1/08/2025
    * Description: Flashlight controller script for Unity
      Manages the behavior of the flashlight, including turning it on/off and tracking its state.
    */
    public Light flashlightLight; /// Assign the Light component in Inspector
    public bool isHeld = false;   /// Set to true when held by player
    private bool isOn = false; /// Set to true when flashlight is on

    void Start()
    {
        if (flashlightLight != null)
            flashlightLight.enabled = false; /// Off by default
    }

    void Update()
    {
        if (!isHeld) return;

        // Toggle with left mouse button
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            isOn = !isOn;
            if (flashlightLight != null)
                flashlightLight.enabled = isOn;
        }
    }

    /// Call this when the player picks up the flashlight
    public void OnPickup()
    {
        isHeld = true;

    }

    /// Call this when the player drops the flashlight
    public void OnDrop()
    {
        isHeld = false;
        if (flashlightLight != null)
            flashlightLight.enabled = false;
    }
}