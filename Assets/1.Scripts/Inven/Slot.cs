using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{

    public InteractiveWeapon item; //획득한 아이템
    public int itemCount; //획득한 아이템 개수'
    public Image itemImage; //아이템의 이미지


    //필요한 컴포넌트
    [SerializeField]
    private Text text_Count; //아이템 개수 텍스트
    [SerializeField]
    private GameObject go_CountImage; //아이템 개수 이미지가 들어갈 원(파란색)
    private Rect baseRect;

    public GameObject player;
    private ShootBehaviour shootBehaviour;
    private InputNumber inputNumber;

    private ItemEffectDatabase itemEffectDatabase;


    void Start()
    {
        itemEffectDatabase = FindObjectOfType<ItemEffectDatabase>();
        shootBehaviour = FindObjectOfType<ShootBehaviour>();
        baseRect =  transform.parent.parent.GetComponent<RectTransform>().rect; //인벤토리 창의 rectTransform정보를 받아옴
        inputNumber = FindObjectOfType<InputNumber>();

    }


    //평상시엔 알파값0이였다가 이미지가 들어오면 250으로 설정
    private void SetColor(float _alpha)
    {
        Color color = itemImage.color;
        color.a = _alpha;
        itemImage.color = color;
    }

    public void AddItem(InteractiveWeapon weapon, int _count = 1)
    {
        Debug.Log("슬롯에 넣기");
        item = weapon;
        itemCount = _count;
        itemImage.sprite = item.itemImage;

        if (weapon.itemType != InteractiveWeapon.ItemType.Equipment)
        {
            go_CountImage.SetActive(true);
            text_Count.text = itemCount.ToString();
        }
        else
        {
            text_Count.text = "0";
            go_CountImage.SetActive(false);
        }
        SetColor(1);
    }

    //public void AddItem2(Item _item, int _count = 1)
    //{
    //    item2 = _item;
    //    itemCount2 = _count;
    //    itemImage.sprite = item2.itemImage;

    //    if (item2.itemType != Item.ItemType.Equipment)
    //    {
    //        go_CountImage.SetActive(true);
    //        text_Count.text = itemCount.ToString();
    //    }
    //    else
    //    {
    //        text_Count.text = "0";
    //        go_CountImage.SetActive(false);
    //    }
    //    SetColor(1);
    //}

    //아이템 개수조정
    public void SetSlotCount(int _count)
    {
        itemCount += _count;
        text_Count.text = itemCount.ToString();

        if(itemCount <= 0)
        {
            ClearSlot();
        }
    }
    
    //슬롯 초기화
    private void ClearSlot()
    {
        item = null;
        itemCount = 0;
        itemImage.sprite = null;
        SetColor(0);

        text_Count.text = "0";
        go_CountImage.SetActive(false);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        //우클릭누르면 사용
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (item != null)
            {
                //사용
                Debug.Log("아이템 사용");
                itemEffectDatabase.UseItem(item);
                if (item.itemType == InteractiveWeapon.ItemType.Used)
                {
                    SetSlotCount(-1);
                }
            }
        }
    }

    //public void OnPointerClick(PointerEventData eventData)
    //{
    //    //우클릭누르면 사용
    //    if (eventData.button == PointerEventData.InputButton.Right)
    //    {
    //        if (item != null)
    //        {
    //            //사용
    //            Debug.Log("아이템 사용");
    //            itemEffectDatabase.UseItem(item);
    //            if (item.itemType == InteractiveWeapon.ItemType.Used)
    //            {
    //                SetSlotCount(-1);
    //            }
    //        }
    //    }
    //}

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(item != null)
        {
            DragSlot.instance.dragSlot = this;
            DragSlot.instance.DragSetImage(itemImage);
            //마우스클릭을한 상태도 이동하면 (드래그)이벤트가 발생한 그 위치로 이동
            DragSlot.instance.transform.position = eventData.position;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (item != null)
        {
            //마우스클릭을한 상태도 이동하면 (드래그)이벤트가 발생한 그 위치로 이동
            DragSlot.instance.transform.position = eventData.position;
        }
    }

    //그냥 드래그가 끝났을 때
    public void OnEndDrag(PointerEventData eventData)
    {
        //인벤토리영역을 벗어 났다면
        if(DragSlot.instance.transform.localPosition.x < baseRect.xMin || DragSlot.instance.transform.localPosition.x > baseRect.xMax
           || DragSlot.instance.transform.localPosition.y<baseRect.yMin || DragSlot.instance.transform.localPosition.y > baseRect.yMax)
        {
            if(DragSlot.instance.dragSlot!=null)
            {
                inputNumber.Call();
            }
            ////dragslot에 prefab을 플레이어의 앞에 생성시키기
            //Instantiate(DragSlot.instance.dragSlot.item.itemPrefab, shootBehaviour.transform.position + shootBehaviour.transform.forward, Quaternion.identity);
            ////dragslot 초기화
            //DragSlot.instance.dragSlot.ClearSlot();
            ////interactiveWeapon.weaponHUD.Toggle(false);
            Debug.Log("인벤토리 영역을 벗어났음");
        }
        else
        {
            DragSlot.instance.SetColor(0);
            DragSlot.instance.dragSlot = null;
        }
        
    }


    //다른 슬롯 위에서 드래그가 끝났을 때
    public void OnDrop(PointerEventData eventData)
    {
        if(DragSlot.instance.dragSlot != null)
            ChangeSlot();
    }


    //private void ChangeSlot()
    //{
    //    Item _tempItem = item; // b의복사본
    //    int _tempItemCount = itemCount; //b의복사본

    //    AddItem(DragSlot.instance.dragSlot.item, DragSlot.instance.dragSlot.itemCount); //b자리에 a가 들어감
        
    //    //b가 빈슬롯이었는지 아니었는지
    //    if(_tempItem != null )
    //        DragSlot.instance.dragSlot.AddItem(_tempItem, _tempItemCount); //a의 자리에 b의복사본이 들어감 => 결국 a와 b가 자리가 바뀜
    //    else
    //        DragSlot.instance.dragSlot.ClearSlot();
    //}
    private void ChangeSlot()
    {
        InteractiveWeapon _tempItem = item; // b의복사본
        int _tempItemCount = itemCount; //b의복사본

        AddItem(DragSlot.instance.dragSlot.item, DragSlot.instance.dragSlot.itemCount); //b자리에 a가 들어감

        //b가 빈슬롯이었는지 아니었는지
        if (_tempItem != null)
            DragSlot.instance.dragSlot.AddItem(_tempItem, _tempItemCount); //a의 자리에 b의복사본이 들어감 => 결국 a와 b가 자리가 바뀜
        else
            DragSlot.instance.dragSlot.ClearSlot();
    }
}
