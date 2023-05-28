using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 랜덤하게 정해진 시간만큼 기다렸는가?
/// </summary>

[CreateAssetMenu(menuName = "PluggableAI/Decisions/Waited")]

public class WaitedDecision : Decision
{
    public float maxTimeToWait; //최대로 기다린 시간
    private float timeToWait; //기다린시간
    private float startTime; //기다리기 시작한 시간

    public override void OnEnableDecision(StateController controller)
    {
        timeToWait = Random.Range(0, maxTimeToWait);    //랜덤하게 만들어진 시간만큼 기다렸다가
        startTime = Time.time; //시작한 현재 시간

    }

    public override bool Decide(StateController controller)
    {
        return(Time.time - startTime) >= timeToWait; //게임이 시작된시간에 기다리기 시작한 시간을 뺀 시간이 기다린시간보다 크다면 충분히 기다렸다는 뜻
    }

}
