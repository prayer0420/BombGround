using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HurtHUD : MonoBehaviour
{
    //피격된곳을 알려주는 UI


    struct HurtData
    {
        public Transform shotOrigin;
        public Image hurtImg;
    }

    private Transform canvas;
    private GameObject hurtPrefab;
    private float decayFactor = 0.8f;
    private Dictionary<int, HurtData> hurtUIData;
    private Transform player, cam;

    public void Setup(Transform canvas, GameObject hurtPrefab, float decayFactor, Transform player)
    {
        hurtUIData = new Dictionary<int, HurtData>();
        this.canvas = canvas;
        this.hurtPrefab = hurtPrefab;
        this.decayFactor = decayFactor;
        this.player = player;
        cam = Camera.main.transform;
    }

    //피격받은곳으로 회전
    private void SetRotation(Image hurtUI, Vector3 orientation, Vector3 shotDirection)
    {
        orientation.y = 0;
        shotDirection.y = 0;
        float rotation = Vector3.SignedAngle(shotDirection, orientation, Vector3.up);

        Vector3 newRotation = hurtUI.rectTransform.rotation.eulerAngles;
        newRotation.z = rotation; 
        Image hurtImg = hurtUI.GetComponent<Image>();
        hurtImg.rectTransform.rotation = Quaternion.Euler(newRotation);
    }

    //알파값 조정
    private Color GetUpdatedAlpha(Color currentColor, bool reset = false)
    {
        if(reset)
        {
            currentColor.a = 1f;
        }
        else
        {
            currentColor.a -= decayFactor * Time.deltaTime;
        }
        return currentColor;
    }

    //그리기
    public void DrawHurtUI(Transform shotOrigin, int hashID)
    {
        if(hurtUIData.ContainsKey(hashID))
        {
            hurtUIData[hashID].hurtImg.color = GetUpdatedAlpha(hurtUIData[hashID].hurtImg.color, true);

        }
        else
        {
            GameObject hurtUI = Instantiate(hurtPrefab, canvas);
            SetRotation(hurtUI.GetComponent<Image>(), cam.forward, shotOrigin.position - player.position);
            HurtData data;
            data.shotOrigin = shotOrigin;
            data.hurtImg = hurtUI.GetComponent<Image>();
            hurtUIData.Add(hashID, data);
        }
    }


    private void Update()
    {
        List<int> toRemoveKeys = new List<int>();
        foreach(int key in hurtUIData.Keys)
        {
            SetRotation(hurtUIData[key].hurtImg, cam.forward, hurtUIData[key].shotOrigin.position-player.position);
            hurtUIData[key].hurtImg.color = GetUpdatedAlpha(hurtUIData[key].hurtImg.color);
            if (hurtUIData[key].hurtImg.color.a <= 0f)
            {
                toRemoveKeys.Add(key);
            }
        }
        for (int i = 0; i < toRemoveKeys.Count; i++)
        {
            Destroy(hurtUIData[toRemoveKeys[i]].hurtImg.transform.gameObject);
            hurtUIData.Remove(toRemoveKeys[i]);
        }
    }

    


}

