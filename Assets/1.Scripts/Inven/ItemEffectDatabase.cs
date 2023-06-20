using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemEffect
{
    public string itemName; //아이템의 이름(키값)
    public int num; //수치
}

public class ItemEffectDatabase : MonoBehaviour
{

    
    public ItemEffect itemEffect;
    public PlayerHealth healthBase;

    public void UseItem(Item _item)
    {
        if (_item.itemType == Item.ItemType.Equipment)
        {
            //장착
            Debug.Log(_item.itemName+"을 장착했습니다");
        }

        else if (_item.itemType == Item.ItemType.Used)
        {
            Debug.Log(itemEffect.num);
            healthBase.RecoveryHp(itemEffect.num);
            Debug.Log(_item.itemName + "을 사용했습니다");
            //for (int x = 0; x < itemEffects.Length; x++)
            //{
            //    playerHealth.RecoveryHp(30);
            //}
            //return;
        }
    }
}
