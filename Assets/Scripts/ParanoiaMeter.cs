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

        // Activate enemy if not already active
        if (enemy != null && enemy.currentState == EnemyBasics.State.Inactive)
        {
            enemy.currentState = EnemyBasics.State.Patrolling;
            // Optionally, set the enemy to start patrolling from the nearest point
            enemy.patrolIndex = enemy.GetNearestPatrolPointIndex();
            enemy.agent.SetDestination(enemy.patrolPoints[enemy.patrolIndex].position);
        }

        // If enemy is active and can see the player, start chasing
        if (enemy != null && enemy.currentState != EnemyBasics.State.Inactive && enemy.CanSeePlayer())
        {
            enemy.currentState = EnemyBasics.State.Chasing;
            // Optionally reset chase timer if needed
        }
    }
}