using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FC;
using UnityEngine.PlayerLoop;
/// <summary>
/// 충돌체를 생성해 무기를 줏을 수 있도록 한다.
/// 루팅(줍는)했으면 충돌체는 제거
/// 무기를 다시 버릴수도 있어야 하며, 충돌체를 다시 붙여준다.
/// 관련해서 UI도 컨트롤 할 수 있어야 하고
/// ShootBehaviour에 줏은 무기를 넣어주게 됩니다.
/// </summary>
/// 

public class InteractiveWeapon : MonoBehaviour
{
    public string label_weaponName; //무기 이름
    public SoundList shotSound, reloadSound, pickSound, dropSound, noBulletSound;
    public Sprite weaponSprite;
    public Vector3 rightHandPosition; //플레이어 오른손에 보정 위치
    public Vector3 relativeRotation; // 총 종류에 따라 플레이어에 맞춘 보정을 위한 회전값
    public float bulletDamage = 10f; //데미지
    public float recoilAngle; //반동



    public enum WeaponType
    {
        NONE,
        SHORT,
        LONG,
    }
    public enum WeaponMode
    {
        SEMI,
        BURST,
        AUTO,
    }

    public WeaponType weaponType = WeaponType.NONE;
    public WeaponMode weaponMode = WeaponMode.SEMI;
    public int burstSize = 1;

    //탄창정보
    public int currentMagCapcity, totalBullets; //현재 탄창양과, 소지하고있는 전체 총알량
    private int fullMag, maxBullets; //재장전시 꽉 채우는 탄의 양과 한번에 채울수 있는 최대 총알량
    private GameObject player, gameController;
    private ShootBehaviour playerInventory;
    private BoxCollider weaponCollider;
    private SphereCollider interactiveRadius; //인터렉티브할수있는 반경
    private Rigidbody weaponRigidbody;
    private bool pickable;

    //UI
    public GameObject screenHUD;
    public WeaponUIManager weaponHUD;
    private Transform pickHUD;
    public Text pickupHUD_Label;

    public Transform muzzleTransform; //총구

    private void Awake()
    {
        gameObject.name = this.label_weaponName;
        gameObject.layer = LayerMask.NameToLayer(TagAndLayer.LayerName.IgnoreRayCast);
        foreach(Transform tr in transform)
        {
            tr.gameObject.layer = LayerMask.NameToLayer(TagAndLayer.LayerName.IgnoreRayCast);
        }

        player = GameObject.FindGameObjectWithTag(TagAndLayer.TagName.Player);
        playerInventory = player.GetComponent<ShootBehaviour>();
        gameController = GameObject.FindGameObjectWithTag(TagAndLayer.TagName.GameController);

        //예외처리
        if(weaponHUD == null)
        {
            if(screenHUD == null)
            {
                screenHUD = GameObject.Find("ScreenHUD");
            }
            weaponHUD = screenHUD.GetComponent<WeaponUIManager>();
        }
        if (pickHUD == null)
        {
            pickHUD = gameController.transform.Find("PickupHUD");
        }

        //인터렉션을 위한 충돌체 설정(자식에다가 붙임)
        weaponCollider = transform.GetChild(0).gameObject.AddComponent<BoxCollider>();
        
        CreatInteractiveRadius(weaponCollider.center);

        weaponRigidbody = gameObject.AddComponent<Rigidbody>();
        
        if(this.weaponType == WeaponType.NONE)
        {
            this.weaponType = WeaponType.SHORT;
        }

        fullMag = currentMagCapcity;
        maxBullets = totalBullets;
        //무기안들고있을땐 탄창 화면 끄기
        pickHUD.gameObject.SetActive(false);
        if(muzzleTransform == null)
        {
            muzzleTransform = transform.Find("muzzle");
        }
    }

    //인터렉션 반경 만드는 함수
    private void CreatInteractiveRadius(Vector3 center)
    {
        interactiveRadius = gameObject.AddComponent<SphereCollider>();
        interactiveRadius.center = center;
        interactiveRadius.radius = 1;
        interactiveRadius.isTrigger = true;
    }

