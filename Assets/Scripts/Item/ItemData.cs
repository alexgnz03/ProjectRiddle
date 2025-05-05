using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "New Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite itemSprite;
    public string itemDescription;
}
