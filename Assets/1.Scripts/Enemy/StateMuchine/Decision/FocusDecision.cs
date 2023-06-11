using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 인지(sense)타입에 따라 특정거리로부터 가깝진 않지만, 시야는 막히지 않았지만 위험요소를 감지했거나
/// 너무 가까운거리에 타겟(플레이어)이 있는지 판단
/// </summary>

[CreateAssetMenu(menuName ="PluggableAI/Decisions/Focus")]

public class FocusDecision : Decision
{
    
    public enum Sense
    {
        NEAR,
        PERCEPTION,
        VIEW,
    }

    [Tooltip("어떤 크기로 위험요소 감지를 하겠습니까?")]
    public Sense sense;

    [Tooltip("현재 엄폐물을 해제 할까요?")]
    public bool invalidateCoverSpot;

    private float radius; //센서에 따른 범위

    //decision이 실행되기전에 초기화하는 함수(start같은 함수)
    public override void OnEnableDecision(StateController controller)
    {
        switch(sense)
        {
            case Sense.NEAR:
                radius = controller.nearRadius;
                break;
            case Sense.PERCEPTION:
                radius = controller.perceptionRadius;   
                break;
            case Sense.VIEW:
                radius = controller.viewRadius;
                break;
            default:
                radius = controller.nearRadius;
                break;
        }
    }

    //타겟이 있고 인지타입 반경크기 안에 시야가막히지 않았다면
    private bool MyHandleTargets(StateController controller, bool hasTarget, Collider[] targetsInHearRadius)
    {
        //타겟이 존재하고 시야가 막히지 않았다면
        if (hasTarget && !controller.BlockedSight())
        {
            if (invalidateCoverSpot)
            {
                controller.CoverSpot = Vector3.positiveInfinity; //엄폐{물 초기화

            }
            controller.targetInSight = true;
            controller.personalTarget = controller.aimTarget.position;
            Debug.Log("타겟쏠준비됐다");
            return true;
        }
        return false;
    }


    public override bool Decide(StateController controller)
    {

        //가깝지 않은 상태에서 경고를 느끼고 시야가 막히지 않았다면
        return (sense != Sense.NEAR && controller.variables.feelAlert && !controller.BlockedSight()) ||
                Decision.CheckTargetInRadius(controller, radius, MyHandleTargets);

    }
}
