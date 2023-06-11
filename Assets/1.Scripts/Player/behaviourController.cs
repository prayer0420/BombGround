using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 현재 동작(current), 기본 동작(default), 오버라이딩 동작, 잠긴동작, 마우스 이동값
/// 땅에 서있는지, GenericBehaviour를 상속받은 동작들을 업데이트 시켜줍니다.
/// </summary>
public class behaviourController : MonoBehaviour
{

    private List<GeneriBehaviour> behaviours; // 동작들
    private List<GeneriBehaviour> overrideBehaviours; //우선시 되는 동작.
    private int currentBehaviour; // 현재 동작 해시코드
    private int defaultBehaviour; //기본 동작 해시코드
    private int behaviourLocked;  //잠긴 동작 해시코드

    //캐싱
    public Transform playerCamera;
    private Animator myAnimator;
    private Rigidbody myRigidbody;
    private ThridPersonOrbitCam camScript;
    private Transform myTransform;

    //값
    private float h; //horizontal axis
    private float v; //vertical axis
    public float turnSmoothing = 0.06f; //카메라를 향하도록 움직일때 회전속도
    private bool changedFOV; //달리기 동작이 카메라 시야각이 변경되었을 때 저장되었니
    public float sprintFOV = 100; //달리기 시야각
    private Vector3 lastDirection; //마지막 향했던 방향
    private bool sprint; //달리기 중인가?
    private int hFloat; //애니메이터 관련 가로축 값
    private int vFloat; //애니메이터 관련 세로축 값
    private int groundedBool; //애니메이터 지상에 있는가
    private Vector3 colExtents; //땅과의 충돌체크를 위한 충돌체 영역


    public float GetH { get => h; }
    public float GetV { get => v; }

    public ThridPersonOrbitCam GetCamScript { get => camScript; }
    public Rigidbody GetRigidbody { get => myRigidbody; }
    public Animator GetAnimator { get => myAnimator; }
    public int GetDefaultBehaviour { get => defaultBehaviour; }


    private void Awake()
    {
        behaviours = new List<GeneriBehaviour>();
        overrideBehaviours = new List<GeneriBehaviour>();
        myAnimator = GetComponent<Animator>();
        hFloat = Animator.StringToHash(FC.AnimatorKey.Horizontal);
        vFloat = Animator.StringToHash(FC.AnimatorKey.Vertical);
        camScript = playerCamera.GetComponent<ThridPersonOrbitCam>();
        myRigidbody = GetComponent<Rigidbody>();
        myTransform = transform;

        //ground?
        groundedBool = Animator.StringToHash(FC.AnimatorKey.Grounded);
        colExtents = GetComponent<Collider>().bounds.extents;
    }

    //마우스 축이 0이냐 아니냐로 이동하는지를 체크
    public bool IsMoving()
    {
        //epsilon:실수가 가질수있는 가장 최소한의 값
        return Mathf.Abs(h) > Mathf.Epsilon || Mathf.Abs(v) > Mathf.Epsilon;
    }

    //수평 이동중인지 체크
    public bool IsHorizontalMoving()
    {
        return Mathf.Abs(h) > Mathf.Epsilon;
    }

    public bool CanSprint()
    {
        foreach (GeneriBehaviour behaviour in behaviours)
        {
            if (!behaviour.AllowSprint)
            {
                return false;
            }

        }
        foreach (GeneriBehaviour generiBehaviour in overrideBehaviours)
        {
            if (!generiBehaviour.AllowSprint)
            {
                return false;
            }
        }
        return true;
    }

    //달리고있는지 체크
    public bool IsSprinting()
    {
        return sprint && IsMoving() && CanSprint();
    }

    public bool IsGrounded()
    {
        //내 위치에서 충돌체 크기만큼 아래로 쏴서 걸리는게 있으면 땅에 있는 거고 없으면 공중에 떠 있다.
        Ray ray = new Ray(myTransform.position + Vector3.up * 2 * colExtents.x, Vector3.down);
        return Physics.SphereCast(ray, colExtents.x, colExtents.x + 0.2f);
    }

    private void Update()
    {

        if (!Inventory.inventoryActivated) { 

            h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        myAnimator.SetFloat(hFloat, h, 0.1f, Time.deltaTime);
        myAnimator.SetFloat(vFloat, v, 0.1f, Time.deltaTime);

        sprint = Input.GetButton(ButtonName.Sprint);

        //달리기중이면 fov를 바꾸고 달리기가 끝나면 fov를 다시 원래대로 돌아와라
        if ((IsSprinting()))
        {
            changedFOV = true;
            camScript.SetFOV(sprintFOV);
        }
        else if (changedFOV)
        {
            camScript.ResetFOV();
            changedFOV = false;
        }


        myAnimator.SetBool(groundedBool, IsGrounded());
        }
    }

    //캐릭터 틀어지는것을 방지해서 보정 해줌
    public void Repositioning()
    {
        if (lastDirection != Vector3.zero)
        {
            //3인칭에서 y값은 무조건 없앰
            lastDirection.y = 0f;
            Quaternion targetRotation = Quaternion.LookRotation(lastDirection);
            Quaternion newRotation = Quaternion.Slerp(myRigidbody.rotation, targetRotation, turnSmoothing);
            myRigidbody.MoveRotation(newRotation);

        }
    }

