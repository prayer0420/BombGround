using FC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertCheck : MonoBehaviour
{
    
    //flolat값을 사전에 제한을 둠(0~50)
    [Range(0,50)] public float alertRadius; //경고 보낼 반경
    public int extraWaves = 1; //경고를 몇 웨이브씩 보낼지

    public LayerMask aleretMask = TagAndLayer.LayerMasking.Enemy;
    private Vector3 current; //현재 위치
    private bool alert; //새로운 경고가 도착했나

    private void Start()
    {
        //특정함수를 1초타임에 1초주기로 반복
        InvokeRepeating("PingAlert", 1, 1);
    }

    
    //특정지점 주위로 보내라
    private void AlertNearBy(Vector3 origin,Vector3 target, int wave = 0)
    {
        if(wave > this.extraWaves)
        {
            return;
        }
        //적한테 경고를 보내라
        Collider[] targetInViewRadius = Physics.OverlapSphere(origin, alertRadius, aleretMask);

        foreach(Collider obj in targetInViewRadius)
        {
            obj.SendMessageUpwards("AlertCallback", target, SendMessageOptions.DontRequireReceiver);
            AlertNearBy(obj.transform.position, target, wave + 1);

        }
    }

   public void RootAlerNearBy(Vector3 origin)
    {
        current = origin;
        alert = true;
    }

    //경고보내기
    void PingAlert()
    {
        if (alert)
        {
            alert = false;
            AlertNearBy(current, current);
        }
    }


}
