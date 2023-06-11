using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 타겟이 멀리있고, 엄폐물에서 최소 한 타임정도는 공격을 기다린 후에 다음 장애물로 이동할지
/// </summary>

[CreateAssetMenu(menuName = "PluggableAI/Decisions/Advance Cover")]

public class AdvanceCoverDecision : Decision
{
    public int waitRounds = 1;

    [Header("Extra Decision")]
    [Tooltip("플레이어가 가까이 있는지 판단")]
    public FocusDecision targetNear;

    public override void OnEnableDecision(StateController controller)
    {
        controller.variables.waitRounds += 1;
        //장애물을 이동할지를 랜덤하게(스탯으로 조정) 판단
        controller.variables.advanceCoverDecision = Random.Range(0f, 1f) < controller.classStats.ChangeCoverChance / 100f;
    }

    public override bool Decide(StateController controller)
    {
        //기다릴시간은 기다리고
        if(controller.variables.waitRounds <= waitRounds)
        {
            return false;
        }

        controller.variables.waitRounds = 0;

        //다음장애물을 얻겠다! 가깝지 않아야한다.
        return controller.variables.advanceCoverDecision && !targetNear.Decide(controller);
    }


}
