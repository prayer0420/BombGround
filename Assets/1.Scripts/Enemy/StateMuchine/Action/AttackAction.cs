using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FC;

/// <summary>
/// 사격 4단계
/// 1. 조준 중이고 조준 유효 각도 안에 타겟이 있거나 가깝다면
/// 2. 발사 간격 딜레이가 충분히 되었다면 애니메이션을 재생
/// 3. 충돌 검출을 하는데 약간의 사격시 충결파도 더해주게 된다(오발률)
/// 4. 총구 이펙트 및 총알 이펙트 생성
/// </summary>

[CreateAssetMenu(menuName = "PluggableAI/Actions/Attack")]

public class AttackAction : Action
{
    private readonly float startShootDelay = 0.2f; //발사간격 딜레이
    private readonly float aimAngleGap = 30f; //조준각도의 차이
    ClearShotDecision clearShotDecision;

    public override void OnReadyAction(StateController controller)
    {
        //최대 쏠 수있는 총알갯수를 랜덤으로 지정
        controller.variables.shotsInRounds = Random.Range(controller.maximumBurst/2, controller.maximumBurst);
        controller.variables.currentShots = 0; //초기화
        controller.variables.startShootTimer = 0f; //총쏠수있는 타이밍 체크
        controller.enemyAnimation.anim.ResetTrigger(AnimatorKey.Shooting); //발사중인건 멈추고
        controller.enemyAnimation.anim.SetBool(AnimatorKey.Crouch, false); //엄폐하고있으면 일어나기
        controller.variables.waitInCoverTime = 0; //엄폐물에서 기다린시간 초기화
        controller.enemyAnimation.ActivePendingAim(); //조준대기, 이제 시야에만 들어오면 조중가능
    }


    private void DoShot(StateController controller, Vector3 direction, Vector3 hitPoint, Vector3 hitNormal = default, bool organic = false, Transform target = null)
    {
        /// 4. 총구 이펙트 및 총알 이펙트 생성
        GameObject muzzleFlash = EffectManager.Instance.EffectOneShot((int)EffectList.flash, Vector3.zero);
        muzzleFlash.transform.SetParent(controller.enemyAnimation.gunMuzzle);
        muzzleFlash.transform.localPosition = Vector3.zero;
        muzzleFlash.transform.eulerAngles = Vector3.left * 90f;

        DestroyDelayed destroyDelayed = muzzleFlash.AddComponent<DestroyDelayed>();
        destroyDelayed.DelayTime = 0.5f; //0.5초 뒤에 사라짐


        //총알 발사체 궤적 
        GameObject shotTracer = EffectManager.Instance.EffectOneShot((int)EffectList.tracer, Vector3.zero);
        shotTracer.transform.SetParent(controller.enemyAnimation.gunMuzzle);
        Vector3 origin = controller.enemyAnimation.gunMuzzle.position;
        shotTracer.transform.position = origin;
        shotTracer.transform.rotation = Quaternion.LookRotation(direction);

        //타겟이있는데 미생물체라면
        if(target && !organic)
        {
            //탄흔 이펙트
            GameObject bulletHole = EffectManager.Instance.EffectOneShot((int)EffectList.bulletHole, hitPoint + 0.01f * hitNormal);
            bulletHole.transform.rotation = Quaternion.FromToRotation(Vector3.up, hitNormal);

            //스파크 튀기기
            GameObject instantSpark = EffectManager.Instance.EffectOneShot((int)EffectList.sparks, hitPoint);
        }
        else if(target && organic) //player
        {
            HealthBase targetHealth =  target.GetComponent<HealthBase>(); // playerHealth

            if(targetHealth)
            {
                targetHealth.TakeDamage(hitPoint, direction, controller.classStats.BulletDamage, target.GetComponent<Collider>(), controller.gameObject);
            }
        }

        //총 사운드
        SoundManager.Instance.PlayShotSound(controller.classID,controller.enemyAnimation.gunMuzzle.position, 2f);
    }


    /// 3. 충돌 검출을 하는데 약간의 사격시 충격파도 더해주게 된다(오발률)
    private void CastShot(StateController controller)
    {
        //오발률 설정
        Vector3 impercision = Random.Range(-controller.classStats.ShotErrorRate, controller.classStats.ShotErrorRate) * controller.transform.right; //오른쪽으로 좀 치우치기
        impercision += Random.Range(-controller.classStats.ShotErrorRate, controller.classStats.ShotErrorRate) * controller.transform.up; //위로 좀 치우치기

        Vector3 shotDirection = controller.personalTarget - controller.enemyAnimation.gunMuzzle.position;
        shotDirection = shotDirection.normalized + impercision;

        Ray ray = new Ray(controller.enemyAnimation.gunMuzzle.position, shotDirection);
        if(Physics.Raycast(ray, out RaycastHit hit, controller.viewRadius, controller.generalStats.shotMask.value))
        {
            bool isOrganic = ((1 << hit.transform.root.gameObject.layer) & controller.generalStats.targetMask) != 0;
            DoShot(controller, ray.direction, hit.point, hit.normal, isOrganic, hit.transform);
        }
        else
        {
            DoShot(controller, ray.direction, ray.origin + (ray.direction * 500f)); //허공에 쏨
        }
    }

    /// 1. 조준 중이고 조준 유효 각도 안에 타겟이 있거나 가깝다면
    /// 2. 발사 간격 딜레이가 충분히 되었다면 애니메이션을 재생
    public FocusDecision targetNear; //가까이 있니?
    public LookDecision isViewing;
    public EnemyAnimations enemyAnimations;


    private bool CanShoot(StateController controller)
    {
        //타겟과 총구사이 거리
        float distance = (controller.personalTarget - controller.enemyAnimation.gunMuzzle.position).sqrMagnitude;


        //각도 차이안에 들어가있고, 조준중이고, 거리가 가깝다면
        if (!controller.reloading && controller.Aimimg && (controller.enemyAnimation.currentAimingAngleGap < aimAngleGap) || distance <= 7.0f || targetNear.Decide(controller))
            //if (controller.Aimimg && (controller.enemyAnimation.currentAimingAngleGap < aimAngleGap) || distance <= 7.0f)
            {
            if(controller.variables.startShootTimer >= startShootDelay)
            {
                return true;
            }
            else
            {
                controller.variables.startShootTimer += Time.deltaTime;
            }
        }
        return false;
    }

    private void Shoot(StateController controller)
    {

        //shotTimer가 0이되면 발사함
        if(Time.timeScale > 0 && controller.variables.shotTimer == 0f)
        {
            controller.enemyAnimation.anim.SetTrigger(AnimatorKey.Shooting);
            CastShot(controller);
            //Debug.Log(controller.variables.shotTimer);
        }
        else if(controller.variables.shotTimer >= (3f * Time.deltaTime))
        {
            controller.bullets = Mathf.Max(--controller.bullets, 0);
            controller.variables.currentShots++;
            controller.variables.shotTimer = 0;
            //Debug.Log(controller.variables.shotTimer);
            return;
        }
        
        controller.variables.shotTimer += controller.classStats.ShotRateFactor * Time.deltaTime;
    }

    public override void Act(StateController controller)
    {
        controller.focusSight = true;

        //총을 발사할수 있는 상황이면
        if (CanShoot(controller))
        {
            Shoot(controller);
        }
        controller.variables.blindEngageTimer += Time.deltaTime;
    }


}
