using UnityEngine;

public class Chest : MonoBehaviour
{
    [Header("Награда")]
    public KeyData keyToGive;

    private bool isOpened = false;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter сработал с объектом: " + other.name); // для отладки
    }

    private void OnTriggerStay(Collider other)
    {
        if (isOpened) return;
        if (!other.CompareTag("Player")) return;

        Debug.Log("Игрок рядом с сундуком! Нажми E"); // должно появляться

        if (Input.GetKeyDown(KeyCode.E))
        {
            OpenChest();
        }
    }

    private void OpenChest()
    {
        isOpened = true;
        Debug.Log("Сундук открыт! Ключ получен.");

        if (keyToGive != null && InventoryUI.Instance != null)
        {
            InventoryUI.Instance.CollectKey(keyToGive);
        }
    }
}