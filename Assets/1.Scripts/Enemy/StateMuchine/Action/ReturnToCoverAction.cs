using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 갈만한 엄폐물을 찾았다는 가정 하에 진행되는 Action
/// 타겟팅하는것 멈추고 장애물로 돌아감
/// </summary>
[CreateAssetMenu(menuName = "PluggableAI/Actions/Return To Cover")]

public class ReturnToCoverAction : Action
{
    public override void OnReadyAction(StateController controller)
    {
        if(!Equals(controller.CoverSpot, Vector3.positiveInfinity))
        {
            controller.nav.destination = controller.CoverSpot; //장애물로 이동
            controller.nav.speed = controller.generalStats.chaseSpeed;
            //약간의 간격이있다면
            if (Vector3.Distance(controller.CoverSpot, controller.transform.position) > 0.5f)
            {
                controller.enemyAnimation.AbortPendingAim(); //사격하던것 멈추기
            }
        }
        else
        {
            controller.nav.destination = controller.transform.position;
        }
    }

    public override void Act(StateController controller)
    {
        if(!Equals(controller.CoverSpot, controller.transform.position))
        {
            controller.focusSight = false;
        }
    }

}
