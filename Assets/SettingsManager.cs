using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour
{
    [Header("Audio")]
    public AudioMixer audioMixer;
    public Slider volumeSlider;
    [Header("Диапазон громкости")]
    public float volumeMin = 0.0001f;
    public float volumeMax = 1f;

    [Header("Графика")]
    public Slider brightnessSlider;
    [Header("Диапазон яркости")]
    public float brightnessMin = 0f;
    public float brightnessMax = 2f;

    [Header("Управление")]
    public Slider mouseSlider;
    [Header("Диапазон мыши")]
    public float mouseMin = 0.5f;
    public float mouseMax = 5f;

    [Header("Экран")]
    public Toggle fullscreenToggle;

    private void Start()
    {
        // Настройка ползунков
        if (volumeSlider != null)
        {
            volumeSlider.minValue = volumeMin;
            volumeSlider.maxValue = volumeMax;
            volumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 0.5f);
            volumeSlider.onValueChanged.AddListener(SetVolume);
            SetVolume(volumeSlider.value);
        }

        if (brightnessSlider != null)
        {
            brightnessSlider.minValue = brightnessMin;
            brightnessSlider.maxValue = brightnessMax;
            brightnessSlider.value = PlayerPrefs.GetFloat("Brightness", 1f);
            brightnessSlider.onValueChanged.AddListener(SetBrightness);
            SetBrightness(brightnessSlider.value);
        }

        if (mouseSlider != null)
        {
            mouseSlider.minValue = mouseMin;
            mouseSlider.maxValue = mouseMax;
            mouseSlider.value = PlayerPrefs.GetFloat("MouseSensitivity", 2f);
            mouseSlider.onValueChanged.AddListener(SetMouseSensitivity);
        }

        if (fullscreenToggle != null)
        {
            fullscreenToggle.isOn = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
            fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
            SetFullscreen(fullscreenToggle.isOn);
        }
    }

    public void SetVolume(float value)
    {
        PlayerPrefs.SetFloat("MasterVolume", value);
        if (audioMixer != null)
            audioMixer.SetFloat("MasterVolume", Mathf.Log10(value) * 20);
        Debug.Log($"Громкость: {value}");
    }

    public void SetBrightness(float value)
    {
        PlayerPrefs.SetFloat("Brightness", value);
        Debug.Log($"Яркость сохранена: {value}");
    }

    public void SetMouseSensitivity(float value)
    {
        PlayerPrefs.SetFloat("MouseSensitivity", value);
        Debug.Log($"Чувствительность мыши сохранена: {value}");
    }

    public void SetFullscreen(bool isFull)
    {
        Screen.fullScreen = isFull;
        PlayerPrefs.SetInt("Fullscreen", isFull ? 1 : 0);
    }

    public void CloseSettings()
    {
        gameObject.SetActive(false);
    }
}