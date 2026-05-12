using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    [Header("Загрузка уровня")]
    public string sceneToLoad = "Level1";

    [Header("Экран загрузки")]
    public GameObject loadingScreen;
    public Slider loadingSlider;
    public Text loadingText;

    [Header("Меню настроек")]
    public GameObject settingsPanel;

    private void Start()
    {
        // Выключаем экран загрузки и панель настроек при старте
        if (loadingScreen != null)
            loadingScreen.SetActive(false);

        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    public void PlayGame()
    {
        StartCoroutine(LoadSceneAsync());
    }

    private IEnumerator LoadSceneAsync()
    {
        if (loadingScreen != null)
            loadingScreen.SetActive(true);

        if (loadingSlider != null) loadingSlider.value = 0f;

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToLoad);
        operation.allowSceneActivation = false;

        float timer = 0f;

        while (!operation.isDone)
        {
            timer += Time.deltaTime;
            float realProgress = operation.progress / 0.9f;

            if (loadingSlider != null)
            {
                loadingSlider.value = Mathf.Lerp(loadingSlider.value, realProgress, Time.deltaTime * 5f);
            }

            if (loadingText != null)
            {
                int percent = Mathf.RoundToInt(loadingSlider.value * 100);
                loadingText.text = $"ЗАГРУЗКА... {percent}%";
            }

            if (operation.progress >= 0.9f && timer >= 3f)
            {
                if (loadingText != null)
                    loadingText.text = "ГОТОВО...";

                while (loadingSlider != null && loadingSlider.value < 0.99f)
                {
                    loadingSlider.value = Mathf.Lerp(loadingSlider.value, 1f, Time.deltaTime * 4f);
                    yield return null;
                }

                yield return new WaitForSeconds(0.7f);
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    public void OpenSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    public void QuitGame()
    {
        Debug.Log("Выход из игры");
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}