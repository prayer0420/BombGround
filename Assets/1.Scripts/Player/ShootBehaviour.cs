using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FC;

/// <summary>
/// 사격기능 : 사격이 가능한지 여부를 체크하는 기능
/// 발사 키 입력을 받아서 애니메이션 재생, 이펙트 생성, 충돌 체크 기능
/// UI 관련해서 십자선표시 기능
/// 발사 속도 조정
/// 캐릭터 상체를 IK를 이용해서 조준 시점에 맞춰서 회전
/// 벽이나 충돌체에 총알이 피격되었을 경우 피탄이펙트를 생성.
/// 인벤토리 역할. 무기를 소지하고 있는지 확인용
/// 재장전과 무기교체 기능까지 포함하고 있어요.
/// </summary>
public class ShootBehaviour : GeneriBehaviour
{

    public Texture2D aimCrossHair, shootCrossHair; //십자선
    public GameObject muzzleFlash, shot, sparks; //섬광, 발사, 스파크
    public Material bulletHole;   //피탄 이펙트
    public int MaxBulletHoles = 50;  //최대 피탄 갯수
    public float shootErroRate = 0.0f; //오발 확률
    public float shootRateFactor = 1f; //발사 속도

    public float armsRotation = 8f; //조준시 팔 회전 

    //충돌체크용 마스크, 적용 안할 것들을 지정
    public LayerMask shotMask = ~(TagAndLayer.LayerMasking.IgnoreRayCast | TagAndLayer.LayerMasking.IgnoreShot | TagAndLayer.LayerMasking.CoverInvisible | TagAndLayer.LayerMasking.Player);

    //생명체에는 피탄 생기지 않도록
    public LayerMask organicMask = TagAndLayer.LayerMasking.Player | TagAndLayer.LayerMasking.Enemy;

    public Vector3 leftArmShortAim = new Vector3(-4.0f, 0.0f, 2.0f); //짧은총, 피스톨 같은 총을 들었을때 조준시 왼팔의 위치 보정


    private int activeWeapon = 0; //0이 아니면 활성화 되어있다

    //애니메이터용 값
    private int weaponType;
    private int changeweaponTrigger; 
    private int shootingTrigger;
    private int aimBool, blockedAimBool, reloadBool; //조준중인지, 조준이 막혔는지, 재장전 중인지

    private List<InteractiveWeapon> weapons; //소지하고 있는 무기들

    private bool isAiming, isAimBlocked; //조준중인지, 조준이 막혀있는지

    private Transform gunMuzzle;
    private float distToHand; //손부터 목까지의 거리

    private Vector3 castRelativeOrigin; //조정할떄 캐스트를 위한 목의 위치

    private Dictionary<InteractiveWeapon.WeaponType, int> slotMap; //어떤 무기타입이 어떤, 몇번째 슬롯에 들어간다

    private Transform hips, spine, chest, rightHand, leftArm; //IK용 트랜스폼
    private Vector3 initialRootRotation; //IK를 하려면 초기값이 필요함
    private Vector3 initialHipsRotation;
    private Vector3 initialSpineRotation;
    private Vector3 initialChestRotation;


    private float shotInterval, originalShotInterval; //총알 수명

    private List<GameObject> bulletHoles; //피탄 흔적들
    private int bulletHoleSlot = 0;
    private int burstShotCount = 0;
    private AimBehavour aimBehavour;
    private Texture2D originalCrossHair;
    private bool isShooting = false;
    private bool isChangingWeapon = false;
    private bool isShotAlive = false;




