using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Item", menuName = "New Item/item")]
public class Item : ScriptableObject
{
    public string itemName; //아이템의 이름
    public Sprite itemImage; //아이템의 이미지
    public GameObject itemPrefab; //아이템의 프리팹
    public ItemType itemType; //아이템의 유형.
    public WeaponType weaponType; //무기 유형

    public enum WeaponType
    {
        NONE,
        SHORT,
        LONG,
    }

    public enum ItemType
    {
        Equipment,
        Consumables,
        Used,
        ETC,
    }
    
    void Start()
    {
        itemType = ItemType.Equipment;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
