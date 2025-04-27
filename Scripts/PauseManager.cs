using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

// This script manages the pause functionality in the game, allowing the game to be paused and resumed using Time.timeScale.
// It also handles the settings menu, where players can adjust the volume and brightness. The script uses a singleton 
// pattern to ensure that only one instance of the pause manager exists across scenes. It controls the display of 
// UI elements like the pause menu, settings buttons, sliders, and overlays, and ensures the game's state is properly 
// maintained during pauses, including cursor visibility and locking.

public class PauseManager : MonoBehaviour
{
    //Singleton for pause manager. Ensures that only one pause manager exists
    //(Should be automatically set to null if stored singleton is destroyed)
    public static PauseManager instance;

    public static bool IsGamePaused = false;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI pausedText;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private TextMeshProUGUI volumeText;
    [SerializeField] private Slider brightnessSlider;
    [SerializeField] private TextMeshProUGUI brightnessText;
    [SerializeField] private Button backButton;
    [SerializeField] private Image brightnessOverlay; // Dark overlay for brightness
    [SerializeField] private Light directionalLight;

    // private PlayerControls controls;

    private void Awake()
    {
        //Set up singleton
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else if (instance != this)
        {
            //Move existing PC to this position, then delete this duplicate
            Destroy(this.gameObject);
        }

        // controls = new PlayerControls();
    }

    private void Start()
    {
        // controls.Controls.Pause.performed += ctx => TogglePause();
        ResumeGame(); // Ensure game starts unpaused
        InitializeUI();
    }

    // OnDestroy, clear the singleton
    void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;        
        }
    }

    private void InitializeUI()
    {
        pausedText.gameObject.SetActive(false);
        settingsButton.gameObject.SetActive(false);
        volumeSlider.gameObject.SetActive(false);
        volumeText.gameObject.SetActive(false);
        brightnessSlider.gameObject.SetActive(false);
        brightnessText.gameObject.SetActive(false);
        backButton.gameObject.SetActive(false);

        brightnessOverlay.color = new Color(0, 0, 0, 0); // Fully transparent overlay
        brightnessSlider.value = 1f;

        volumeSlider.value = AudioListener.volume;
        volumeSlider.onValueChanged.AddListener(OnVolumeSliderChanged);
        brightnessSlider.onValueChanged.AddListener(OnBrightnessSliderChanged);
    }

    // private void OnEnable()
    // {
    //    if (controls == null) return; // Prevent null error
    // controls?.Enable();
    // }

    // private void OnDisable()
    // {
    //     if (controls == null) return; // Prevent null error
    // controls?.Disable();
    // }

    public void TogglePause()
    {
        IsGamePaused = !IsGamePaused;

        if (IsGamePaused)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }

    // Pause the game using Time.timeScale
    private void PauseGame()
    {
        Time.timeScale = 0f;
        pausedText.gameObject.SetActive(true);
        settingsButton.gameObject.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("Game Paused");
    }

    // Resume the game using Time.timeScale
    private void ResumeGame()
    {
        Time.timeScale = 1f;
        pausedText.gameObject.SetActive(false);
        settingsButton.gameObject.SetActive(false);
        volumeSlider.gameObject.SetActive(false);
        volumeText.gameObject.SetActive(false);
        brightnessSlider.gameObject.SetActive(false);
        brightnessText.gameObject.SetActive(false);
        backButton.gameObject.SetActive(false);

        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;

        Debug.Log("Game Resumed");
    }

    // **Settings Menu Functions**
    public void OnSettingsButtonClick()
    {
        volumeSlider.gameObject.SetActive(true);
        volumeText.gameObject.SetActive(true);
        brightnessSlider.gameObject.SetActive(true);
        brightnessText.gameObject.SetActive(true);
        backButton.gameObject.SetActive(true);
        settingsButton.gameObject.SetActive(false);
    }

    public void OnBackButtonClick()
    {
        volumeSlider.gameObject.SetActive(false);
        volumeText.gameObject.SetActive(false);
        brightnessSlider.gameObject.SetActive(false);
        brightnessText.gameObject.SetActive(false);
        backButton.gameObject.SetActive(false);
        settingsButton.gameObject.SetActive(true);
    }

    // **Volume and Brightness Adjustments**
    public void OnVolumeSliderChanged(float value)
    {
        AudioListener.volume = value;
    }

    public void OnBrightnessSliderChanged(float value)
    {
        Color overlayColor = new Color(0, 0, 0, 1 - value);
        brightnessOverlay.color = overlayColor;
    }
}
