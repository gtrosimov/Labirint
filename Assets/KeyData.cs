using UnityEngine;

[CreateAssetMenu(fileName = "NewKey", menuName = "Inventory/Key")]
public class KeyData : ScriptableObject
{
    public string keyName = "Ключ от выхода";
    public string keyDescription = "Открывает большую дверь на выход из лабиринта";
    public Sprite icon;
}