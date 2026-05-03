using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("Настройки двери")]
    public bool isLocked = true;      // закрыта ли дверь сначала
    public GameObject doorPanel;      // 3D-модель двери (или часть, которая двигается)
    public Vector3 openPosition = new Vector3(0, 2, 0); // смещение при открытии (например, вверх или в сторону)
    public float openSpeed = 2f;

    private Vector3 closedPos;
    private bool isOpen = false;
    private bool isPlayerNear = false;

    [Header("UI подсказка")]
    public GameObject promptUI;       // например, текст "Нажми E, чтобы открыть"

    void Start()
    {
        if (doorPanel == null) doorPanel = gameObject;
        closedPos = doorPanel.transform.localPosition;
        if (promptUI != null) promptUI.SetActive(false);
    }

    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E) && !isOpen)
        {
            if (!isLocked)
                OpenDoor();
            else
                Debug.Log("Дверь заперта");
        }

        // Плавное движение двери
        if (isOpen)
        {
            doorPanel.transform.localPosition = Vector3.Lerp(doorPanel.transform.localPosition, closedPos + openPosition, Time.deltaTime * openSpeed);
        }
    }

    void OpenDoor()
    {
        isOpen = true;
        GetComponent<Collider>().enabled = false; // отключаем коллайдер, чтобы игрок мог пройти
        if (promptUI != null) promptUI.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isOpen)
        {
            isPlayerNear = true;
            if (!isLocked && promptUI != null) promptUI.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            if (promptUI != null) promptUI.SetActive(false);
        }
    }

    // Метод для вызова из скрипта ключа (если дверь заперта)
    public void Unlock()
    {
        isLocked = false;
        Debug.Log("Дверь разблокирована!");
        if (isPlayerNear && promptUI != null) promptUI.SetActive(true);
    }
}
