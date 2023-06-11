using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 타겟이 시야가 막히지 않은 상태에서 타겟이 시야각 (1/2) 사이에 있다면..
/// </summary>
[CreateAssetMenu(menuName ="PluggableAI/Decisions/Look")]

public class LookDecision : Decision
{
    private bool MyHandleTargets(StateController controller, bool hasTarget, Collider[] targetsInRadius)
    {
        if (hasTarget)
        {
            Vector3 target = targetsInRadius[0].transform.position; //플레이어의 위치
            Vector3 dirToTarget = target - controller.transform.position;
            //시야각의 반 만큼보다도 타겟이 더 안쪽에 있는지
            bool inFOVCondition = (Vector3.Angle(controller.transform.forward, dirToTarget) < controller.viewAngle / 2);
            if(inFOVCondition && !controller.BlockedSight())
            {
                controller.targetInSight = true;
                controller.personalTarget = controller.aimTarget.position;
                Debug.Log("적이 시야각 1/2에 보였다!");
                return true;
            }
        }
        return false;
    }

    public override bool Decide(StateController controller)
    {
        controller.targetInSight = false; //결정하기전엔 꺼놓기
        return CheckTargetInRadius(controller, controller.viewRadius, MyHandleTargets);
    }
}
