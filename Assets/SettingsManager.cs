using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour
{
    [Header("Аудио")]
    public AudioMixer audioMixer;
    public Slider volumeSlider;

    [Header("Графика")]
    public Slider brightnessSlider;

    [Header("Управление")]
    public Slider mouseSensitivitySlider;

    [Header("Игра")]
    public Dropdown difficultyDropdown;
    public Toggle fullscreenToggle;

    private void Start()
    {
        LoadSettings();
    }

    // Громкость
    public void SetVolume(float value)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat("MasterVolume", value);
    }

    // Яркость
    public void SetBrightness(float value)
    {
        PlayerPrefs.SetFloat("Brightness", value);
    }

    // Чувствительность мыши
    public void SetMouseSensitivity(float value)
    {
        PlayerPrefs.SetFloat("MouseSensitivity", value);
    }

    // Сложность
    public void SetDifficulty(int index)
    {
        PlayerPrefs.SetInt("Difficulty", index);
    }

    // Полноэкранный режим
    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
    }

    private void LoadSettings()
    {
        if (volumeSlider) volumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 0.8f);
        if (brightnessSlider) brightnessSlider.value = PlayerPrefs.GetFloat("Brightness", 1f);
        if (mouseSensitivitySlider) mouseSensitivitySlider.value = PlayerPrefs.GetFloat("MouseSensitivity", 2f);
        if (difficultyDropdown) difficultyDropdown.value = PlayerPrefs.GetInt("Difficulty", 1);
        if (fullscreenToggle) fullscreenToggle.isOn = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
    }
}