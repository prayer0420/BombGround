using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// transition, action을 갖고있고, 디지모드 갖고있음
/// 액트 함수 호출, 스테이트 바뀌면 호출
/// </summary>


[CreateAssetMenu(menuName = "PluggeableAI/State")]

public class State : ScriptableObject
{

    public Action[] actions;

    public Transition[] transitions;

    //gizmo를 이용해 적의 state를 표현
    public Color sceneGizmoColor = Color.gray;


    public void DoActions(StateController controller)
    {
        //액션들 하나하나를 호출
        for (int i = 0; i < actions.Length; i++)
        {
            actions[i].Act(controller);
        }
    }

    //state가 바뀔때 준비해라~
    //state갖고있는 action들의 onreayaction다 호출, 각 transtion들의 다 호출../ state가 바뀔 때 호출
    public void OnEnableActions(StateController controller)
    {
        for (int i = 0; i < actions.Length; i++)
        {
            actions[i].OnReadyAction(controller);
        }
        for (int i = transitions.Length; i>=0; i--)
        {
            transitions[i].decision.OnEnableDecision(controller);
        }
    }


    public void CheckTransitions(StateController controller)
    {
        for (int i = 0; i < transitions.Length; i++)
        {
            bool decision = transitions[i].decision.Decide(controller);
            if(decision)
            {
                //transitions[i].decision에 의해서 transitions[i].trueState로 넘어가라
                controller.TransitionToState(transitions[i].trueState, transitions[i].decision);
            }
            else
            {
                controller.TransitionToState(transitions[i].falseState, transitions[i].decision);
            }
            
            //스테이트가 바꼈으면 
            if(controller.currentState != this)
            {
                controller.currentState.OnEnableActions(controller);
                break;
            }
        }
    }
}
