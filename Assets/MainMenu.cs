using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Кнопка PLAY — ведёт на обучение
    public void PlayGame()
    {
        SceneManager.LoadScene("Level1");
    }

    // Кнопка SETTINGS — пока просто заглушка
    public void OpenSettings()
    {
        Debug.Log("Настройки откроются позже");
        // Здесь позже добавишь панель настроек
    }

    // Кнопка QUIT — выход из игры
    public void QuitGame()
    {
        Debug.Log("Выход из игры");
        Application.Quit();
        
        // Чтобы кнопка работала в редакторе (останавливала плей-режим)
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}