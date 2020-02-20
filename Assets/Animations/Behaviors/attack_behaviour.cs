using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class attack_behaviour : StateMachineBehaviour
{
    private Vector3 InitScale { get; set; }

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        InitScale = animator.gameObject.transform.localScale;
        if (animator.gameObject.GetComponent<Player>() != null && animator.GetInteger("direction") == GameRuler.DIRECTION_TOP)
            animator.gameObject.transform.localScale += InitScale * 0.4f;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.gameObject.GetComponent<Player>() != null && animator.GetInteger("direction") == GameRuler.DIRECTION_TOP)
            animator.gameObject.transform.localScale -= InitScale * 0.4f;
        animator.SetBool("attack", false);
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
