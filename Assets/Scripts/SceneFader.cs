using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneFader : MonoBehaviour
{
    public Image fadeImage; // Assign a full-screen UI Image (black, alpha 1)
    public float fadeDuration = 1f;

    void Start()
    {
        if (fadeImage != null)
        {
            // Start fully transparent
            Color c = fadeImage.color;
            c.a = 0f;
            fadeImage.color = c;
        }
    }

    public void FadeAndSwitchScene(string sceneName)
    {
        StartCoroutine(FadeRoutine(sceneName));
    }

    private IEnumerator FadeRoutine(string sceneName)
    {
        Debug.Log("Fading out to switch scene: " + sceneName);
        // Fade out
        yield return StartCoroutine(Fade(0f, 1f));
        // Load scene
        SceneManager.LoadScene(sceneName);
        // Wait one frame for scene to load
        yield return null;
        // Fade in
        yield return StartCoroutine(Fade(1f, 0f));
    }

    private IEnumerator Fade(float from, float to)
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(from, to, t / fadeDuration);
            if (fadeImage != null)
            {
                Color c = fadeImage.color;
                c.a = a;
                fadeImage.color = c;
            }
            yield return null;
        }
        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = to;
            fadeImage.color = c;
        }
    }

    public IEnumerator FadeAndSwitchSceneCoroutine(string sceneName)
    {
        yield return StartCoroutine(Fade(0f, 1f)); // Fade out
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
}