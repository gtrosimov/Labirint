using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    
    public GameObject keyUIPrefab;  // префаб с Image + Text
    public Transform keyPanel;      // панель в левом верхнем углу
    
    private List<KeyItem> keys = new List<KeyItem>();
    
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    public void AddKey(Sprite icon, string name, string opensDoor)
    {
        keys.Add(new KeyItem(icon, name, opensDoor));
        
        // Создаём UI элемент ключа
        GameObject keyUI = Instantiate(keyUIPrefab, keyPanel);
        keyUI.GetComponent<Image>().sprite = icon;
        keyUI.GetComponentInChildren<Text>().text = name;
    }
    
    public bool HasKeyForDoor(string doorName)
    {
        foreach (var key in keys)
            if (key.opensDoor == doorName) return true;
        return false;
    }
    
    public void UseKeyForDoor(string doorName)
    {
        foreach (var key in keys)
        {
            if (key.opensDoor == doorName)
            {
                keys.Remove(key);
                // Удалить UI элемент
                break;
            }
        }
    }
    
    private class KeyItem
    {
        public Sprite icon;
        public string name;
        public string opensDoor;
        
        public KeyItem(Sprite icon, string name, string opensDoor)
        {
            this.icon = icon;
            this.name = name;
            this.opensDoor = opensDoor;
        }
    }
}