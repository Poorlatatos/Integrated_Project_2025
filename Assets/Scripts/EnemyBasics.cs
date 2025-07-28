using UnityEngine;
using UnityEngine.AI;

public class EnemyBasics : MonoBehaviour
{
    public enum State { Patrolling, Chasing, Idle }
    public State currentState = State.Patrolling;

    public Transform[] patrolPoints;
    public float idleTime = 2f;
    public float chaseDistance = 10f;
    public float stopChaseDistance = 15f;
    public float idleRotationSpeed = 60f;
    public Transform player;

    [Header("Detection Settings")]
    public float detectionRadius = 10f;
    public LayerMask detectionLayerMask;
    public LayerMask obstructionMask;

    private NavMeshAgent agent;
    private int patrolIndex = 0;
    private float idleTimer = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (patrolPoints.Length > 0)
            agent.SetDestination(patrolPoints[patrolIndex].position);
    }

    void Update()
    {
        bool canSeePlayer = CanSeePlayer();
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        switch (currentState)
        {
            case State.Patrolling:
                Patrol();
                if (canSeePlayer && distanceToPlayer < detectionRadius)
                    currentState = State.Chasing;
                break;

            case State.Chasing:
                Chase();
                if (!canSeePlayer || distanceToPlayer > stopChaseDistance)
                {
                    patrolIndex = GetNearestPatrolPointIndex();
                    currentState = State.Patrolling;
                    if (patrolPoints.Length > 0)
                        agent.SetDestination(patrolPoints[patrolIndex].position);
                }
                break;

            case State.Idle:
                Idle();
                break;
        }
    }

    int GetNearestPatrolPointIndex()
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

    bool CanSeePlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer > detectionRadius)
            return false;

        Vector3 enemyEyePos = transform.position + Vector3.up * 1.5f;
        Vector3 playerHeadPos = player.position + Vector3.up * 1.0f;
        Vector3 dirToPlayer = (playerHeadPos - enemyEyePos).normalized;
        Ray ray = new Ray(enemyEyePos, dirToPlayer);

        if (Physics.Raycast(ray, out RaycastHit hit, detectionRadius, detectionLayerMask | obstructionMask))
        {
            Debug.Log("Raycast hit: " + hit.transform.name + " on layer " + LayerMask.LayerToName(hit.transform.gameObject.layer));
            Debug.DrawLine(ray.origin, hit.point, Color.red, 0.1f);
            if (hit.transform == player)
                return true;
        }
        else
        {
            Debug.Log("Raycast hit nothing.");
            Debug.DrawRay(ray.origin, ray.direction * detectionRadius, Color.yellow, 0.1f);
        }
        return false;
    }

    void Patrol()
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

    void Chase()
    {
        agent.isStopped = false;
        agent.SetDestination(player.position);
    }

    void Idle()
    {
        agent.isStopped = true;
        idleTimer += Time.deltaTime;
        transform.Rotate(Vector3.up, idleRotationSpeed * Time.deltaTime);

        if (idleTimer >= idleTime)
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
}