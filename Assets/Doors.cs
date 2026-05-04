using UnityEngine;

public class SmartDoor : MonoBehaviour
{
    [Header("Настройки")]
    public GameObject interactionText;
    public float openDuration = 3f;      // через сколько секунд закроется
    public float speed = 2f;             // скорость вращения

    private bool isPlayerNear = false;
    private bool isOpen = false;
    private Quaternion closedRot;
    private Quaternion openRot;
    private float closeTimer = 0f;

    void Start()
    {
        closedRot = transform.rotation;
        if (interactionText != null) interactionText.SetActive(false);
    }

    void Update()
    {
        // Ожидание закрытия
        if (isOpen)
        {
            closeTimer -= Time.deltaTime;
            if (closeTimer <= 0f)
            {
                isOpen = false;
            }
        }

        // Плавный поворот
        Quaternion targetRot = isOpen ? openRot : closedRot;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * speed);

        // Нажатие E
        if (isPlayerNear && !isOpen && Input.GetKeyDown(KeyCode.E))
        {
            OpenDoor();
        }
    }

    void OpenDoor()
    {
        // Определяем сторону открытия (от себя или на себя)
        Vector3 playerDir = transform.InverseTransformPoint(Camera.main.transform.position);
        float angle = playerDir.z > 0 ? -90f : 90f;
        openRot = closedRot * Quaternion.Euler(0, angle, 0);

        isOpen = true;
        closeTimer = openDuration;

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