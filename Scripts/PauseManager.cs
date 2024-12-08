using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;


public class PauseManager : MonoBehaviour
{
    public static bool IsGamePaused = false;

    public TextMeshProUGUI pausedText;
    public Button settingsButton;
    public Slider volumeSlider;
    public TextMeshProUGUI volumeText;
    public Slider brightnessSlider;
    public TextMeshProUGUI brightnessText;
    public Button backButton;
    public Light directionalLight;
    private PlayerControls controls;

    private void Awake()
    {
        controls = new PlayerControls();
    }

    private void Start()
    {
        Time.timeScale = 1f;
        PauseManager.IsGamePaused = false;
        pausedText.gameObject.SetActive(false);
        settingsButton.gameObject.SetActive(false);
        volumeSlider.gameObject.SetActive(false);
        volumeText.gameObject.SetActive(false);
        brightnessSlider.gameObject.SetActive(false);
        brightnessText.gameObject.SetActive(false);
        backButton.gameObject.SetActive(false);

        // Initialize brightness overlay to be fully transparent
        brightnessOverlay.color = new Color(0, 0, 0, 0);
        brightnessSlider.value = 1f;

        volumeSlider.value = AudioListener.volume;
        volumeSlider.onValueChanged.AddListener(OnVolumeSliderChanged);

        brightnessSlider.onValueChanged.AddListener(OnBrightnessSliderChanged);
    }

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

    public void OnVolumeSliderChanged(float value)
    {
        AudioListener.volume = value;
    }

    [SerializeField] private UnityEngine.UI.Image brightnessOverlay; // Add dark overlay image

    public void OnBrightnessSliderChanged(float value)
    {
        Color overlayColor = new Color(0, 0, 0, 1 - value);
        brightnessOverlay.color = overlayColor;
    }




    // private void OnEnable()
    // {
    //     controls.Enable();
    //     controls.Controls.Pause.performed += _ => TogglePause();
    // }

    // private void OnDisable()
    // {
    //     controls.Controls.Pause.performed -= _ => TogglePause();
    //     controls.Disable();
    // }

    public void TogglePause(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Started) return;

        IsGamePaused = !IsGamePaused;
        Time.timeScale = IsGamePaused ? 0f : 1f;
        pausedText.gameObject.SetActive(IsGamePaused);
        settingsButton.gameObject.SetActive(IsGamePaused);
        
        if (!IsGamePaused)
        {
            volumeSlider.gameObject.SetActive(false);
            volumeText.gameObject.SetActive(false);
            brightnessSlider.gameObject.SetActive(false);
            brightnessText.gameObject.SetActive(false);
            backButton.gameObject.SetActive(false);
        }
        
        Debug.Log($"Pause Toggled: isPaused = {IsGamePaused}, TimeScale = {Time.timeScale}");
    }


}
