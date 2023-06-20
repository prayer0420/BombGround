using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    public class Inventory : MonoBehaviour
    {

        public static bool inventoryActivated = false;  //인벤토리 창이 활성화 되어있는지

        //필요한 컴퍼넌트
        [SerializeField]
        private GameObject go_InventoryBase;

        [SerializeField]
        private GameObject go_SlotParent;

        private Slot[] slots;
        void Start()
        {
            slots = go_SlotParent.GetComponentsInChildren<Slot>(); //slot들을 담아놓음    
        }

        // Update is called once per frame
        void Update()
        {
            TryOpenInventory();
        }

        private void TryOpenInventory()
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                inventoryActivated = !inventoryActivated;

                if (inventoryActivated)
                {
                   OpenInventory();
                   MoveBehaviour.CanMove = false;
                }
                else
                {
                   ClosInventory();
                   MoveBehaviour.CanMove = true;
                }
            }
        }

        private void OpenInventory()
        {
            go_InventoryBase.SetActive(true);
        }

        private void ClosInventory()
        {
            go_InventoryBase.SetActive(false);
        }


        public void Acquired(Item _item, int _count = 1)
        {
            //장비 타입이 아닐경우에만!
            if (Item.ItemType.Equipment != _item.itemType)
            {
                for (int i = 0; i < slots.Length; i++)
                {
                    if (slots[i].item != null)
                    {
                        if (slots[i].item.itemName == _item.itemName)
                        {
                            slots[i].SetSlotCount(_count);
                            return;
                        }
                    }
                }
            }

            //장비타입이고, 아이템이 없을 때
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].item == null)
                {
                    slots[i].AddItem(_item, _count);
                    return;
                }
            }


        }
    }
