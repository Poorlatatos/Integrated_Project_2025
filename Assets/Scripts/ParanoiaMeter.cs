using UnityEngine;
using UnityEngine.UI;

public class ParanoiaMeter : MonoBehaviour
{
    [Header("Paranoia Settings")]
    public float paranoia = 0f;
    public float maxParanoia = 100f;
    public float paranoiaIncreasePerItem = 10f;

    [Header("References")]
    public EnemyBasics enemy; // Assign your EnemyBasics script in Inspector
    public PlayerControl player; // Assign your PlayerControl script in Inspector
    public Slider paranoiaSlider; // Optional: assign a UI slider for paranoia

    void Start()
    {
        if (paranoiaSlider != null)
        {
            paranoiaSlider.maxValue = maxParanoia;
            paranoiaSlider.value = paranoia;
        }
    }

    // Call this when the player picks up an item
    public void IncreaseParanoia()
    {
        paranoia = Mathf.Min(paranoia + paranoiaIncreasePerItem, maxParanoia);
        if (paranoiaSlider != null)
            paranoiaSlider.value = paranoia;

        // Check if enemy can see the player
        if (enemy != null && enemy.CanSeePlayer())
        {
            enemy.currentState = EnemyBasics.State.Chasing;
            // Optionally reset chase timer if needed
        }
    }
}