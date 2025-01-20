using UnityEngine;

public enum SlotTag
{
    None, Armour, Weapon, Accessory
}

[CreateAssetMenu(menuName = "Scriptable Objects/Item")]
public class Item : ScriptableObject
{
    public Sprite sprite;
    public SlotTag itemTag;
    public string itemName = "Unique Placeholder";
    public string itemDescription = "There once was a lonely potato, who lived in Ireland!";

    [Header("If it can be equipped")]
    public GameObject equipmentPrefab;
}
