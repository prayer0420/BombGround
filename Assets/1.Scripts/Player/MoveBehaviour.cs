using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 이동과 점프 동작을 담당하는 컴포넌트
/// 충돌처리에 대한 기능도 포함
/// 기본 동작으로써 작동.
/// </summary>
public class MoveBehaviour : GeneriBehaviour
{

    public float walkSpeed = 0.15f;
    public float runSpeed = 1.0f;
    public float sprintSpeed = 2.0f;
    public float speedDampTime = 0.1f; //damp: 점프변화 부드럽게

    public float jumpHeight = 1.5f;
    public float jumpInertiaForce = 10.0f; //점프 관성
    public float speed, speedSeeker; //마우스 조절
    private int jumpBool; //애니메이터용
    private int groundBool; //애니메이터용 땅인지
    private bool jump; //점프 가능한 상태인지
    private bool isColliding; //충돌체크

    private CapsuleCollider capsuleCollider;
    private Transform myTransform;

    private void Start()
    {
        //캐싱
        myTransform = transform;
        capsuleCollider = GetComponent<CapsuleCollider>();
        jumpBool = Animator.StringToHash(FC.AnimatorKey.Jump);
        groundBool = Animator.StringToHash(FC.AnimatorKey.Grounded);
        behaviourController.GetAnimator.SetBool(groundBool, true); //처음엔 땅에 서있음

        //무브 컨트롤러를 기본동작으로 등록
        behaviourController.SubScribeBehaviour(this);
        behaviourController.RegisterDefaultBehaviour(this.behaviourCode);

        speedSeeker = runSpeed;
    }

    //이동의 기본은 회전!
    Vector3 Rotating(float horizontal, float vertical)
    {
        //카메라의 포워드 방향 
        Vector3 forward = behaviourController.playerCamera.TransformDirection(Vector3.forward);

        //모든 3d 이동은 y = 0
        forward.y = 0.0f;
        //정규화, 크기가 1인 벡터(방향)
        forward = forward.normalized;

        //직교하는 벡터
        Vector3 right = new Vector3(forward.z, 0.0f, -forward.x);

        //내가원하는 타겟방향
        Vector3 targetDirection = Vector3.zero;
        targetDirection = forward * vertical + right * horizontal;

        //어딘가 이동하려고 한다면
        if(behaviourController.IsMoving() && targetDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

            // 기존 회전에서 타겟 회전으로 회전 할때 보간을 이용해서 부드럽게 회전
            Quaternion newRotation = Quaternion.Slerp(behaviourController.GetRigidbody.rotation, targetRotation, behaviourController.turnSmoothing);

            behaviourController.GetRigidbody.MoveRotation(newRotation);
            behaviourController.SetLastDirection(targetDirection);
        }
        //가만히 서있거나 마지막 카메라 방향으로 회전중이면 (줄어들고있으면)
        if(!(Mathf.Abs(horizontal) >0.9f) || Mathf.Abs(vertical) > 0.9f)
        {
            behaviourController.Repositioning();
        }
        return targetDirection;
    }

    private void RemoveVerticalVelocity()
    {
        //Velocity에서 y만 없앰
        Vector3 horizontalVelocity = behaviourController.GetRigidbody.velocity;
        horizontalVelocity.y = 0.0f;
        behaviourController.GetRigidbody.velocity = horizontalVelocity;
    }
    
    //이동 관리
    void MovementManagement(float horizontal, float vertical)
    {
        if(behaviourController.IsGrounded())
        {
            //땅에 붙어있으면 중력을 켜줌
            behaviourController.GetRigidbody.useGravity = true;
        }
        //점프중이 아닌데도 y값이 0보다크다면 어디간에 껴있는 것
        else if (!behaviourController.GetAnimator.GetBool(jumpBool) && behaviourController.GetRigidbody.velocity.y > 0)
        {
            RemoveVerticalVelocity();
        }

        //회전
        Rotating(horizontal, vertical);
        Vector2 dir = new Vector2(horizontal, vertical);
        speed = Vector2.ClampMagnitude(dir, 1f).magnitude;
        speedSeeker += Input.GetAxis("Mouse ScrollWheel");
        speedSeeker = Mathf.Clamp(speedSeeker, walkSpeed, runSpeed);
        speed *= speedSeeker;

        if(behaviourController.IsSprinting())
        {
            speed = sprintSpeed;
        }
        behaviourController.GetAnimator.SetFloat(speedFloat, speed, speedDampTime, Time.deltaTime);
    }
    
