using UnityEngine;

public class SmartDoor : MonoBehaviour
{
    [Header("Настройки")]
    public GameObject interactionText;
    public float openDuration = 3f;
    public float speed = 2f;

    private bool isPlayerNear = false;
    private bool isOpen = false;
    private Quaternion closedRot;
    private Quaternion openRot;
    private float closeTimer = 0f;
    private Transform player; // ссылка на игрока

    void Start()
    {
        closedRot = transform.rotation;
        if (interactionText != null) interactionText.SetActive(false);
    }

    void Update()
    {
        if (isOpen)
        {
            closeTimer -= Time.deltaTime;
            if (closeTimer <= 0f)
            {
                isOpen = false;
            }
        }

        Quaternion targetRot = isOpen ? openRot : closedRot;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * speed);

        if (isPlayerNear && !isOpen && Input.GetKeyDown(KeyCode.E))
        {
            OpenDoorForPlayer();
        }
    }

    void OpenDoorForPlayer()
    {
        Camera playerCamera = player.GetComponentInChildren<Camera>();
        if (playerCamera == null) return;

        Vector3 playerDir = transform.InverseTransformPoint(playerCamera.transform.position);
        float angle = playerDir.z > 0 ? -90f : 90f;
        openRot = closedRot * Quaternion.Euler(0, angle, 0);

        isOpen = true;
        closeTimer = openDuration;
        if (interactionText != null) interactionText.SetActive(false);
    }

    public void OpenForEnemy()
    {
        if (isOpen) return;

        Vector3 enemyDir = transform.InverseTransformPoint(transform.position);
        float angle = enemyDir.z > 0 ? -90f : 90f;
        openRot = closedRot * Quaternion.Euler(0, angle, 0);

        isOpen = true;
        closeTimer = openDuration;
        if (interactionText != null) interactionText.SetActive(false);
    }

    // Объединённый OnTriggerEnter для игрока и врага
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isOpen)
        {
            player = other.transform;
            isPlayerNear = true;
            if (interactionText != null) interactionText.SetActive(true);
        }
        else if (other.CompareTag("Enemy") && !isOpen)
        {
            EnemyAI_Smart enemy = other.GetComponent<EnemyAI_Smart>();
            if (enemy != null) enemy.OpenDoor(gameObject);
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