using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using System.Collections;

public class PoliceChaser : MonoBehaviour
{
    /*
    * Author: Jaasper Lee Zong Hng
    * Date: 12/08/2025
    * Description: Police AI script for Unity
      Controls the behavior of police officers in the game, including chasing the player and triggering alarms.
    */

    public Transform player; 
    public float chaseSpeed = 6f; /// Speed of the police chase
    private NavMeshAgent agent;

    [Header("Police Lights")]
    public Light redLight; /// Assign in Inspector
    public Light blueLight; /// Assign in Inspector
    public float flashInterval = 0.3f; /// Assign in Inspector

    private float flashTimer = 0f;
    private bool redOn = true;

    [Header("Siren")]
    public AudioSource sirenSource; /// Assign in Inspector
    public AudioClip sirenClip;     /// Assign in Inspector

    [Header("Caught Sequence")]
    public AudioClip caughtClip; /// Assign a "caught" or jumpscare sound
    public AudioSource caughtAudioSource; /// Assign a separate AudioSource for caught sound
    public MonoBehaviour playerControlScript; /// Assign your PlayerControl script
    public MonoBehaviour cameraLookScript;    /// Assign your camera look script (e.g., FirstPersonCamera)
    public Camera playerCamera;               /// Assign the player's camera
    public float caughtDuration = 2f;         /// How long to freeze before resetting

    private bool isCatching = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent != null)
            agent.speed = chaseSpeed;

        if (redLight != null) redLight.enabled = true;
        if (blueLight != null) blueLight.enabled = false;

        if (sirenSource != null)
            sirenSource.Stop();
    }

    void Update()
    {
        if (isCatching) return;

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

    void OnCollisionEnter(Collision collision) /// Handle collision events
    {
        if (isCatching) return;
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(CaughtSequence());
        }
    }

    private IEnumerator CaughtSequence() /// Coroutine for handling the caught sequence
    {
        isCatching = true;

        // Play caught audio
        if (caughtAudioSource != null && caughtClip != null)
            caughtAudioSource.PlayOneShot(caughtClip);

        // Freeze player movement and camera
        if (playerControlScript != null)
            playerControlScript.enabled = false;
        if (cameraLookScript != null)
            cameraLookScript.enabled = false;

        // Lock camera to look at police
        if (playerCamera != null)
        {
            Vector3 dir = (transform.position - playerCamera.transform.position).normalized;
            Quaternion lookRot = Quaternion.LookRotation(dir, Vector3.up);
            playerCamera.transform.rotation = lookRot;
        }

        yield return new WaitForSeconds(caughtDuration);

        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}