    //반경안에 플레이어가 들어오면 플레이어에게  UI를 보여줌
    private void TogglePickHUD(bool toggle)
    {
        pickHUD.gameObject.SetActive(toggle);
        if(toggle)
        {
            //ui가 플레이어를 향하도록
            pickHUD.position = this.transform.position + Vector3.up * 0.5f;
            Vector3 direction = player.GetComponent<behaviourController>().playerCamera.forward;
            
            direction.y = 0;
            //플레이어를 바라보고 회전하도록
            pickHUD.rotation = Quaternion.LookRotation(direction);
            //무기 위에 UI창 띄우기
            pickupHUD_Label.text = "Pick" + this.gameObject.name;
        }
    }

    //탄창 현황 보여주기
    private void UpdateHUD()
    {
        weaponHUD.UpdateWeaponHUD(weaponSprite, currentMagCapcity, fullMag, totalBullets);
    }


    public void Toggle(bool active)
    {
        if(active)
        {
            //아이템 주을때 나는 소리
            SoundManager.Instance.PlayOneShotEffect((int)pickSound, transform.position, 0.5f);
        }
        weaponHUD.Toggle(active);
        UpdateHUD();
    }

    private void Update()
    {
        //주을때 기능
        if(this.pickable && Input.GetButtonDown(ButtonName.Pick))
        {
            //disable physics weapon
            weaponRigidbody.isKinematic = true;
            weaponCollider.enabled = false;

            playerInventory.AddWeapon(this);
            Destroy(interactiveRadius);
            this.Toggle(true);
            this.pickable = false;

            TogglePickHUD(false);
        }
    }


    //총을 버릴때
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject != player && Vector3.Distance(transform.position, player.transform.position) <= 5f)
        {
            SoundManager.Instance.PlayOneShotEffect((int)dropSound, transform.position, 0.5f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //trigger밖에 벗어나면 못주음
        if(other.gameObject == player)
        {
            pickable = false;
            TogglePickHUD(false);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        //인벤토리가 있고 활성화되어있으면(살아있으면)
        if(other.gameObject == player && playerInventory && playerInventory.isActiveAndEnabled)
        {
            //주울수 있음
            pickable = true;
            TogglePickHUD(true);
        }
    }

    public void Drop()
    {
        gameObject.SetActive(true);
        //약간 던지는 느낌
        transform.position += Vector3.up;
        //물리작용 On
        weaponRigidbody.isKinematic = false;
        //플레이어에 붙여놓은걸 뗌
        this.transform.parent = null;
        CreatInteractiveRadius(weaponCollider.center);
        this.weaponCollider.enabled = true;
        weaponHUD.Toggle(false);
    }

    public bool StartReload()
    {
        //현재 탄창양이 꽉찼거나, 현재 소지하고있는 총알이 없으면
        //(재장전 할 필요가 없을때, 재장전 할 총알이 없을 때)
        if (currentMagCapcity == fullMag || totalBullets == 0)
        {
            return false;
        }
        //현재 총알이 전체 탄창량에서 현재 탄창량을 뺸 만큼보다 작으면
        //(내가 갖고있는게 탄창에 채울수있는만큼보다 적을때)
        else if(totalBullets < fullMag - currentMagCapcity)
        {
            //현재탄알들을 다 탄창에 채움
            currentMagCapcity += totalBullets;
            //현재 탄알들 0
            totalBullets = 0;
        }
        else
        {
            //전체 탄창의 용량에서 현재 탄창을 빼주면, 내가 채워야할 탄창의 갯수를 알게되고
            //내가 가지고있는 총알에서 내가 채워야할 탄창의 갯수를 빼줌
             totalBullets -= fullMag - currentMagCapcity;
            //현재 탄창을 전체 탄만큼 채워줌(위에서 다 뺀만큼)
            currentMagCapcity = fullMag;
        }
        return true;
    }

    public void EndReload()
    {
        UpdateHUD();
    }

    //총 쏠 때
    public bool Shoot(bool firstShot = true)
    {
        if(currentMagCapcity > 0)
        {
            currentMagCapcity--;
            UpdateHUD();
            return true;
        }
        //첫번째 쐈다면
        if(firstShot && noBulletSound != SoundList.None)
        {
            //총구에서 처음 나갈때 나는 소리나게
            SoundManager.Instance.PlayOneShotEffect((int)noBulletSound, muzzleTransform.position, 0.5f);
        }
        
        return false;
    }

    //탄창아이템 등을 주워서 총알 채워줄때
    public void ResetBullet()
    {
        //잔탄량을 꽉 채워줌
        currentMagCapcity = fullMag;
        totalBullets = maxBullets;
    }

}
