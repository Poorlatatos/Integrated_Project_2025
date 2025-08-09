using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
public class PauseMenuManager : MonoBehaviour
{
    [Header("Menu Panels")]
    public GameObject pauseMenuPanel;
    public GameObject settingsMenuPanel;
    public GameObject controlsPanel; // Assign the controls panel

    [Header("Settings UI")]
    public Slider audioSlider; // Assign in Inspector
    public Slider sensitivitySlider; // Assign in Inspector
    public AudioMixer masterMixer; // Assign your AudioMixer in Inspector
    public FirstPersonCamera playerCamera; // Assign your FirstPersonCamera in Inspector
    public TextMeshProUGUI audioValueText;        // Assign in Inspector
    public TextMeshProUGUI sensitivityValueText;  // Assign in Inspector

    [Header("Game State")]
    public bool isPaused = false;

    private InputAction pauseAction;

    void Awake()
    {
        pauseAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/escape");
        pauseAction.performed += ctx => TogglePauseMenu();
        pauseAction.Enable();

        pauseMenuPanel.SetActive(false);
        settingsMenuPanel.SetActive(false);
        controlsPanel.SetActive(false);

        // Initialize sliders
        if (audioSlider != null && audioValueText != null)
        {
            float volume;
            masterMixer.GetFloat("Master", out volume);
            audioSlider.value = Mathf.Pow(10, volume / 20f); // Convert dB to [0,1]
            audioSlider.onValueChanged.AddListener(SetVolume);
            audioValueText.text = Mathf.RoundToInt(audioSlider.value * 100).ToString();
        }
        if (sensitivitySlider != null && sensitivityValueText != null)
        {
            sensitivitySlider.value = playerCamera.mouseSensitivity;
            sensitivitySlider.onValueChanged.AddListener(SetSensitivity);
            sensitivityValueText.text = sensitivitySlider.value.ToString("F2");
        }

    }

    void OnDestroy()
    {
        pauseAction.Disable();
    }

    public void TogglePauseMenu()
    {
        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    public void PauseGame()
    {
        isPaused = true;
        pauseMenuPanel.SetActive(true);
        settingsMenuPanel.SetActive(false);
        controlsPanel.SetActive(false);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        isPaused = false;
        pauseMenuPanel.SetActive(false);
        settingsMenuPanel.SetActive(false);
        controlsPanel.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OpenSettings()
    {
        pauseMenuPanel.SetActive(false);
        settingsMenuPanel.SetActive(true);
        controlsPanel.SetActive(false);
    }

    public void BackToPauseMenu()
    {
        settingsMenuPanel.SetActive(false);
        controlsPanel.SetActive(false);
        pauseMenuPanel.SetActive(true);
    }

    public void OpenControls()
    {
        settingsMenuPanel.SetActive(false);
        controlsPanel.SetActive(true);
    }

    public void BackToSettings()
    {
        controlsPanel.SetActive(false);
        settingsMenuPanel.SetActive(true);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Opening_Scene");
    }

    // --- Settings handlers ---
    public void SetVolume(float value)
    {
        Debug.Log("SetVolume called: " + value);
        float dB = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
        masterMixer.SetFloat("MasterVolume", dB);
        if (audioValueText != null)
            audioValueText.text = Mathf.RoundToInt(value * 100).ToString(); // Show as 0–100
    }

    public void SetSensitivity(float value)
    {
        Debug.Log("SetSensitivity called: " + value);
        if (playerCamera != null)
            playerCamera.mouseSensitivity = value;
        if (sensitivityValueText != null)
            sensitivityValueText.text = value.ToString("F2"); // Show with 2 decimals
    }
}