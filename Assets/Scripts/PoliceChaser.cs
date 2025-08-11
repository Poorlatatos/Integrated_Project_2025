using UnityEngine;
using UnityEngine.AI;

public class PoliceChaser : MonoBehaviour
{
    public Transform player;
    public float chaseSpeed = 6f;
    private NavMeshAgent agent;

    [Header("Police Lights")]
    public Light redLight;
    public Light blueLight;
    public float flashInterval = 0.3f;

    private float flashTimer = 0f;
    private bool redOn = true;

    [Header("Siren")]
    public AudioSource sirenSource; // Assign in Inspector
    public AudioClip sirenClip;     // Assign in Inspector

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent != null)
            agent.speed = chaseSpeed;

        if (redLight != null) redLight.enabled = true;
        if (blueLight != null) blueLight.enabled = false;

        // Do NOT play siren here! Wait for EscapeTrigger to trigger it.
        if (sirenSource != null)
            sirenSource.Stop();
    }

    void Update()
    {
        if (player != null && agent != null)
        {
            agent.SetDestination(player.position);
        }

        flashTimer += Time.deltaTime;
        if (flashTimer >= flashInterval)
        {
            flashTimer = 0f;
            redOn = !redOn;
            if (redLight != null) redLight.enabled = redOn;
            if (blueLight != null) blueLight.enabled = !redOn;
        }
    }

    // Call this method to start the siren
    public void PlaySiren()
    {
        if (sirenSource != null && sirenClip != null && !sirenSource.isPlaying)
        {
            Debug.Log("Playing siren!");
            sirenSource.clip = sirenClip;
            sirenSource.loop = true;
            sirenSource.Play();
        }
    }
}