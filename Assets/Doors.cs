using UnityEngine;

public class SmartDoor : MonoBehaviour
{
    public GameObject interactionText;
    public float openDuration = 3f;
    public float speed = 2f;

    private bool isPlayerNear = false;
    private bool isOpen = false;
    private Quaternion closedRot;
    private Quaternion openRot;
    private float closeTimer = 0f;
    private Transform player;

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
            if (closeTimer <= 0f) isOpen = false;
        }

        Quaternion target = isOpen ? openRot : closedRot;
        transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * speed);

        if (isPlayerNear && !isOpen && Input.GetKeyDown(KeyCode.E))
            OpenForPlayer();
    }

    void OpenForPlayer()
    {
        if (player == null) return;
        Vector3 dir = transform.InverseTransformPoint(player.position + Vector3.up * 0.5f);
        float angle = dir.z > 0 ? -90f : 90f;
        openRot = closedRot * Quaternion.Euler(0, angle, 0);
        isOpen = true;
        closeTimer = openDuration;
        if (interactionText != null) interactionText.SetActive(false);
    }

    // --- ЭТОТ МЕТОД ВЫЗЫВАЕТСЯ ИЗ СКРИПТА ВРАГА ---
    public void OpenForEnemy()
    {
        // Враг всегда открывает дверь от себя
        float angle = -90f;
        openRot = closedRot * Quaternion.Euler(0, angle, 0);
        isOpen = true;
        closeTimer = openDuration;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isOpen)
        {
            player = other.transform;
            isPlayerNear = true;
            if (interactionText != null) interactionText.SetActive(true);
        }
        // --- ВОТ ТУТ ВРАГ ПОДХОДИТ К ДВЕРИ ---
        else if (other.CompareTag("Enemy") && !isOpen)
        {
            EnemyAI enemy = other.GetComponent<EnemyAI>();
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