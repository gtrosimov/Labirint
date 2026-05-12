using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float mouseSensitivity = 2f;
    public float maxLookAngle = 80f;

    public GameObject darkPanel;
    public Light redLight;

    private CharacterController controller;
    private Camera playerCamera;
    private float xRotation = 0f;
    private bool isCaught = false;
    private Vector3 velocity;
    private bool isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();

        // ПРИМЕНЯЕМ СОХРАНЁННУЮ ЧУВСТВИТЕЛЬНОСТЬ
        mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 2f);
        Debug.Log($"Загружена чувствительность мыши: {mouseSensitivity}");

        // ПРИМЕНЯЕМ СОХРАНЁННУЮ ЯРКОСТЬ
        float brightness = PlayerPrefs.GetFloat("Brightness", 1f);
        RenderSettings.ambientLight = new Color(brightness, brightness, brightness);
        Debug.Log($"Загружена яркость: {brightness}");

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (darkPanel != null) darkPanel.SetActive(false);
        if (redLight != null) redLight.enabled = false;
    }

    void Update()
    {
        if (isCaught) return;

        // Мышь
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Движение
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;
        float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        controller.Move(move * speed * Time.deltaTime);

        // Гравитация
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0) velocity.y = -2f;
        velocity.y += Physics.gravity.y * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    public void GetCaught(Vector3 enemyPosition)
    {
        if (isCaught) return;
        isCaught = true;
        this.enabled = false;
        controller.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        StartCoroutine(CatchCutscene(enemyPosition));
    }

    private System.Collections.IEnumerator CatchCutscene(Vector3 enemyPos)
    {
        if (darkPanel != null) darkPanel.SetActive(true);
        if (redLight != null) redLight.enabled = true;

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
                elapsed += Time.deltaTime;
                yield return null;
            }
            transform.rotation = targetRot;
        }

        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}