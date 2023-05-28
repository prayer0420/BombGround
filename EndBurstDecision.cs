using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 한번 사격이 시작되면 재장전 전까지 쏠수있는 총알의 수를 판단.
/// </summary>

[CreateAssetMenu(menuName = "PluggableAI/Decisions/End Burst")]

public class EndBurstDecision : Decision
{
    //쓸수있는 총알 갯수가 남아있냐

    public override bool Decide(StateController controller)
    {
        //사격을 많이 했으면 TRUE ,  재장전(WAIT)
        return controller.variables.currentShots >= controller.variables.shotsInRounds;
    }

    
}
