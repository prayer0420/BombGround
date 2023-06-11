using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 특정지점 타겟팅
/// </summary>
[CreateAssetMenu(menuName = "PluggableAI/Actions/Spot Focus")]

public class SpotFocusAction : Action
{
    public override void Act(StateController controller)
    {
        controller.nav.destination = controller.personalTarget; //타겟지점만 바라보고있음
        controller.nav.speed = 0; //이동하진 않음

    }

}
