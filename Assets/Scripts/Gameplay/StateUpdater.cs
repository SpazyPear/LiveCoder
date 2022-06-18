using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateUpdater : ControlledMonoBehavour
{
    Animator[] stateMachines;

    private void Start()
    {
        stateMachines = GameObject.FindObjectsOfType<Animator>();
    }

    public override void OnStart()
    {
        base.OnStart();
        foreach (Animator stateMachine in stateMachines)
        {
            stateMachine.enabled = true;
        }
    }

    public override void OnStep()
    {
        base.OnStep();
        foreach (Animator stateMachine in stateMachines)
        {
            stateMachine.speed = 1.0f;
            stateMachine.Update(Time.deltaTime);
            stateMachine.speed = 0.0f;
        }
    }
}
