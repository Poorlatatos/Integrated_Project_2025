using UnityEngine;
using UnityEngine.AI;

public class EnemyBasics : MonoBehaviour
{
    /*
    * Author: Jaasper Lee Zong Hng
    * Date: 1/08/2025
    * Description: Basic enemy AI script for Unity
    */
    public enum State { Inactive, Patrolling, Chasing, Idle }
    public State currentState = State.Inactive;

    public Transform[] patrolPoints; /// Array of patrol points for the enemy
    public float idleTime = 2f; /// Time to wait at each patrol point
    public float chaseDistance = 10f; /// Distance at which the enemy will start chasing the player
    public float stopChaseDistance = 15f; /// Distance at which the enemy will stop chasing the player
    public float idleRotationSpeed = 60f; /// Speed at which the enemy will rotate while idling
    public Transform player; /// Reference to the player transform

    [Header("Detection Settings")] /// Settings for enemy detection
    public float detectionRadius = 10f; /// Radius within which the enemy can detect the player
    public float detectionAngle = 60f; /// Field of view in degrees
    public LayerMask detectionLayerMask; /// Layer mask for detection
    public LayerMask obstructionMask; /// Layer mask for obstructions

    public NavMeshAgent agent;
    public int patrolIndex = 0;
    public float idleTimer = 0f;

    public float chaseTimeout = 3f; /// How long to keep chasing after losing sight
    public float chaseTimer = 0f;

    [Header("Special Patrol Point")]
    public int specialPatrolIndex = 0; /// Set this to the index of the special patrol point in the Inspector
    public float specialIdleTime = 5f; /// How long to idle at the special point
    public Animator animator; /// Reference to the enemy animator

