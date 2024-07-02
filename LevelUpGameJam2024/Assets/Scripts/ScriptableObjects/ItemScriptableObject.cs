using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item001", menuName = "ScriptableObjects/ItemScriptableObject", order = 0)]
public class ItemScriptableObject : ScriptableObject
{
    [SerializeField] private int id;
    public int Id
    {
        get { return id;}
        set { id = value; }
    }

    [SerializeField] private string itemName;
    public string ItemName
    {
        get { return itemName; }
        set { itemName = value; }
    }

    [SerializeField] private string itemDef;
    public string ItemDef
    {
        get { return itemDef; }
        set { itemDef = value; }
    }

    [SerializeField] private Sprite sprite;
    public Sprite Sprite
    {
        get { return sprite; }
        set { sprite = value; }
    }

    public enum Type{ Weapon, Armour, Object, Resource}
    [SerializeField] private Type itemType;
    public Type ItemType
    {
        get { return itemType; }
        set { itemType = value; }
    }

    [SerializeField] private int stat;
    public int Stat
    {
        get { return stat; }
        set { stat = value; }
    }

    public ItemScriptableObject(int id, string itemName, string itemDef, Sprite sprite, Type itemType, int stat)
    {
        this.Id = id;
        this.itemName = itemName;
        this.itemDef = itemDef;
        this.sprite = sprite;
        this.itemType = itemType;
        this.stat = stat;
    }

}
