using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponUIManager : MonoBehaviour
{

    public Color bulletColor = Color.white;
    public Color emptyBullerColor = Color.black;
    private Color noBulletColor; //투명하게 색깔 표시


    [SerializeField]
    private Image weaponHUD;
    [SerializeField]
    private GameObject bulletMag;
    [SerializeField]
    private Text totalBulletsHUD;

    void Start()
    {
        noBulletColor = new Color(0f, 0f, 0f, 0f);

        //링크 깨졌을때 예외처리
        if(weaponHUD == null)
        {
            weaponHUD = transform.Find("WeaponHUD/Weapon").GetComponent<Image>();
        }

        if(bulletMag == null)
        {
            bulletMag = transform.Find("WeaponHUD/Data/Mag").gameObject;
        }

        if(totalBulletsHUD == null)
        {
            totalBulletsHUD = transform.Find("WeaponHUD/Data/bulletAmount").GetComponent<Text>();   
        }
        Toggle(false);
    }


    //탄창 껐다 켜기
    public void Toggle(bool active)
    {
        weaponHUD.transform.parent.gameObject.SetActive(active);
    }


    public void UpdateWeaponHUD(Sprite weaponSprite, int bulletLeft, int fullMag, int ExtraBullet)
    {
        if(weaponSprite !=null && weaponHUD.sprite != weaponSprite)
        {
            weaponHUD.sprite = weaponSprite;
            weaponHUD.type = Image.Type.Filled;
            weaponHUD.fillMethod = Image.FillMethod.Horizontal;
        }
        int bulletCount = 0;
        foreach(Transform bullet in bulletMag.transform)
        {
            //잔탄
            if(bulletCount < bulletLeft)
            {
                bullet.GetComponent<Image>().color = bulletColor;
            }
            //넘치는 탄
            else if(bulletCount > fullMag) 
            {
                bullet.GetComponent<Image>().color = noBulletColor;
            }
            //사용한 탄
            else
            {
                bullet.GetComponent<Image>().color = emptyBullerColor;
            }
            bulletCount++;
        }
        totalBulletsHUD.text = bulletLeft + " / " + ExtraBullet;
    }
}
