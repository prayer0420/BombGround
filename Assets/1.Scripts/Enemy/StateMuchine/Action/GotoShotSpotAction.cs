using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 특정지점으로 이동하라!
/// </summary>
[CreateAssetMenu(menuName = "PluggableAI/Actions/GoToShot Spot")]
public class GotoShotSpotAction : Action
{
    public override void OnReadyAction(StateController controller)
    {
        controller.focusSight = false;
        controller.nav.destination = controller.personalTarget;
        controller.nav.speed = controller.generalStats.chaseSpeed;
        controller.enemyAnimation.AbortPendingAim(); //조준중인것 끄기
    }

    public override void Act(StateController controller)
    {
    }

}
