using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FC;
/// <summary>
/// 타겟이 있다면 타겟까지 이동하지만, 타겟을 잃었다면 가만히 서있습니다.
/// </summary>


[CreateAssetMenu(menuName = "PluggableAI/Actions/Search")]

public class SearchAction : Action
{

    //초기화

    public override void OnReadyAction(StateController controller)
    {
        //시야 안에 인지하고있었다면 풀기
        controller.focusSight = false;
        //조준 풀기
        controller.enemyAnimation.AbortPendingAim();
        //엄폐물에 숨어있으면 풀기
        controller.enemyAnimation.anim.SetBool(AnimatorKey.Crouch, false);
        //장애물도 필요없음
        controller.CoverSpot = Vector3.positiveInfinity;

    }

    public override void Act(StateController controller)
    {
        //타겟을 잃었다면
        if(Equals(controller.personalTarget, Vector3.positiveInfinity))
        {
            //가만히 서있기
            controller.nav.destination = controller.transform.position;
        }
        else
        {
            //쫓아가는 속도값으로 바꿔줌
            controller.nav.speed = controller.generalStats.chaseSpeed;
            controller.nav.destination = controller.personalTarget;
        }
    }

}