    //점프 하기전 충돌 확인(경사면에서 미끄러지는 기능)
    private void OnCollisionStay(Collision collision)
    {
        isColliding = true;
        //GetContact(0) = 부딪힌것중 가장 첫번째가 0.1f보다 작다는것을 경사면이 부딪혔다는 뜻
        if (behaviourController.IsCurrentBehaviour(GetBehaviourCode) && collision.GetContact(0).normal.y <= 0.1f)
        {
            float vel = behaviourController.GetAnimator.velocity.magnitude;
            //미끄러지게 만듦
            Vector3 targentMove = Vector3.ProjectOnPlane(myTransform.forward, collision.GetContact(0).normal).normalized * vel;
            //미끄러지는 방향으로 쭉 미끄러지게 함
            behaviourController.GetRigidbody.AddForce(targentMove, ForceMode.VelocityChange);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        isColliding = false;
    }
    
    void JumpManagement()
    {
        //만약 점프를 할 수있는 상황인데, 점프중이 아니고 땅에 붙어있는 상황이면
        if(jump && !behaviourController.GetAnimator.GetBool(jumpBool) && behaviourController.IsGrounded())
        {
            //점프중에는 이동할수 없으니 이동동작을 잠그고
            behaviourController.LockTempBehaviour(behaviourCode);
            behaviourController.GetAnimator.SetBool(jumpBool, true);

            //스피드가 있으면
            if(behaviourController.GetAnimator.GetFloat(speedFloat) > 0.1f)
            {
                //마찰을 줄여줘야함
                capsuleCollider.material.dynamicFriction = 0f;
                capsuleCollider.material.staticFriction = 0f;
                RemoveVerticalVelocity();
                float velocity = 2f * Mathf.Abs(Physics.gravity.y) * jumpHeight;
                velocity = Mathf.Sqrt(velocity);
                behaviourController.GetRigidbody.AddForce(Vector3.up * velocity, ForceMode.VelocityChange);
            }
        } else if (behaviourController.GetAnimator.GetBool(jumpBool))
        {
            //땅에 붙어있지않고(비행중이고) 충돌중인게 없고, 현재 임시로 이동동작이 잠겨있다면
            if(!behaviourController.IsGrounded() && !isColliding && behaviourController.GetTempLockStatus())
            {
                //포물선을 그리며 앞으로 점프
                behaviourController.GetRigidbody.AddForce(myTransform.forward * jumpInertiaForce * Physics.gravity.magnitude *
                sprintSpeed, ForceMode.Acceleration);
            }
            //떨어지는 중이고, 땅에 내려온 순간이라면
            if(behaviourController.GetRigidbody.velocity.y <0f && behaviourController.IsGrounded())
            {
                
                behaviourController.GetAnimator.SetBool(groundBool, true);
                //마찰력 다시 넣기
                capsuleCollider.material.dynamicFriction = 0.6f;
                capsuleCollider.material.staticFriction = 0.6f;
                jump = false;
                behaviourController.GetAnimator.SetBool(jumpBool, false);
                behaviourController.UnLockTempBehaivour(this.behaviourCode);
            }
        }

    }

    private void Update()
    {
        //점프 할수 있는 상황에서 점프키를 눌렀냐
        if(!jump && Input.GetButtonDown(ButtonName.Jump) && behaviourController.IsCurrentBehaviour(this.behaviourCode) &&
            !behaviourController.IsOverriding())
        {
            jump = true;
        }
    }
     
    public override void LocalFixedUpdate()
    {
        MovementManagement(behaviourController.GetH, behaviourController.GetV);
        JumpManagement();
    }


}
