using UnityEngine;

public class Sprint3DBar : MonoBehaviour
{
    /*
    * Author: Jaasper Lee Zong Hng
    * Date: 4/08/2025
    * Description: Sprint 3D bar script for Unity
      Controls the visual representation of the player's sprinting stamina.
    */

    public PlayerControl playerControl; /// Assign your PlayerControl in Inspector
    public float minScaleY = 0.05f; /// Minimum scale when empty
    public float maxScaleY = 1f;    /// Maximum scale when full
    public Transform barTransform; /// Assign the 3D bar object here

    void Update()
    {
        if (playerControl == null || barTransform == null) return;

        float percent = Mathf.Clamp01(playerControl.sprintTimer / playerControl.sprintDuration);
        Vector3 scale = barTransform.localScale;
        scale.y = Mathf.Lerp(minScaleY, maxScaleY, percent);
        barTransform.localScale = scale;
    }
}