using UnityEngine;
using UnityEngine.UI;

public class ChestWithKey : MonoBehaviour
{
    public GameObject keyPrefab;           // модель ключа (3D объект или Image)
    public Sprite keyIcon;                 // иконка ключа для UI
    public string keyName = "Древний ключ";
    public string doorToOpen = "Выходная дверь"; // имя двери, которую открывает

    private bool isPlayerNear = false;
    private bool isTaken = false;

    void Update()
    {
        if (isPlayerNear && !isTaken && Input.GetKeyDown(KeyCode.E))
        {
            TakeKey();
        }
    }

    void TakeKey()
    {
        isTaken = true;
        
        // Добавляем ключ в инвентарь (через InventoryManager)
        InventoryManager.Instance.AddKey(keyIcon, keyName, doorToOpen);
        
        // Показываем уведомление
        StartCoroutine(ShowKeyNotification());
        
        // Уничтожаем сундук или модель ключа
        Destroy(gameObject);
    }

    System.Collections.IEnumerator ShowKeyNotification()
    {
        GameObject notification = new GameObject("KeyNotification");
        notification.transform.SetParent(GameObject.Find("Canvas").transform);
        
        Text text = notification.AddComponent<Text>();
        text.text = $"Ты получил {keyName}!";
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.color = Color.green;
        text.fontSize = 24;
        text.alignment = TextAnchor.MiddleCenter;
        
        RectTransform rect = notification.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(0, 200);
        rect.sizeDelta = new Vector2(400, 100);
        
        yield return new WaitForSeconds(2f);
        Destroy(notification);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isTaken)
            isPlayerNear = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            isPlayerNear = false;
    }
}