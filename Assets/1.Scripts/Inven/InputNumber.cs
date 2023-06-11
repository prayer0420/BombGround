using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputNumber : MonoBehaviour
{
    [SerializeField]
    private Text text_Preivew;
    [SerializeField]
    private Text text_Input;
    [SerializeField]
    private InputField if_text;
    [SerializeField]
    private GameObject go_Base;
    [SerializeField]
    private ActionController actionController;

    private bool activated = false;
    public void Call()
    {
        activated = true;
        go_Base.SetActive(true);
        if_text.text = "";
        //text_preview에는 최대갯수롤 적어놓음
        text_Preivew.text = DragSlot.instance.dragSlot.itemCount.ToString();
    }


    public void Cancel()
    {
        activated = false;
        go_Base.SetActive(false);
        DragSlot.instance.SetColor(0);

        DragSlot.instance.dragSlot = null;
    }

    public void OK()
    {
        DragSlot.instance.SetColor(0);
        int num;
        //숫자인지 한글인지 확인
        if(text_Input.text != "")
        {
            //숫자체크해서 숫자라면
            if (CheckNumber(text_Input.text))
            {
                num = int.Parse(text_Input.text);
                //num이 아이템개수보다 높으면 최대치로 설정
                if(num > DragSlot.instance.dragSlot.itemCount)
                    num=DragSlot.instance.dragSlot.itemCount;
            }
            //문자라면 그냥 다 1로 처리
            else
            {
                num = 1;
            }
        }
        //아무것도 적지 않을 때
        else
        {
            //text_preview에는 최대갯수롤 적어놓음
            //아무것도 안적으면 최대갯수를 떨굼
            num = int.Parse(text_Preivew.text);
        }
        StartCoroutine(DropItemCoroutine(num));
    }

    //하나씩 떨어트리게(빠르게)
    IEnumerator DropItemCoroutine(int _num)
    {
        for (int i = 0; i < _num; i++)
        {
            Instantiate(DragSlot.instance.dragSlot.item.itemPrefab, actionController.transform.position + actionController.transform.forward, Quaternion.identity);
            DragSlot.instance.dragSlot.SetSlotCount(-1);
            yield return new WaitForSeconds(0.05f);
        }
        DragSlot.instance.dragSlot = null;
        go_Base.SetActive(false) ;
        activated = false;

    }

    //숫자인지 아닌지 체크
    public bool CheckNumber(string _argString)
    {
        //문자열이 char배열로 들어감
        //argString = 가나다라   => _tempCharAraay[0] = "가",  _tempCharAraay[1] = "나" ....
        char[] _tempCharArray = _argString.ToCharArray();
        bool isNumber = true; //숫자라고 간주
        for (int i = 0; i < _tempCharArray.Length; i++)
        {
            //숫자라는 뜻
            if (_tempCharArray[i] >= 48 && _tempCharArray[i] <= 57)
                continue;
            //문자가 하나라도 있으면 false
            isNumber = false;  
        }
        return isNumber;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(activated)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                OK();
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cancel();
            }
        }
    }
}