    private void FixedUpdate()
    {
        bool isAnyBehaviourActive = false;

        //잠긴 동작이나 오버라이딩 동작이 없으면
        if (behaviourLocked > 0 || overrideBehaviours.Count == 0)
        {

            foreach (GeneriBehaviour behaviour in behaviours)
            {
                //다른 동작들을 업데이트하면 안되니까
                if (behaviour.isActiveAndEnabled && currentBehaviour == behaviour.GetBehaviourCode)
                {
                    //기본 동작을 업데이트
                    isAnyBehaviourActive = true;
                    behaviour.LocalFixedUpdate();
                }
            }
        }
        // 잠긴 동작이나 오버라이딩 동작(대체동작)이 있다면
        else
        {
            foreach (GeneriBehaviour behaviour in overrideBehaviours)
            {
                //대체동작을 업데이트 
                behaviour.LocalFixedUpdate();
            }
        }
        //활성화된게 없었고 대체동작이 없다며
        if (!isAnyBehaviourActive && overrideBehaviours.Count == 0)
        {
            //땅에 붙여놓고 리포지션
            myRigidbody.useGravity = true;
            Repositioning();
        }
    }

    //update 돌고난 다음에 lateupdate(플레이어 이동하고 카메라 이동)
    private void LateUpdate()
    {
        if (behaviourLocked > 0 || overrideBehaviours.Count == 0)
        {

            foreach (GeneriBehaviour behaviour in behaviours)
            {
                //다른 동작들을 업데이트하면 안되니까
                if (behaviour.isActiveAndEnabled && currentBehaviour == behaviour.GetBehaviourCode)
                {
                    //기본 동작을 업데이트
                    behaviour.LocalLateUpdate();
                }
            }
        }
        else
        {
            foreach (GeneriBehaviour behaviour in overrideBehaviours)
            {
                behaviour.LocalLateUpdate();
            }
        }
    }

    //동작 등록
    public void SubScribeBehaviour(GeneriBehaviour behaviour)
    {
        behaviours.Add(behaviour);
    }

    //기본동작
    public void RegisterDefaultBehaviour(int behaviourCode)
    {
        defaultBehaviour = behaviourCode;
        currentBehaviour = behaviourCode;

    }

    //바꾸길 원하는 동작
    public void RegisterBehaviour(int behaviourCode)
    {
        if(currentBehaviour == defaultBehaviour)
        {
            currentBehaviour = behaviourCode;
        }
    }

    //동작 해제
    public void UnRegisterBehaviour(int behaviourCode)
    {
        if(currentBehaviour == behaviourCode)
        {
            currentBehaviour = defaultBehaviour;
        }
    }

    public bool OverrideWithBehaviour(GeneriBehaviour behaviour)
    {
        if (!overrideBehaviours.Contains(behaviour))
        {
            if(overrideBehaviours.Count == 0)
            {
                foreach(GeneriBehaviour behaviour1 in behaviours)
                {
                    if(behaviour1.isActiveAndEnabled &&  currentBehaviour == behaviour1.GetBehaviourCode)
                    {
                        behaviour1.OnOverride();
                        break;
                    }
                }
            }
            overrideBehaviours.Add(behaviour);
            return true;
        }
        return false;
    }

    public bool RevokeOverridingBehaviour(GeneriBehaviour behaviour)
    {
        if (overrideBehaviours.Contains(behaviour))
        {
            overrideBehaviours.Remove(behaviour);
            return true;
        }
        return false;
    }

    public bool IsOverriding(GeneriBehaviour behaviour = null)
    {
        if(behaviour == null)
        {
            return overrideBehaviours.Count > 0;
        }
        return overrideBehaviours.Contains(behaviour);
    }

    //현재동작이냐?
    public bool IsCurrentBehaviour(int behaviourCode)
    {
        return this.currentBehaviour == behaviourCode;
    }

    public bool GetTempLockStatus(int behaviourCode = 0)
    {
        return (behaviourLocked != 0 && behaviourLocked != behaviourCode);
    }

    //잠시 동작을 잠그는 함수
    public void LockTempBehaviour(int behaviourCode)
    {
        if(behaviourLocked== 0)
        {
            behaviourLocked = behaviourCode;
        }
    }

    public void UnLockTempBehaivour(int behaviourCode)
    {
        if(behaviourLocked == behaviourCode)
        {
            behaviourLocked = 0;
        }
    }

    public Vector3 GetLasDirection()
    {
        return lastDirection;
    }

    public void SetLastDirection(Vector3 direction)
    {
        lastDirection = direction;
    }


}

public abstract class GeneriBehaviour : MonoBehaviour
{
    protected int speedFloat;
    protected behaviourController behaviourController;
    protected int behaviourCode;
    protected bool canSprint;

    private void Awake()
    {
        this.behaviourController = GetComponent<behaviourController>();
        speedFloat = Animator.StringToHash(FC.AnimatorKey.Speed);
        canSprint = true;
        
        //동작 타입을 해시코드로 가지고 있다가 추후에 구별용으로 사용.
        behaviourCode = this.GetType().GetHashCode();
    }
    public int GetBehaviourCode
    {
        get => behaviourCode;
    }

    public bool AllowSprint
    {
        get => canSprint;
    }

    public virtual void LocalLateUpdate()
    {

    }

    public virtual void LocalFixedUpdate()
    {

    }

    public virtual void OnOverride()
    {

    }

}
