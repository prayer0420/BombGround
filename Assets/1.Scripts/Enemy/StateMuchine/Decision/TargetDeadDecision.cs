using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 타겟이 죽었는지 체크..
/// </summary>
[CreateAssetMenu(menuName = "PluggableAI/Decisions/Target Dead")]

public class TargetDeadDecision : Decision
{
    public override bool Decide(StateController controller)
    {
        try
        {
            return controller.aimTarget.root.GetComponent<HealthBase>().IsDead; //죽었으면 true반환
        }
        catch (UnassignedReferenceException)
        {
            Debug.LogError("생명력관리 컴퍼넌트 HealthBase를 붙여주세요~" + controller.name, controller.gameObject);

        }
        return false;
    }
}
