using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBeahviour : StateMachineBehaviour
{
    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    public Soldier unit;
    public Character attacking;
    Animator animator;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        unit = animator.GetComponent<Soldier>();
        attacking = GameManager.findClosestEnemy(unit);
        CodeExecutor.onStepActions.Add(OnStep);
        this.animator = animator;
    }

    void OnStep()
    {
       /* if (attacking != null)
            unit.attack(attacking);
        else
            animator.SetBool("Attack", false);
*/
    }

    //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }

    //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        CodeExecutor.onStepActions.Remove(OnStep);

    }

    //OnStateMove is called right after Animator.OnAnimatorMove()
    override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("updated");
        
    }

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
