using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 공격과 동시에 이동하는 액션이며, 일단 회전할때는 회전을 하고 회전을 다했으면
/// strafing이 활성화됩니다.
/// </summary>

[CreateAssetMenu(menuName = "PluggableAI/Actions/Focus Move")]
public class FocusMoveAction : Action
{
    public ClearShotDecision claerShotDecision;

    private Vector3 currentDest; //현재 이동하고자 하는 목표
    private bool aligned; //타겟과 정렬이 되었냐

    public override void OnReadyAction(StateController controller)
    {
        controller.hadClearShot = controller.haveClearShot = false;
        currentDest = controller.nav.destination;
        controller.focusSight = true; //현재 타겟을 잡고있는 상태
        aligned = false;
    }

    public override void Act(StateController controller)
    {
        if(!aligned)
        {
            controller.nav.destination = controller.personalTarget; //캐릭터정면으로 바라보기
            controller.nav.speed = 0; //일단 서있기

            //회전할 속도가 안나면(플레이어를 정면으로 바라보고 있다면)
            if(controller.enemyAnimation.angularSpeed == 0f)
            {
                controller.Strafing = true;
                aligned = true;
                controller.nav.destination = currentDest;
                controller.nav.speed = controller.generalStats.evadeSpeed;
                Debug.Log("스트래핑됐다");
            }
        }
        else
        {
            controller.haveClearShot = claerShotDecision.Decide(controller); //쏠 수있는 상태가 됐다면
            if(controller.hadClearShot != controller.haveClearShot) //전에 못쐈었는데 지금은 사격이 가능해졌다면
            {
                //지금 쏠 수있는 상태로 만들어주고
                controller.Aimimg = controller.haveClearShot;
                //사격이 가능하고 현재 이동목표가 엄폐물과 다를때, 일단 이동하지 말아라
                if(controller.haveClearShot && !Equals(currentDest, controller.CoverSpot))
                {
                    controller.nav.destination = controller.transform.position;
                }
            }
            controller.hadClearShot = controller.haveClearShot;
        }
    }


}
