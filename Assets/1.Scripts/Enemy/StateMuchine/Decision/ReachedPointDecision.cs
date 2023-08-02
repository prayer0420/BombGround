using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// navmeshagent에서 남은 거리가 멈추는 중일 정도로 얼마 남지않았거나, 경로를 검색중이 아니라면 true
/// </summary>

[CreateAssetMenu(menuName = "PluggableAI/Decisions/Reached Point")]

public class ReachedPointDecision : Decision
{
    public override bool Decide(StateController controller)
    {
        //
        if (Application.isPlaying == false)
        {
            return false;
        }
        //남은게 멈춰있는것보다 작고(거의도착했다는뜻), 길찾기 대기중이 아니라면
        if (controller.nav.remainingDistance - controller.nav.stoppingDistance < 0.01f && !controller.nav.pathPending)
        {
            Debug.Log(controller.nav.remainingDistance);
            Debug.Log(controller.nav.stoppingDistance);
            Debug.Log("도착?");
            return true;
        }
        else
        {
            return false;
        }

    }
}