    private void Start()
    {
        weaponType = Animator.StringToHash(AnimatorKey.Weapon);
        aimBool = Animator.StringToHash(AnimatorKey.Aim);
        blockedAimBool = Animator.StringToHash(AnimatorKey.BlockedAim);
        changeweaponTrigger = Animator.StringToHash(AnimatorKey.ChangeWeapon);
        shootingTrigger = Animator.StringToHash(AnimatorKey.Shooting);
        reloadBool = Animator.StringToHash(AnimatorKey.Reload);
        weapons = new List<InteractiveWeapon>(new InteractiveWeapon[3]);
        aimBehavour = GetComponent<AimBehavour>();
        bulletHoles = new List<GameObject>();

        muzzleFlash.SetActive(false);
        shot.SetActive(false);
        sparks.SetActive(false);

        slotMap = new Dictionary<InteractiveWeapon.WeaponType, int>
        {
            {InteractiveWeapon.WeaponType.SHORT,1},
            {InteractiveWeapon.WeaponType.LONG,2}

        };


        Transform neck = this.behaviourController.GetAnimator.GetBoneTransform(HumanBodyBones.Neck);
        if (!neck)
        {
            neck = this.behaviourController.GetAnimator.GetBoneTransform(HumanBodyBones.Head).parent;
        }

        hips = this.behaviourController.GetAnimator.GetBoneTransform(HumanBodyBones.Hips);
        spine = this.behaviourController.GetAnimator.GetBoneTransform(HumanBodyBones.Spine);
        chest = this.behaviourController.GetAnimator.GetBoneTransform(HumanBodyBones.Chest);
        rightHand = this.behaviourController.GetAnimator.GetBoneTransform(HumanBodyBones.RightHand);
        leftArm = this.behaviourController.GetAnimator.GetBoneTransform(HumanBodyBones.LeftUpperArm);


        //default value
        initialRootRotation = (hips.parent == transform) ? Vector3.zero : hips.parent.localEulerAngles;
        initialHipsRotation = hips.localEulerAngles;
        initialSpineRotation = spine.localEulerAngles;
        initialChestRotation = chest.localEulerAngles;
        originalCrossHair = aimBehavour.crossHair;
        shotInterval = originalShotInterval;
        castRelativeOrigin = neck.position - transform.position;
        distToHand = (rightHand.position - neck.position).magnitude * 1.5f;
    }

    //발사 비주얼 담당
    private void DrawShoot(GameObject weapon, Vector3 destination, Vector3 targetNormal, Transform parent, bool placeSparks =true, bool placeBulletHole = true)
    {
        //총구를 살짝 오른쪽으로
        Vector3 origin = gunMuzzle.position - gunMuzzle.right * 0.5f;

        //총구 이펙트 활성화
        muzzleFlash.SetActive(true);
        //총구이펙트를 총구에 붙힘
        muzzleFlash.transform.SetParent(gunMuzzle);
        muzzleFlash.transform.localPosition = Vector3.zero;
        muzzleFlash.transform.localEulerAngles = Vector3.back * 90f;

        //발사
        GameObject instantShot = EffectManager.Instance.EffectOneShot((int)EffectList.tracer, origin);
        instantShot.SetActive(true);
        instantShot.transform.rotation = Quaternion.LookRotation(destination - origin);
        //이펙트를 자식으로 놔둠
        instantShot.transform.parent = shot.transform.parent;

        //스파크가 튈수있는 부분이면(벽.물체 등)
        if (placeSparks)
        {
            GameObject instantSparks = EffectManager.Instance.EffectOneShot((int)EffectList.sparks, destination);
            instantSparks.SetActive(true);
            instantSparks.transform.parent = sparks.transform.parent;
        }

        //탄흔
        if(placeBulletHole)
        {
            Quaternion hitRotation = Quaternion.FromToRotation(Vector3.back, targetNormal);
            GameObject bullet = null;
            if(bulletHoles.Count < MaxBulletHoles)
            {
                bullet = GameObject.CreatePrimitive(PrimitiveType.Quad);
                bullet.GetComponent<MeshRenderer>().material = bulletHole;
                bullet.GetComponent<Collider>().enabled = false;
                bullet.transform.localScale = Vector3.one * 0.07f;
                bullet.name = "BulletHole";
                bulletHoles.Add(bullet);
            }
            else
            {
                //재활용(처음만든것을 없애고 새로 생성)
                bullet = bulletHoles[bulletHoleSlot];
                bulletHoleSlot++;
                bulletHoleSlot %= MaxBulletHoles;
            }
            bullet.transform.position = destination + 0.01f * targetNormal;
            bullet.transform.rotation = hitRotation;
            bullet.transform.SetParent(parent);
        }
    }

    //총 쏘기
    