    [Header("Enemy Sounds")]
    public AudioClip[] randomSounds;
    public AudioSource audioSource;
    public float minSoundInterval = 5f;
    public float maxSoundInterval = 15f;

private float soundTimer = 0f;
private float nextSoundTime = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (patrolPoints.Length > 0)
        {
            // Place at special patrol point if inactive
            if (currentState == State.Inactive)
            {
                transform.position = patrolPoints[specialPatrolIndex].position;
                agent.Warp(patrolPoints[specialPatrolIndex].position);
            }
            else
            {
                agent.SetDestination(patrolPoints[patrolIndex].position);
            }
        }
        SetNextSoundTime();
    }

    private void SetNextSoundTime() /// Set the next sound playback time
    {
        nextSoundTime = Random.Range(minSoundInterval, maxSoundInterval);
        soundTimer = 0f;
    }

    void Update()
    {
        if (currentState == State.Inactive)
            return; // Do nothing if inactive

        // Set walking animation based on movement and state
        bool isWalking = (currentState == State.Patrolling || currentState == State.Chasing) && agent.velocity.magnitude > 0.1f;
        if (animator != null)
        {
            animator.SetBool("isWalking", isWalking);
            animator.SetBool("isIdle", currentState == State.Idle);
        }

        bool canSeePlayer = CanSeePlayer();
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        switch (currentState)
        {
            case State.Patrolling:
                Patrol();
                if (canSeePlayer && distanceToPlayer < detectionRadius)
                {
                    currentState = State.Chasing;
                    chaseTimer = chaseTimeout; // Start the chase timer
                }
                break;

            case State.Chasing:
                Chase();
                if (canSeePlayer)
                {
                    chaseTimer = chaseTimeout; // Reset timer if player is seen
                }
                else
                {
                    chaseTimer -= Time.deltaTime;
                    if (chaseTimer <= 0f || distanceToPlayer > stopChaseDistance)
                    {
                        patrolIndex = GetNearestPatrolPointIndex();
                        currentState = State.Patrolling;
                        if (patrolPoints.Length > 0)
                            agent.SetDestination(patrolPoints[patrolIndex].position);
                    }
                }
                break;

            case State.Idle:
                Idle();
                break;
        }

        if (agent.velocity.sqrMagnitude > 0.1f)
        {
            Vector3 lookDirection = agent.velocity.normalized;
            lookDirection.y = 0; // Keep only horizontal rotation
            if (lookDirection != Vector3.zero)
                transform.forward = lookDirection;
        }
        // Play random sound at random intervals
        if (randomSounds != null && randomSounds.Length > 0 && audioSource != null)
        {
            soundTimer += Time.deltaTime;
            if (soundTimer >= nextSoundTime)
            {
                AudioClip clip = randomSounds[Random.Range(0, randomSounds.Length)];
                audioSource.PlayOneShot(clip);
                SetNextSoundTime();
            }
        }
    }

    public int GetNearestPatrolPointIndex() /// Get the index of the nearest patrol point
    {
        int nearestIndex = 0;
        float minDist = Mathf.Infinity;
        for (int i = 0; i < patrolPoints.Length; i++)
        {
            float dist = Vector3.Distance(transform.position, patrolPoints[i].position);
            if (dist < minDist)
            {
                minDist = dist;
                nearestIndex = i;
            }
        }
        return nearestIndex;
    }

    public bool CanSeePlayer() /// Check if the enemy can see the player
    {
        Vector3 enemyEyePos = transform.position + Vector3.up * 1.5f;
        Vector3 playerHeadPos = player.position + Vector3.up * 1.0f;
        Vector3 dirToPlayer = (playerHeadPos - enemyEyePos);
        float distanceToPlayer = dirToPlayer.magnitude;

        // Draw the ray to the player
        Debug.DrawLine(enemyEyePos, playerHeadPos, Color.yellow);

        if (distanceToPlayer > detectionRadius)
            return false;

        // Angle check (cone)
        dirToPlayer.Normalize();
        Vector3 forward = transform.forward;
        float angleToPlayer = Vector3.Angle(forward, dirToPlayer);

        // Draw the cone boundaries
        Quaternion leftRayRotation = Quaternion.AngleAxis(-detectionAngle * 0.5f, Vector3.up);
        Quaternion rightRayRotation = Quaternion.AngleAxis(detectionAngle * 0.5f, Vector3.up);
        Vector3 leftRayDirection = leftRayRotation * forward;
        Vector3 rightRayDirection = rightRayRotation * forward;
        Debug.DrawRay(enemyEyePos, leftRayDirection * detectionRadius, Color.cyan);
        Debug.DrawRay(enemyEyePos, rightRayDirection * detectionRadius, Color.cyan);

        if (angleToPlayer > detectionAngle * 0.5f)
            return false;

        // Line of sight check
        Ray ray = new Ray(enemyEyePos, dirToPlayer);
        if (Physics.Raycast(ray, out RaycastHit hit, detectionRadius, detectionLayerMask | obstructionMask))
        {
            Debug.DrawLine(enemyEyePos, hit.point, hit.transform == player ? Color.green : Color.red);
            if (hit.transform == player)
                return true;
        }
        return false;
    }

    void Patrol() /// Handle patrolling behavior
    {
        if (patrolPoints.Length == 0) return;

        agent.isStopped = false;
        agent.SetDestination(patrolPoints[patrolIndex].position);

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            currentState = State.Idle;
            idleTimer = 0f;
        }
    }

    void Chase() /// Handle chasing behavior
    {
        agent.isStopped = false;
        agent.SetDestination(player.position);
    }

    void Idle() /// Handle idle behavior
    {
        agent.isStopped = true;
        idleTimer += Time.deltaTime;
        transform.Rotate(Vector3.up, idleRotationSpeed * Time.deltaTime);

        // Use special idle time if at the special patrol point
        float currentIdleTime = (patrolIndex == specialPatrolIndex) ? specialIdleTime : idleTime;

        if (idleTimer >= currentIdleTime)
        {
            // Randomize next patrol point (not the same as current)
            int nextIndex = patrolIndex;
            if (patrolPoints.Length > 1)
            {
                while (nextIndex == patrolIndex)
                    nextIndex = Random.Range(0, patrolPoints.Length);
            }
            else
            {
                nextIndex = 0;
            }
            patrolIndex = nextIndex;
            currentState = State.Patrolling;
            agent.SetDestination(patrolPoints[patrolIndex].position);
        }
    }

    void OnCollisionEnter(Collision collision) /// Handle collision events
    {
        Debug.Log("Enemy collided with: " + collision.gameObject.name + " | Tag: " + collision.gameObject.tag);

        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Enemy touched player!");
            JumpscareManager jm = FindFirstObjectByType<JumpscareManager>();
            if (jm != null)
                jm.TriggerJumpscare();
            else
                Debug.LogWarning("JumpscareManager not found!");
        }
    }
}