using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 타겟이 보이거나 붙어있으면 교전 대기시간을 초기화 하고
/// 보이지않거나 멀어져 있거나 하면 blindEngageTime 만큼 기다릴건지 판단
/// </summary>
[CreateAssetMenu(menuName = "PluggableAI/Decisions/Engage")]
public class EngageDecision : Decision
{

    [Header("Extra Deicisions")]

    public LookDecision isViewing;  //보이는 중이니
    public FocusDecision targetNear; //가까이 있니?


    public override bool Decide(StateController controller)
    {
        //보이거나 가까이 있다면
        if(isViewing.Decide(controller) || targetNear.Decide(controller))
        {
            //플레이어가 사라졌을때부터 인지하고 있는 시간을 초기화
            controller.variables.blindEngageTimer = 0;

        }

        //플레이어가 사라졌을때 찾아야하는 시간보다 플레이어가 사라졌을때부터 인지하고 있는시간을 넘겼을 경우
        else if (controller.variables.blindEngageTimer >= controller.blindEngageTime)
        {
            //플레이어가 사라졌을때부터 인지하고 있는 시간을 초기화
            controller.variables.blindEngageTimer = 0;
            return false;
        }
        return true;
    }
}
