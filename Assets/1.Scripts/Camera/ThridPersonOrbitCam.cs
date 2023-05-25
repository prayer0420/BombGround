using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//카메라 속성 중 중요 속성 하나는 카메라로 부터 위치 오프셋 벡터, 피봇 오프셋 벡터
//위치 오프셋 벡터는 충돌 처리용으로 사용하고, 피봇 오프셋 벡터는 시선이동에 사용하도록
//충돌체크 : 이동 충돌 체크 기능(캐릭터->카메라 / 카메라->캐릭터)
//사격 반동(recoil)을 위한 기능
//FOV(시야각)변경 기능

//[RequireComponent](typeof(Camera))]
public class ThridPersonOrbitCam : MonoBehaviour
{
    public Transform player; //player transform

    public Vector3 pivotOffset = new Vector3(0.0f, 1.0f, 0.0f); // 살짝 위로 바라봄

    public Vector3 camOffset = new Vector3(0.4f, 0.5f, -2.0f); // 카메라 위치 어깨부분

    public float smooth = 10f; // 카메라 반응 속도

    public float horizontalAimingSpeed = 6.0f; // 카메라의 수평 회전 속도(에이밍 조절 속도)

    public float verticalAimingSpeed = 6.0f; //카메라의 수직 회전 속도

    public float maxVerticalAngle = 30.0f;  // 카메라의 수직 회전 각도 최댓값
    public float minVerticalAngle = -60.0f; // 카메라의 수직 회전 각도 최솟값

    public float recoilAngleBound = 5.0f; // 사격 반동 바운스 값
    private float angleH = 0.0f; //마우스 이동에 따른 카메라 수평 이동 수치. 
    private float angleV = 0.0f; //마우스 이동에 따른 카메라 수직 이동 수치. 

    private Transform cameraTransform; //트랜스폼 캐싱
    private Camera myCamera;
    private Vector3 relCameraPos; //플레이어로부터 카메라까지의 벡터.
    private float relCameraPosMag; //플레이어로부터 카메라까지 사이의 거리.

    private Vector3 smoothPivotOffset; //카메라 피봇용 벡터(줌 확대 및 축소할 때 등)
    private Vector3 smoothCamOffset; //카메라 포인트 보간용 벡터(줌 확대 및 축소할 때 등)
    private Vector3 targetPivotOffset; //카메라 피봇용 보간용 벡터
    private Vector3 targetCamOffset; //카메라 포인트 보간용 벡터

    private float defaultFOV; //기본 시야값
    private float targetFOV; //타겟 시야값

    private float targetMaxVerticlaAngle; //타겟에 대한 카메라 수직 최대 각도(리콜)
    private float recoilAngle = 0.0f; //사격 반동 각도

    //프로퍼티를 이용해서 앵글값 가져오기
    public float GetH
    {
        //get => angleH;
        get
        {
            return angleH;
        }
    }

    private void Awake()
    {
        //캐싱
        cameraTransform = transform;
        myCamera = cameraTransform.GetComponent<Camera>();

        //카메라 기본 포지션 세팅
        cameraTransform.position = player.position + Quaternion.identity * pivotOffset + Quaternion.identity * camOffset;

        //identity : 회전을 주지 않음
        cameraTransform.rotation = Quaternion.identity;
        

        //카메라와 플레이어간의 상대 벡터, 충돌 체크에 사용하기 위함
        relCameraPos = cameraTransform.position - player.position;
        //플레이어를 제외한 상태에서 충돌체크하기 위해 0.5f를 빼줌
        relCameraPosMag = relCameraPos.magnitude - 0.5f;
        //기본 셋팅
        smoothPivotOffset = pivotOffset;
        smoothCamOffset = camOffset;
        defaultFOV = myCamera.fieldOfView;
        angleH = player.eulerAngles.y;

        ResetTargetOffsets();
        ResetFOV();
        ResetMaxVerticalAngle();
    }


    public void ResetTargetOffsets()
    {
        targetPivotOffset = pivotOffset;
        targetCamOffset = camOffset;
    }

    public void ResetFOV()
    {
        this.targetFOV = defaultFOV;
    }

    public void ResetMaxVerticalAngle()
    {
        targetMaxVerticlaAngle = maxVerticalAngle;
    }

    public void BounceVertical(float degrees)
    {
        recoilAngle = degrees;
    }

    public void SetTargetOffset(Vector3 newPivotOffset, Vector3 newCamOffset)
    {
        targetPivotOffset = newPivotOffset;
        targetCamOffset = newCamOffset;
    }

