using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class StarWarsScroll : MonoBehaviour
{
    public RectTransform scrollText; // Assign the RectTransform of your scrolling text
    public float scrollSpeed = 40f;
    public float endY = 1000f; // Y position at which the scroll ends
    public float skipHoldTime = 2f; // How long to hold space to skip
    public CanvasGroup fadeGroup; // Optional: assign for fade-out effect
    public float fadeDuration = 1f;
    public string nextSceneName; // Leave blank to just disable scroll on skip

    [Header("Skip UI")]
    public TextMeshProUGUI skipText; // Assign in Inspector (e.g. "Hold SPACE to skip...")
    public string skipMessage = "Holding SPACE to skip...";

    private Vector2 startPos;
    private float skipTimer = 0f;
    private bool skipping = false;
    private bool finished = false;

    void Start()
    {
        if (scrollText != null)
            startPos = scrollText.anchoredPosition;
        if (fadeGroup != null)
            fadeGroup.alpha = 1f;
        if (skipText != null)
            skipText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (finished) return;

        // Scroll the text upwards
        if (scrollText != null && !skipping)
        {
            scrollText.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;
            if (scrollText.anchoredPosition.y >= endY)
            {
                EndScroll();
            }
        }

        // Skip logic
        if (Input.GetKey(KeyCode.Space))
        {
            skipTimer += Time.deltaTime;
            if (skipText != null)
            {
                skipText.text = skipMessage;
                skipText.gameObject.SetActive(true);
            }
            if (skipTimer >= skipHoldTime && !skipping)
            {
                skipping = true;
                if (skipText != null)
                    skipText.gameObject.SetActive(false);
                StartCoroutine(FadeAndEnd());
            }
        }
        else
        {
            skipTimer = 0f;
            if (skipText != null)
                skipText.gameObject.SetActive(false);
        }
    }

    private void EndScroll()
    {
        finished = true;
        if (!string.IsNullOrEmpty(nextSceneName))
            SceneManager.LoadScene(nextSceneName);
        else
            gameObject.SetActive(false);
    }

    private System.Collections.IEnumerator FadeAndEnd()
    {
        if (fadeGroup != null)
        {
            float t = 0f;
            while (t < fadeDuration)
            {
                fadeGroup.alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
                t += Time.deltaTime;
                yield return null;
            }
            fadeGroup.alpha = 0f;
        }
        EndScroll();
    }
}