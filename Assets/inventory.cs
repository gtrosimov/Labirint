using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance;

    [Header("Большое окно")]
    public GameObject keyLargePanel;

    [Header("Маленькая иконка")]
    public GameObject keySmallPanel;

    private void Awake()
    {
        Instance = this;
        Debug.Log("InventoryUI запущен");
    }

    public void CollectKey(KeyData keyData)
    {
        Debug.Log("CollectKey ВЫЗВАН!");

        if (keyLargePanel != null)
        {
            keyLargePanel.SetActive(true);
            Debug.Log("Большая панель ACTIVATED");
        }

        if (keySmallPanel != null)
        {
            keySmallPanel.SetActive(true);
            Debug.Log("Маленькая иконка ACTIVATED");
        }
    }

    public bool HasKey() => true;
}