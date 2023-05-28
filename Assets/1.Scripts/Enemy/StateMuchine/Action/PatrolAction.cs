using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FC;
/// <summary>
/// Waypoint를 지정한다.(갯수는 ?)
/// 지정한 waypoint를 돈다
/// 한 waypoint에 도착할때마다 대기시간동안 대기를 하고 다음 waypoint로 넘어간다
/// </summary>

[CreateAssetMenu(menuName = "PluggableAI/Actions/Patrol")]

public class PatrolAction : Action
{
    public override void OnReadyAction(StateController controller)
    {
        controller.enemyAnimation.AbortPendingAim(); //조준중이였다면 취소
        controller.enemyAnimation.anim.SetBool(AnimatorKey.Crouch, false); //숨어있었다면 취소
        controller.personalTarget = Vector3.positiveInfinity; //정찰중엔은 타겟도없고
        controller.CoverSpot = Vector3.positiveInfinity; //정찰중엔은 엄폐물도 상관이 없음
    }

    private void Patrol(StateController controller)
    {
        if (controller.patrolWaypoints.Count == 0)
        {
            return;
        }
        controller.focusSight = false;
        controller.nav.speed = controller.generalStats.patrolSpeed;
        //남아있는거리가 멈춰있는 거리보다 작고 , 경로찾는중이아니라면 == 멈췄다면
        if(controller.nav.remainingDistance<=controller.nav.stoppingDistance && !controller.nav.pathPending)
        {
            controller.variables.patrolTimer += Time.deltaTime;

            if(controller.variables.patrolTimer >= controller.generalStats.partolWaitTime)
            {
                //계속 순환하도록
                controller.wayPointIndex = (controller.wayPointIndex + 1) % controller.patrolWaypoints.Count;
                controller.variables.patrolTimer = 0;
            }
        }

        try
        {
            controller.nav.destination = controller.patrolWaypoints[controller.wayPointIndex].position;
        }
        catch(UnassignedReferenceException)
        {
            Debug.LogWarning("웨이포인트가 없어요. 셋팅해주세요!", controller.gameObject);

            //내위치를 넣어줌
            controller.patrolWaypoints = new List<Transform>
            {
                controller.transform
            };
            controller.nav.destination = controller.transform.position;
        }
    }


    public override void Act(StateController controller)
    {
        Patrol(controller);
    }

}