    public void SetFOV(float customFOV)
    {
        this.targetFOV = customFOV;
    }
    
    
    bool ViewingPosCheck(Vector3 checkPos, float deltaPlayerHeight)
    {
        //player.position은 발바닥이기 때문에, 추가 높이값을 따로 받아줘야함
        Vector3 target = player.position + (Vector3.up * deltaPlayerHeight);
        //checkPos지점부터 0.2radius방향으로 레이저를 발사해서 받아온 정보를 relcameraPosMag방향으로 쏨
        if(Physics.SphereCast(checkPos, 0.2f, target - checkPos, out RaycastHit hit, relCameraPosMag))
        {
            //플레이어가 아니고 트리거가 아닌것
            if (hit.transform != player && !hit.transform.GetComponent<Collider>().isTrigger)
            {
                return false;
            }

        }
        return true;
    }

    bool ReverseViewingPosCheck(Vector3 checkPos, float deltaPlayerHeight, float MaxDistance)
    {
        Vector3 origin = player.position + (Vector3.up * deltaPlayerHeight);
        if(Physics.SphereCast(origin, 0.2f, origin-checkPos, out RaycastHit hit, MaxDistance))
        {
            //플레이어가 아니고 자기자신도 아니고 트리거 아닌것
            if (hit.transform != player && hit.transform != transform && !hit.transform.GetComponent<Collider>().isTrigger)
            {
                return false;
            }
        }
        return true;
    }

    bool DoubleViewingPosCheck(Vector3 checkPos, float offset)
    {
        float playerFoucusHeight = player.GetComponent<CapsuleCollider>().height * 0.75f;
        return ViewingPosCheck(checkPos, playerFoucusHeight) &&
               ReverseViewingPosCheck(checkPos,playerFoucusHeight, offset);
    }

    private void Update()
    {
        //마우스 이동 값
        angleH += Mathf.Clamp(Input.GetAxis("Mouse X"), -1f, 1f) * horizontalAimingSpeed;
        angleV += Mathf.Clamp(Input.GetAxis("Mouse Y"), -1f, 1f) * verticalAimingSpeed;

        //카메라 수직 이동제한
        angleV = Mathf.Clamp(angleV, minVerticalAngle, targetMaxVerticlaAngle);

        //수직 카메라 반동
        angleV = Mathf.LerpAngle(angleV, angleV + recoilAngle, 10f * Time.deltaTime);

        //카메라 회전
        Quaternion camYRoation = Quaternion.Euler(0.0f, angleH, 0.0f);
        Quaternion aimRotation = Quaternion.Euler(-angleV, angleH, 0.0f);
        cameraTransform.rotation = aimRotation;

        //set FOV
        myCamera.fieldOfView = Mathf.Lerp(myCamera.fieldOfView, targetFOV, Time.deltaTime);

        Vector3 baseTempPosition = player.position + camYRoation * targetPivotOffset;
        //충돌되지 않을 때
        Vector3 noCollisionOffest = targetCamOffset; // 조준 할 때 카메라의 오프셋 값, 조준할때와 평소와 다르다
        
        //targetCamOffset 에서 aim으로 넘어갈때 충돌체크를 하고 넘어감
        for(float zOffset = targetCamOffset.z; zOffset <= 0f; zOffset += 0.5f)
        {
            noCollisionOffest.z = zOffset;
            //너무가까우면(zOffset값이 0)
            if (DoubleViewingPosCheck(baseTempPosition + aimRotation * noCollisionOffest,
                Mathf.Abs(zOffset)) || zOffset == 0f)
            {
                break;
            }
        }
        //Reposition Camera
        smoothPivotOffset = Vector3.Lerp(smoothPivotOffset, targetPivotOffset, smooth * Time.deltaTime);
        smoothCamOffset = Vector3.Lerp(smoothCamOffset, noCollisionOffest, smooth * Time.deltaTime);


        cameraTransform.position = player.position + camYRoation * smoothPivotOffset + aimRotation * smoothCamOffset;

        if(recoilAngle > 0.0f)
        {
            recoilAngle -= recoilAngleBound * Time.deltaTime;
        }
        else if(recoilAngle < 0.0f)
        {
            recoilAngle += recoilAngleBound * Time.deltaTime;
        }

    }


    public float GetCurrentPivotMagnitude(Vector3 finalPivotOffset)
    {
        return Mathf.Abs((finalPivotOffset - smoothPivotOffset).magnitude);
    }

}