    private void ShootWeapon(int weapon, bool firstShot = true)
    {
        //반동, 에러, 충돌체크
        //조준 중인지, 재장전 중이면, 조준이막혔으면, 총을 발사한게 아니면, 첫번째 샷이 아니면
        if (!isAiming || isAimBlocked || behaviourController.GetAnimator.GetBool(reloadBool) ||
           !weapons[weapon].Shoot(firstShot))
        {
            return;
        }
        else
        {
           this.burstShotCount++;
            behaviourController.GetAnimator.SetTrigger(shootingTrigger); //총 쏘는 애니메이션
            aimBehavour.crossHair = shootCrossHair; //총 쏠때는 십자선을 바꿔줌

            behaviourController.GetCamScript.BounceVertical(weapons[weapon].recoilAngle); //반동

            //실패율
            Vector3 imprecision = Random.Range(-shootErroRate, shootErroRate) * behaviourController.playerCamera.forward;
            //올곧은 직선이 아니라 약간 흔들리게 쏨
            Ray ray = new Ray(behaviourController.playerCamera.position, behaviourController.playerCamera.forward + imprecision); 

            RaycastHit hit = default(RaycastHit);

            if(Physics.Raycast(ray, out hit, 500f, shotMask))
            {
                if(hit.collider.transform != transform)
                {
                    //피격된 laymask가 organicmask와 같니?
                    bool isOrganic = (organicMask == (organicMask | (1<< hit.transform.root.gameObject.layer)));

                    //총구멍과 이펙트만들기
                    DrawShoot(weapons[weapon].gameObject, hit.point, hit.normal, hit.collider.transform, !isOrganic, !isOrganic);

                    if (hit.collider)
                    {
                        //데미지 정보 알려줌
                        hit.collider.SendMessageUpwards("HitCallBack", new HealthBase.DamageInfo(hit.point, ray.direction, weapons[weapon].bulletDamage, hit.collider),
                            SendMessageOptions.DontRequireReceiver);
                    }
                }
            }
            //총에 맞은게 없으면
            else
            {
                Vector3 destination = (ray.direction * 500f) - ray.origin;
                //안맞았으니 위에다가 쏨(허공에 쏨)
                DrawShoot(weapons[weapon].gameObject, destination, Vector3.up, null, false, false);
            }
            SoundManager.Instance.PlayOneShotEffect((int)weapons[weapon].shotSound, gunMuzzle.position, 5f);
            GameObject gameController = GameObject.FindGameObjectWithTag(TagAndLayer.TagName.GameController);
            gameController.SendMessage("RootAlertNearBy", ray.origin, SendMessageOptions.DontRequireReceiver);

            //그다음 총쏠떄까지 0.5초
            shotInterval = 0.5f;
            isShotAlive = true;
        }
    }

    public void EndReloadWeapon()
    {
        behaviourController.GetAnimator.SetBool(reloadBool, false);
        weapons[activeWeapon].EndReload();
    }

    //장전되었는지
    private void SetWeaponCrossHair(bool armed)
    {
        if (armed)
        {
            aimBehavour.crossHair = aimCrossHair;
        }
        else
        {
            aimBehavour.crossHair = originalCrossHair;
        }
    }

    private void ShotProgress()
    {
        if(shotInterval > 0.2f)
        {
            //ratefactor가 크면 클수록 빨리 쏠 수있음
            shotInterval -= shootRateFactor * Time.deltaTime;
            //날라가고있따면
            if(shotInterval <= 0.4f)
            {
                SetWeaponCrossHair(activeWeapon > 0);
                muzzleFlash.SetActive(false);
                print("muzzle off");
                //무기를 장착한 부위가 있으면 
                if(activeWeapon > 0)
                {
                    //총구반동 줄여줌
                    behaviourController.GetCamScript.BounceVertical(-weapons[activeWeapon].recoilAngle * 0.1f);

                    //총모드에 따라 다음 인터벌 설정
                    if (shotInterval <= (0.4f - 2f * Time.deltaTime))
                    {
                        print(shotInterval);

                        if (weapons[activeWeapon].weaponMode == InteractiveWeapon.WeaponMode.AUTO &&
                            Input.GetAxisRaw(ButtonName.Shoot) != 0)
                        {
                            ShootWeapon(activeWeapon, false);
                        }
                        else if (weapons[activeWeapon].weaponMode == InteractiveWeapon.WeaponMode.BURST&&
                                 burstShotCount < weapons[activeWeapon].burstSize)
                        {
                            ShootWeapon(activeWeapon, false);
                        }
                        else if (weapons[activeWeapon].weaponMode != InteractiveWeapon.WeaponMode.BURST)
                        {
                            burstShotCount = 0;
                        }
                    }
                }

            }
        }
        //총 쏜지 꽤 됐으면
        else
        {
          isShotAlive = false;
            behaviourController.GetCamScript.BounceVertical(0); //반동X
            print("끝");

            burstShotCount = 0;

        }
    }

