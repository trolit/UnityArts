﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBehaviour : StateMachineBehaviour {


    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

        animator.GetComponent<Character>().Attack = true;

        // player doesnt need it but 
        // leave the line outside of if 
        // statement so enemy can use it
        animator.SetFloat("speed", 0);

        if (animator.tag == "Player")
        {

            if (Player.Instance.OnGround)
            {
                Player.Instance.MyRigidbody.velocity = Vector2.zero;
            }
        }
	}

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{

    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // when done attacking (now both for enemy and player)
        animator.GetComponent<Character>().Attack = false;

        // disable Sword Collider
        animator.GetComponent<Character>().SwordCollider.enabled = false;

        animator.ResetTrigger("attack");

        
        if (animator.tag == "Player")
        {
            animator.ResetTrigger("soulfire");
        }
    }

	// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
	//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
	//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}
}
