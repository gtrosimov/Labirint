using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour
{
    [Header("Компоненты")]
    public Slider volumeSlider;
    public Slider brightnessSlider;
    public Slider mouseSensitivitySlider;
    public Toggle crazyModeToggle;
    public Toggle fullscreenToggle;

    [Header("Audio")]
    public AudioMixer audioMixer;

    // Значения по умолчанию
    private float defaultVolume = 0.7f;      // 70%
    private float defaultBrightness = 1f;    // нормальная яркость
    private float defaultMouseSensitivity = 2f;

    private void Start()
    {
        LoadSettings();
    }

    public void SetVolume(float value)
    {
        if (audioMixer != null)
            audioMixer.SetFloat("MasterVolume", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat("MasterVolume", value);
        Debug.Log($"Громкость: {value}");
    }

    public void SetBrightness(float value)
    {
        PlayerPrefs.SetFloat("Brightness", value);
        ApplyBrightness(value);
        Debug.Log($"Яркость: {value}");
    }

    private void ApplyBrightness(float value)
    {
        RenderSettings.ambientLight = new Color(value, value, value);
    }

    public void SetMouseSensitivity(float value)
    {
        if (value < 0.5f) value = 0.5f; // защита от нуля
        PlayerPrefs.SetFloat("MouseSensitivity", value);
        Debug.Log($"Чувствительность мыши: {value}");
        
        // Обновляем у игрока, если он в сцене
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerController pc = player.GetComponent<PlayerController>();
            if (pc != null)
                pc.mouseSensitivity = value;
        }
    }

    public void SetCrazyMode(bool isEnabled)
    {
        PlayerPrefs.SetInt("CrazyMode", isEnabled ? 1 : 0);
        Debug.Log(isEnabled ? "Безумная сложность включена" : "Безумная сложность выключена");
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
        Debug.Log($"Полноэкранный режим: {isFullscreen}");
    }

    public void CloseSettings()
    {
        gameObject.SetActive(false);
    }

    private void LoadSettings()
    {
        // Загружаем или ставим значения по умолчанию
        float savedVolume = PlayerPrefs.GetFloat("MasterVolume", defaultVolume);
        float savedBrightness = PlayerPrefs.GetFloat("Brightness", defaultBrightness);
        float savedMouseSens = PlayerPrefs.GetFloat("MouseSensitivity", defaultMouseSensitivity);

        // Применяем к слайдерам
        if (volumeSlider != null)
            volumeSlider.value = savedVolume;
        
        if (brightnessSlider != null)
            brightnessSlider.value = savedBrightness;
        
        if (mouseSensitivitySlider != null)
            mouseSensitivitySlider.value = savedMouseSens;

        // Загружаем Toggle
        if (crazyModeToggle != null)
            crazyModeToggle.isOn = PlayerPrefs.GetInt("CrazyMode", 0) == 1;

        if (fullscreenToggle != null)
            fullscreenToggle.isOn = PlayerPrefs.GetInt("Fullscreen", 1) == 1;

        // Применяем настройки сразу
        SetVolume(savedVolume);
        SetBrightness(savedBrightness);
        SetMouseSensitivity(savedMouseSens);
    }
}