    //기존무기와 새로운 무기
    //획득한 무기를 또 획득하면 무기를 버리고 재장전시켜주고 새로운 무기를 획득했다면 빈 슬롯에 장착
    private void ChangeWeapon(int oldWeapon, int newWeapon)
    {
        if(oldWeapon > 0)
        {
            weapons[oldWeapon].gameObject.SetActive(false);

            gunMuzzle = null;
            weapons[oldWeapon].Toggle(false);
        }

        //기존에 갖고있지 않다면
        while (weapons[newWeapon] == null && newWeapon > 0)
        {
            //빈슬롯을 찾는다
            Debug.Log(weapons.Count);
            newWeapon = (newWeapon + 1) % weapons.Count;
        }
        //무기를 줏었으면
        if (newWeapon > 0)
        {
            weapons[newWeapon].gameObject.SetActive(true);
            gunMuzzle = weapons[newWeapon].transform.Find("muzzle");
            weapons[newWeapon].Toggle(true);
        }

        activeWeapon = newWeapon;

        if(oldWeapon != newWeapon)
        {
            //총기교환
            behaviourController.GetAnimator.SetTrigger(changeweaponTrigger);
            behaviourController.GetAnimator.SetInteger(weaponType, weapons[newWeapon] ? (int)weapons[newWeapon].weaponType : 0);
        }
        //새로운 무기의 십자선으로 바꿈
        SetWeaponCrossHair(newWeapon > 0);

    }

    //총을 쏘거나 재장전하거나 버리거나
    private void Update()
    {
        //무기발사트리거 활성화를 시켰냐
        float shootTrigger = Mathf.Abs(Input.GetAxisRaw(ButtonName.Shoot));
        //사격중이아니고, 활성화한 무기가있고, 버스트샷카운트가 0이라면
         if(shootTrigger > Mathf.Epsilon && !isShooting && activeWeapon > 0 &&burstShotCount == 0)
        {
            //사격중인것. 발사해라
            isShooting = true;
            ShootWeapon(activeWeapon);
        }
         else if(isShooting && shootTrigger < Mathf.Epsilon)
        {
            //총쏘는 행위가 끝난것
            isShooting = false;
            aimBehavour.crossHair = aimCrossHair; //총 안쏠때는 십자선을 바꿔줌

        }
         //장전중이고 활성화된 무기가 있다면
        else if(Input.GetButtonUp(ButtonName.Reload) && activeWeapon>0) 
        {
            if (weapons[activeWeapon].StartReload())
            {
                SoundManager.Instance.PlayOneShotEffect((int)weapons[activeWeapon].reloadSound,
                    gunMuzzle.position, 0.5f);
                behaviourController.GetAnimator.SetBool(reloadBool, true);
            }
        }

         //무기를 떨어트렸으면
         else if(Input.GetButtonDown(ButtonName.Drop) && activeWeapon > 0)
        {
            EndReloadWeapon();
            int weaponToDrop = activeWeapon;
            //빈무기랑 바꾸는 것 == 무기를 버리는것
            ChangeWeapon(activeWeapon, 0);
            weapons[weaponToDrop].Drop();
            weapons[weaponToDrop] = null;
        }

         //무기를 교체중이냐
        else
        {
            if((Mathf.Abs(Input.GetAxisRaw(ButtonName.Change)) > Mathf.Epsilon && !isChangingWeapon))
            {
                isChangingWeapon = true;
                int nextWeapon = activeWeapon + 1;
                ChangeWeapon(activeWeapon, nextWeapon % weapons.Count);
            }

            else if (Mathf.Abs(Input.GetAxisRaw(ButtonName.Change)) < Mathf.Epsilon)
            {
                isChangingWeapon = false;
            }
        }

        if (isShotAlive)
        {
            ShotProgress();
        }
        isAiming = behaviourController.GetAnimator.GetBool(aimBool);
    }

