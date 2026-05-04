using UnityEngine;
using UnityEngine.UI;

public class DoorWithE : MonoBehaviour
{
    [Header("Настройки")]
    public GameObject interactionText; // UI текст "Нажми E", перетащить из Canvas
    public Color openColor = Color.green;

    private Collider blockCollider;    // коллайдер, блокирующий проход
    private bool isPlayerNear = false;
    private bool isOpen = false;

    void Start()
    {
        // Находим коллайдер, который блокирует проход (не триггер)
        blockCollider = GetComponent<Collider>();
        if (interactionText != null) interactionText.SetActive(false);
    }

    void Update()
    {
        if (!isOpen && isPlayerNear && Input.GetKeyDown(KeyCode.E))
        {
            OpenDoor();
        }
    }

    void OpenDoor()
    {
        isOpen = true;
        // Отключаем коллайдер, который мешал проходу
        if (blockCollider != null) blockCollider.enabled = false;
        // Меняем цвет материала (или можно отключить рендер)
        Renderer rend = GetComponent<Renderer>();
        if (rend != null) rend.material.color = openColor;
        // Скрываем подсказку
        if (interactionText != null) interactionText.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isOpen)
        {
            isPlayerNear = true;
            if (interactionText != null) interactionText.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            if (interactionText != null) interactionText.SetActive(false);
        }
    }
}