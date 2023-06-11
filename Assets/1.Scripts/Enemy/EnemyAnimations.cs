using FC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System;


public class EnemyAnimations : MonoBehaviour
{
    [HideInInspector] public Animator anim;
    [HideInInspector] public float currentAimingAngleGap;//현재 조준중인 각도 차이
    [HideInInspector] public Transform gunMuzzle; //총구
    [HideInInspector] public float angularSpeed; //조준,회전 하는데 걸리는 시간


    private StateController controller;
    private NavMeshAgent nav;
    private bool pendingAim; //조준을 기다리는 시간.(바로 상체를 움직이는게아니라 캐릭터를 올바른방향으로 움직이고 나서 원하는 방향으로 움직임)
    private Transform hips, spine; //bone trnas
    private Vector3 initialRootRotation;
    private Vector3 initialHipsRotation;
    private Vector3 initialSpineRotation;
    private Quaternion lastRotation;
    private float timeCountAim, timeCountGuard; //원하는 회전값을 돌리기위한 타임카운트.(조준이랑 방어할때)
    private readonly float turnSpeed = 25f; //strafing할때 어떻게 이동할지  빠르기 정함
    public ClassStats ClassStats;

    private void Awake()
    {
        //setup
        controller = GetComponent<StateController>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        nav.updateRotation = false;

        hips = anim.GetBoneTransform(HumanBodyBones.Hips);
        spine = anim.GetBoneTransform(HumanBodyBones.Spine);

        initialRootRotation = (hips.parent == transform) ? Vector3.zero : hips.parent.localEulerAngles;
        initialHipsRotation = hips.localEulerAngles;
        initialSpineRotation = spine.localEulerAngles;

        anim.SetTrigger(AnimatorKey.ChangeWeapon);
        //짧은총인지 긴 총인지 셋팅
        anim.SetInteger(AnimatorKey.Weapon, (int)Enum.Parse(typeof(WeaponType), controller.classStats.WeaponType));


        //muzzle셋팅
        foreach (Transform child in anim.GetBoneTransform(HumanBodyBones.RightHand))
        {
            gunMuzzle = child.Find("muzzle");
            if(gunMuzzle != null)
            {
                break;
            }

        }
        //rigidbody끄기
        foreach(Rigidbody member in GetComponentsInChildren<Rigidbody>())
        {
            member.isKinematic = true;
        }
    }

    //애니메이터에 여러 파라미터를 셋업해줌
    //nav업데이튼 동안 호출
    void Setup(float speed, float angle, Vector3 strafeDirection)
    {
        angle *= Mathf.Deg2Rad; //Mathf.Deg2Rad = 1도를 라디안으로 바꾸는 상수값. 삼각함수 사용하려면 도를 라디안으로 바꿔줘야함
        
        angularSpeed = angle / controller.generalStats.angleResponseTime; //변환된 각도를 시간에 대한 각속도(단위 시간당 회전하는 각도)로 계산하는 과정
        anim.SetFloat(AnimatorKey.Speed, speed,  controller.generalStats.speedDampTime, Time.deltaTime);
        anim.SetFloat(AnimatorKey.AngularSpeed, angularSpeed, controller.generalStats.angularSpeedDampTime, Time.deltaTime);

        anim.SetFloat(AnimatorKey.Horizontal, strafeDirection.x, controller.generalStats.speedDampTime, Time.deltaTime);
        anim.SetFloat(AnimatorKey.Vertical, strafeDirection.z, controller.generalStats.speedDampTime, Time.deltaTime);
    }