    /// <summary>
    /// 인벤토리 역할을 하게 될 함수
    /// </summary>
    /// <param name="weapon"></param>
    public void AddWeapon(InteractiveWeapon newWeapon)
    {
        newWeapon.gameObject.transform.SetParent(rightHand);
        newWeapon.transform.localPosition = newWeapon.rightHandPosition;
        newWeapon.transform.localRotation = Quaternion.Euler(newWeapon.relativeRotation);

        //똑같은 타입의 무기를 줏었다면
        if (weapons[slotMap[newWeapon.weaponType]])
        {
            //이름까지 똑같은 무기라면
            if (weapons[slotMap[newWeapon.weaponType]].label_weaponName == newWeapon.label_weaponName)
            {
                //총알을 채워줌
                weapons[slotMap[newWeapon.weaponType]].ResetBullet();
                ChangeWeapon(activeWeapon, slotMap[newWeapon.weaponType]);
                //새로운 무기는 버림
                Destroy(newWeapon.gameObject);
                return;
            }
            //똑같은 무기가 아니라면
            else
            {
                //갖고있던 무기를 떨궈
                weapons[slotMap[newWeapon.weaponType]].Drop();
            }
        }
        //다른 타입의 무기를 줏었다면
        weapons[slotMap[newWeapon.weaponType]] = newWeapon;
        ChangeWeapon(activeWeapon, slotMap[newWeapon.weaponType] );
    }

    private bool CheckforBlockedAim()
    {
        //손부터 목 사이에 막혔는지 확인
        isAimBlocked = Physics.SphereCast(transform.position + castRelativeOrigin, 0.1f,
            behaviourController.GetCamScript.transform.forward, out RaycastHit hit, distToHand - 0.1f);

        isAimBlocked = isAimBlocked && hit.collider.transform != transform;
        behaviourController.GetAnimator.SetBool(blockedAimBool, isAimBlocked);

        Debug.DrawRay(transform.position + castRelativeOrigin, behaviourController.GetCamScript.transform.forward * distToHand, isAimBlocked ? Color.red : Color.cyan);
        return isAimBlocked;
    }

    //조준에따라 상체바꿔주기
    public void OnAnimatorIK(int layerIndex)
    {
        //조준중이고 활성화된 무기가 있다면
        if(isAiming && activeWeapon > 0)
        {
            //조준할수없게 막혀있다면
            if(CheckforBlockedAim())
            {
                return;
            }
            Quaternion targetRot = Quaternion.Euler(0, transform.eulerAngles.y, 0);
            targetRot *= Quaternion.Euler(initialRootRotation);
            targetRot *= Quaternion.Euler(initialHipsRotation);
            targetRot *= Quaternion.Euler(initialSpineRotation);
            behaviourController.GetAnimator.SetBoneLocalRotation(HumanBodyBones.Spine, Quaternion.Inverse(hips.rotation) * targetRot);

            float xcamRot = Quaternion.LookRotation(behaviourController.playerCamera.forward).eulerAngles.x;
            targetRot = Quaternion.AngleAxis(xcamRot + armsRotation, this.transform.right);
            if (weapons[activeWeapon] && weapons[activeWeapon].weaponType == InteractiveWeapon.WeaponType.LONG)
            {
                targetRot *= Quaternion.AngleAxis(9f,transform.right);
                targetRot *= Quaternion.AngleAxis(20f, transform.up);
            }

            targetRot *= spine.rotation;
            targetRot *= Quaternion.Euler(initialChestRotation);
            behaviourController.GetAnimator.SetBoneLocalRotation(HumanBodyBones.Chest, Quaternion.Inverse(spine.rotation) * targetRot);
        }
    }

    private void LateUpdate()
    {
        //조준중이고, 활성화된무기가있고, 활성화된 무기가 short이라면 
        if(isAiming && weapons[activeWeapon] && weapons[activeWeapon].weaponType == InteractiveWeapon.WeaponType.SHORT)
        {
            leftArm.localEulerAngles = leftArm.localEulerAngles + leftArmShortAim;
        }
    }
}















