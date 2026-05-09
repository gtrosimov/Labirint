using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    [Header("Сцена для загрузки")]
    public string sceneToLoad = "Level1";

    [Header("Экран загрузки")]
    public GameObject loadingScreen;
    public Slider loadingSlider;
    public Text loadingText;

    private void Start()
    {
        if (loadingScreen != null)
            loadingScreen.SetActive(false);
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

            // Реальный прогресс загрузки
            float realProgress = operation.progress / 0.9f;

            // Плавное заполнение с задержкой (чтобы дольше выглядело)
            if (loadingSlider != null)
            {
                float smoothProgress = Mathf.Lerp(loadingSlider.value, realProgress, Time.deltaTime * 3f);
                loadingSlider.value = smoothProgress;
            }

            if (loadingText != null)
            {
                int percent = Mathf.RoundToInt(loadingSlider.value * 100);
                loadingText.text = $"ЗАГРУЗКА... {percent}%";
            }

            // Ждём минимум 3.5 секунды + завершение загрузки
            if (operation.progress >= 0.9f && timer >= 3.5f)
            {
                if (loadingText != null)
                    loadingText.text = "ГОТОВО...";

                // Плавно добиваем до конца
                while (loadingSlider != null && loadingSlider.value < 0.99f)
                {
                    loadingSlider.value = Mathf.Lerp(loadingSlider.value, 1f, Time.deltaTime * 3f);
                    yield return null;
                }

                yield return new WaitForSeconds(0.8f);
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    public void QuitGame()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}