    void NavAnimSetup()
    {
        float speed;
        float angle;

        speed = Vector3.Project(nav.desiredVelocity, transform.forward).magnitude;
        // nav.desiredVelocity: 목표 지점으로 이동하기 위해 계산된 속도 벡터. 이 벡터는 현재 위치와 목표 위치 사이의 이동 방향과 속도
        // nav.desiredVelocity는 글로벌좌표이므로 로컬좌표로 변환하기위해서 transform.forwar에 사영해서 객체의 전방으로 나아가게하고
        // magnitude로 크기를 구함

        if (controller.focusSight)
        {
            Vector3 dest = (controller.personalTarget - transform.position);
            dest.y = 0.0f;
            //SignedAngle이 0보다작으면 내가봤을때 왼쪽/ 크면 오른쪽
            angle = Vector3.SignedAngle(transform.forward, dest, transform.up);

            if(controller.Strafing)
            {
                Debug.Log("스트레핑중이다");
                dest = dest.normalized; //방향
                Quaternion targetStrafeRotation = Quaternion.LookRotation(dest); //LookRotation에 너무 작은값을 주면 안됨
                //현재 각도를 타겟을향하도록 부드럽게 회전 시켜줌
                transform.rotation = Quaternion.Lerp(transform.rotation, targetStrafeRotation, turnSpeed * Time.deltaTime);
            }

        }
        //포커싱되어있지않으면
        else
        {
            //nav움직이고자 하는 방향이 없다면
            if(nav.desiredVelocity == Vector3.zero)
            {
                angle = 0.0f;
            }

            else
            {
                angle = Vector3.SignedAngle(transform.forward, nav.desiredVelocity, transform.up);
            }
        }

        //플레이어를 향하려 할 때 깜빡 거리지 않도록 각도 데드존을 적용
        if(!controller.Strafing && Mathf.Abs(angle) < controller.generalStats.angleDeadZone)
        {
            //객체를 이동방향 정면으로 회전시킴
            transform.LookAt(transform.position + nav.desiredVelocity);
            angle = 0f;
            //조준할수없는데 타겟을 바라보고 있으면
            if(pendingAim && controller.focusSight)
            {
                controller.Aimimg = true;
                pendingAim = false;
            }
        }

        //Strafe Direction
        Vector3 direction = nav.desiredVelocity;
        direction.y = 0.0f; 
        direction = direction.normalized;
        direction = Quaternion.Inverse(transform.rotation) * direction; //4차원을 3차원으로 바꾸려면 인버스해줘야함. 로컬좌표계로 바꿔주기 위함
        Setup(speed,angle,direction);
    }


    private void Update()
    {
        NavAnimSetup();
    }

    //이동,회전할때 보정
    private void OnAnimatorMove()
    {

        if(Time.timeScale > 0 && Time.deltaTime > 0) //timescale이 0이되면 게임이 일시정지됨(업데이트가 안됨).
        {
            nav.velocity = anim.deltaPosition / Time.deltaTime;
            if(!controller.Strafing)
            {
                transform.rotation = anim.rootRotation;
            }
        }
    }

    //조준할때 보정
    private void LateUpdate()
    {
        if (controller.Aimimg)
        {
            Vector3 direction = controller.personalTarget - spine.position; //내위치

            if(direction.magnitude < 0.01f || direction.magnitude > 1000000.0f)
            {
                return;
            }

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            targetRotation *= Quaternion.Euler(initialRootRotation);
            targetRotation *= Quaternion.Euler(initialHipsRotation);
            targetRotation *= Quaternion.Euler(initialSpineRotation);

            //aimoffset에 따라 회전값이 바뀌도록
            targetRotation *= Quaternion.Euler(VectorHelper.ToVector(controller.classStats.AimOffset));
            //회전값 보간
            Quaternion frameRotation = Quaternion.Slerp(lastRotation, targetRotation, timeCountAim);
            //각도가 60도 이하일 동안(엉덩이를 기준으로 척추 회전이 60도 이하인 경우에는 계속 조준이 가능하다
            if(Quaternion.Angle(frameRotation, hips.rotation) <= 60.0f)
            {
                spine.rotation = frameRotation;
                timeCountAim += Time.deltaTime;
            }
            else
            {
                //비현실적 회전 방지
                if(timeCountAim ==0 && Quaternion.Angle(frameRotation, hips.rotation) > 70.0f)
                {
                    StartCoroutine(controller.UnstuckAim(2f));
                }
                spine.rotation = lastRotation;
                timeCountAim = 0;
            }

            lastRotation = spine.rotation;
            Vector3 target = controller.personalTarget - gunMuzzle.position;
            Vector3 forward = gunMuzzle.forward;
            currentAimingAngleGap =  Vector3.Angle(target, forward);

            timeCountGuard = 0;

        }
        //조준이 끝났을떄 천천히 돌아오게
        else
        {
            lastRotation = spine.rotation;
            spine.rotation *= Quaternion.Slerp(Quaternion.Euler(VectorHelper.ToVector(controller.classStats.AimOffset)), Quaternion.identity, timeCountGuard);
            timeCountGuard += Time.deltaTime;
            Debug.Log("조준끝났어");
        }
    }

    public void ActivePendingAim()
    {
        pendingAim = true;
    }

    public void AbortPendingAim()
    {
        //controller.enemyAnimation.anim.SetBool(AnimatorKey.Aim, false);
        pendingAim = false;
        controller.Aimimg = false;
    }
}

