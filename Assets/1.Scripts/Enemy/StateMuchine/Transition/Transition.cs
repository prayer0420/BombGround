using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Transition
{
    //조건 체크를 위한 decision
    public Decision decision;
    public State trueState;
    public State falseState;
    
}
