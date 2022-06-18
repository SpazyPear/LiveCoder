using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseStateMachineBehaviour : StateMachineBehaviour
{
    Soldier soldier;
    Animator animator;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        soldier = animator.GetComponent<Soldier>();
        this.animator = animator;
        CodeExecutor.onStepActions.Add(OnStep);

    }

    public virtual void OnStep()
    {

    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
        CodeExecutor.onStepActions.Remove(OnStep);

    }
}
