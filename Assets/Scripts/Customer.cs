using UnityEngine;
using UnityEngine.AI;

public class Customer : MonoBehaviour
{
    public Transform[] wanderPoints; // Assign points around the shop
    public float wanderSpeed = 1.5f;
    public float runSpeed = 4f; // Speed when reporting to shopkeeper
    public float glanceInterval = 2f;
    public float glanceAngle = 60f;
    public float glanceDuration = 1f;
    public float detectionRadius = 7f;
    public float detectionAngle = 90f;
    public LayerMask playerLayer;
    public LayerMask obstructionMask;

    public EnemyBasics shopkeeper; // Assign the shopkeeper (EnemyBasics) in Inspector
    public Transform player;       // Assign the player in Inspector

    private NavMeshAgent agent;
    private int currentWanderIndex = 0;
    private float glanceTimer = 0f;
    private bool isGlancing = false;
    private float glanceDirection = 1f;
    private bool hasReported = false;
    public Animator animator;
    private Vector3 lastPosition;
    private enum State { Wandering, Glancing, Reporting }
    private State currentState = State.Wandering;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        lastPosition = transform.position;
        agent.speed = wanderSpeed;
        if (wanderPoints.Length > 0)
            agent.SetDestination(wanderPoints[0].position);
    }

    void Update()
    {
        bool isWalking = agent.velocity.magnitude > 0.1f && (currentState == State.Wandering || currentState == State.Reporting);
        animator.SetBool("isWalking", isWalking);
        
        switch (currentState)
        {
            case State.Wandering:
                agent.speed = wanderSpeed;
                Wander();
                GlanceCheck();
                break;
            case State.Glancing:
                agent.speed = wanderSpeed;
                Glance();
                GlanceCheck();
                break;
            case State.Reporting:
                agent.speed = runSpeed;
                ReportToShopkeeper();
                break;
        }
    }

    void Wander()
    {
        agent.isStopped = false;
        if (wanderPoints.Length == 0) return;

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            // Start glancing around
            currentState = State.Glancing;
            glanceTimer = 0f;
            glanceDirection = Random.value > 0.5f ? 1f : -1f;
        }
    }

    void Glance()
    {
        agent.isStopped = true;
        glanceTimer += Time.deltaTime;
        // Slowly rotate left/right to simulate glancing
        transform.Rotate(Vector3.up, glanceDirection * glanceAngle * Time.deltaTime / glanceDuration);

        if (glanceTimer >= glanceDuration)
        {
            // Move to next wander point
            currentWanderIndex = (currentWanderIndex + 1) % wanderPoints.Length;
            agent.SetDestination(wanderPoints[currentWanderIndex].position);
            currentState = State.Wandering;
        }
    }

    void GlanceCheck()
    {
        // Visualize the customer's forward direction
        Debug.DrawRay(transform.position + Vector3.up * 1.5f, transform.forward * detectionRadius, Color.cyan);

        // Visualize the detection cone boundaries
        Quaternion leftRayRotation = Quaternion.AngleAxis(-detectionAngle * 0.5f, Vector3.up);
        Quaternion rightRayRotation = Quaternion.AngleAxis(detectionAngle * 0.5f, Vector3.up);
        Vector3 leftRayDirection = leftRayRotation * transform.forward;
        Vector3 rightRayDirection = rightRayRotation * transform.forward;
        Debug.DrawRay(transform.position + Vector3.up * 1.5f, leftRayDirection * detectionRadius, Color.green);
        Debug.DrawRay(transform.position + Vector3.up * 1.5f, rightRayDirection * detectionRadius, Color.green);

        // Only check if player is snooping (e.g., holding an item)
        FirstPersonCamera cam = player.GetComponent<FirstPersonCamera>();
        if (cam != null && cam.heldItem != null && !hasReported)
        {
            Debug.Log("Customer sees player holding an item!");
            Vector3 toPlayer = player.position - transform.position;
            float distance = toPlayer.magnitude;
            if (distance <= detectionRadius)
            {
                float angle = Vector3.Angle(transform.forward, toPlayer.normalized);
                if (angle <= detectionAngle * 0.5f)
                {
                    Debug.Log("Player is within detection cone!");
                    if (!Physics.Raycast(transform.position + Vector3.up * 1.5f, toPlayer.normalized, distance, obstructionMask))
                    {
                        Debug.Log("Customer has line of sight to player! Reporting...");
                        hasReported = true;
                        currentState = State.Reporting;
                        agent.SetDestination(shopkeeper.transform.position);
                    }
                    else
                    {
                        Debug.Log("Line of sight blocked.");
                    }
                }
                else
                {
                    Debug.Log("Player not in detection angle.");
                }
            }
            else
            {
                Debug.Log("Player not in detection radius.");
            }
        }
    }

    void ReportToShopkeeper()
    {
        agent.isStopped = false;
        agent.SetDestination(shopkeeper.transform.position);

        if (!agent.pathPending && agent.remainingDistance < 1.5f)
        {
            // Inform the shopkeeper
            if (shopkeeper.currentState == EnemyBasics.State.Patrolling || shopkeeper.currentState == EnemyBasics.State.Idle)
            {
                shopkeeper.currentState = EnemyBasics.State.Chasing;
                shopkeeper.chaseTimer = shopkeeper.chaseTimeout;
            }
            // Customer resumes wandering after reporting
            hasReported = false;
            currentState = State.Wandering;
            agent.speed = wanderSpeed;
            agent.SetDestination(wanderPoints[currentWanderIndex].position);
        }
    }
    public void TryReportPlayerStealing()
    {
        if (hasReported) return;

        Vector3 toPlayer = player.position - transform.position;
        float distance = toPlayer.magnitude;
        if (distance <= detectionRadius)
        {
            float angle = Vector3.Angle(transform.forward, toPlayer.normalized);
            if (angle <= detectionAngle * 0.5f)
            {
                if (!Physics.Raycast(transform.position + Vector3.up * 1.5f, toPlayer.normalized, distance, obstructionMask))
                {
                    Debug.Log("Customer SEES player stealing! Reporting...");
                    hasReported = true;
                    currentState = State.Reporting;
                    agent.SetDestination(shopkeeper.transform.position);
                }
                else
                {
                    Debug.Log("Customer's view is blocked, does not report.");
                }
            }
            else
            {
                Debug.Log("Player not in detection angle, does not report.");
            }
        }
        else
        {
            Debug.Log("Player not in detection radius, does not report.");
        }
    }
}