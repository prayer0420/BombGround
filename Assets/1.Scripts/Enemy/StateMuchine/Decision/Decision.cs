using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 조건 체크하는 클래스
/// 조건 체크를 위해 특정 위치로부터 원하는 검색 반경에 있는 충돌체를 찾아서 그 안에 타겟이 있는지 확인 
/// 시야 각도, 거리, 들을수있는 범위 등을 체크
/// </summary>
public abstract class Decision : ScriptableObject
{
    public abstract bool Decide(StateController controller);

    public virtual void OnEnableDecision(StateController controller)
    {

    }

    //타겟을 갖고있는지, 어느 반경안에있는 콜라이더를 받아옴
    public delegate bool HandleTargets(StateController controller, bool hasTargets, Collider[] targetInRadius);

    
    //특정위치에서 특정범위안에있는 타겟의 정보를 받아올수있음.
    public static bool CheckTargetInRadius(StateController controller, float radius, HandleTargets handleTargets)
    {
        //죽었으면 타겟은 내 타겟이 아님
        if(controller.aimTarget.root.GetComponent<HealthBase>().IsDead)
        {
            return false;
        }
        //컨트롤러의 위치 중심으로해서 내가 원하는 radius반경안에서 controller.generalStats.targetMask 를 넘기고 콜라이더를 얻어옴
        //handletarget에 넘김. targetsInRadius.Length > 0이라면 적 입장에서 플레이어가 있다는것.
        else
        {
            Collider[] targetsInRadius = Physics.OverlapSphere(controller.transform.position, radius, controller.generalStats.targetMask);
            return handleTargets(controller, targetsInRadius.Length > 0, targetsInRadius);
        }
    }
}
