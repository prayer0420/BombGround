using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthHUD : MonoBehaviour
{
    public float decayDuration = 2f;  //HUD가 표시된 후 페이드 아웃되기까지의 지속 시간
    private Camera m_Camera;
    private Image hud, bar;
    private float decayTimer; // HUD의 페이드 아웃 지속 시간을 추적
    private Color originColor, noAlphaColor;

    private void Start()
    {
        hud = transform.Find("HUD").GetComponent<Image>();
        bar = transform.Find("Bar").GetComponent<Image>();
        m_Camera = Camera.main;
        originColor = noAlphaColor = hud.color;
        noAlphaColor.a = 0f; //투명하지 않은 색상

        gameObject.SetActive(false);
    }

     private void LateUpdate()
    {
        if(gameObject.activeSelf == false)
        {
            return;
        }
        //ui가 camera를 바라보게
        transform.LookAt(transform.position + m_Camera.transform.rotation * Vector3.forward,
            m_Camera.transform.rotation * Vector3.up);
        decayTimer += Time.deltaTime;

        if(decayTimer >= 0.5f * decayDuration) //페이드 아웃이 절반 이상 진행된 경우
        {
            float from = decayTimer - (0.5f * decayDuration);
            float to = 0.5f * decayDuration;
            hud.color = Color.Lerp(originColor, noAlphaColor, from/to);
            bar.color = Color.Lerp(originColor, noAlphaColor, from/to);
        }
        if(decayTimer >= decayDuration)
        {
            gameObject.SetActive(false );
        }

    }

    public void SetVisible()
    {
        gameObject.SetActive (true);
        decayTimer = 0f;
        hud.color = bar.color = originColor; 
    }


}
