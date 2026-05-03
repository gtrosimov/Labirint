using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Движение")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float mouseSensitivity = 2f;
    public float maxLookAngle = 80f;

    [Header("Кат-сцена при поимке")]
    public GameObject darkPanel;
    public Light redLight;
    public float catchDuration = 4f;           // сколько длится кат-сцена
    public float cameraTurnSpeed = 3f;

    private CharacterController controller;
    private Camera playerCamera;
    private float xRotation = 0f;
    private bool isCaught = false;

    private Vector3 originalCameraPos;
    private Quaternion originalCameraRot;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (darkPanel != null) darkPanel.SetActive(false);
        if (redLight != null) redLight.enabled = false;
    }

    void Update()
    {
        if (isCaught) return;

        HandleMouseLook();
        HandleMovement();
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    void HandleMovement()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;
        float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

        controller.Move(move * speed * Time.deltaTime);

        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0) velocity.y = -2f;
        velocity.y += Physics.gravity.y * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private bool isGrounded;
    private Vector3 velocity;

    // ====================== КАТ-СЦЕНА ======================
    public void GetCaught(Vector3 enemyPosition)
    {
        if (isCaught) return;
        isCaught = true;

        // Сохраняем оригинальное положение камеры
        originalCameraPos = playerCamera.transform.position;
        originalCameraRot = playerCamera.transform.rotation;

        // Полностью отключаем управление
        this.enabled = false;
        controller.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Запускаем кат-сцену
        StartCoroutine(CatchCutscene(enemyPosition));
    }

    private IEnumerator CatchCutscene(Vector3 enemyPos)
    {
        // Включаем эффекты
        if (darkPanel != null) darkPanel.SetActive(true);
        if (redLight != null) redLight.enabled = true;

        // Плавный поворот игрока и камеры к врагу
        float elapsed = 0f;
        float duration = 1.2f;
        Vector3 direction = (enemyPos - transform.position).normalized;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(direction);

            while (elapsed < duration)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, elapsed / duration * 1.8f);
                playerCamera.transform.localRotation = Quaternion.Slerp(playerCamera.transform.localRotation, 
                    Quaternion.Euler(0, 0, 0), elapsed / duration * 1.5f);
                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.rotation = targetRot;
        }

        // Даём время на анимацию атаки врага
        yield return new WaitForSeconds(1.8f);
        // Можно добавить лёгкую тряску камеры здесь, если хочешь

        // Перезагрузка уровня
        yield return new WaitForSeconds(0.8f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Опционально: сброс если нужно
    private void OnDisable()
    {
        if (darkPanel != null) darkPanel.SetActive(false);
    }
}