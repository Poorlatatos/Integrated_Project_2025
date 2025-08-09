using UnityEngine;

public class ParanoiaMeter : MonoBehaviour
{
    [Header("Paranoia Settings")]
    public float paranoia = 0f;
    public float maxParanoia = 100f;
    public float paranoiaIncreasePerItem = 10f;
    public float paranoiaDecreaseRate = 2f; // Amount paranoia decreases per second

    [Header("References")]
    public EnemyBasics enemy; // Assign your EnemyBasics script in Inspector
    public PlayerControl player; // Assign your PlayerControl script in Inspector

    void Update()
    {
        // Decrease paranoia slowly over time
        if (paranoia > 0f)
        {
            paranoia -= paranoiaDecreaseRate * Time.deltaTime;
            if (paranoia < 0f) paranoia = 0f;
        }
    }

    // Call this when the player picks up an item
    public void IncreaseParanoia()
    {
        paranoia = Mathf.Min(paranoia + paranoiaIncreasePerItem, maxParanoia);

        // Activate enemy if not already active
        if (enemy != null && enemy.currentState == EnemyBasics.State.Inactive)
        {
            enemy.currentState = EnemyBasics.State.Patrolling;
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