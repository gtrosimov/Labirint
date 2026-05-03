using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FearSystem : MonoBehaviour
{
    public Slider fearBar;
    public float maxFear = 100f;
    private float currentFear = 0f;

    public void AddFear(float amount)
    {
        currentFear += amount;
        if (fearBar != null) fearBar.value = currentFear / maxFear;
        if (currentFear >= maxFear)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void ReduceFear(float amount)
    {
        currentFear -= amount;
        if (currentFear < 0) currentFear = 0;
        if (fearBar != null) fearBar.value = currentFear / maxFear;
    }

    private void Start()
    {
        if (fearBar != null) fearBar.value = 0;
    }
}