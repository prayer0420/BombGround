using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FC;
using UnityEngine.UI;
/// <summary>
/// 숨을수 있는 엄폐물이 없다면 가만히 서있지만 새로운 엄폐물이 있고 엄폐물보다 가깝다면 엄폐물을 변경
/// 총알 장전도 해준다
/// </summary>


[CreateAssetMenu(menuName = "PluggableAI/Actions/FindCover")]

public class FindCoverAction : Action
{
    public FocusDecision targetNear;
    public override void OnReadyAction(StateController controller)
    {
        controller.focusSight = false;
        controller.enemyAnimation.AbortPendingAim();
        controller.enemyAnimation.anim.SetBool(AnimatorKey.Crouch, false);
        ArrayList nextCoverData = controller.coverLookUp.GetBestCoverSpot(controller); //가장 효율적인 엄폐물 찾아줌
        Vector3 potentialCover = (Vector3)nextCoverData[1];
        
        if(Vector3.Equals(potentialCover, Vector3.positiveInfinity)) //엄폐물이 없다면
        {
            controller.nav.destination = controller.transform.position; //일단 가만히 있기
            return;
        }
        //지금 장애물 위치보다 새로운 장애물위치가 더 가깝고, 주변에 장애물이 없다면
        else if((controller.personalTarget- potentialCover).sqrMagnitude < (controller.personalTarget- controller.CoverSpot).sqrMagnitude && !controller.IsNearOhterSpot(potentialCover, controller.nearRadius))
        {
            controller.coverHash = (int)nextCoverData[0];
            controller.CoverSpot = potentialCover;
        }
        controller.nav.destination = controller.CoverSpot; //엄폐물 위치로 이동
        controller.nav.speed = controller.generalStats.evadeSpeed;
        Debug.Log(controller.nav.destination);
        controller.variables.currentShots = controller.variables.shotsInRounds; //재장전
    }

    public override void Act(StateController controller)
    {
        
    }




}
