using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Psychonaut
{
    public class InteractState : BaseState
    {
        public InteractState(PlayerController player, Animator animator) : base(player, animator)
        {
        }
        public override void OnEnter()
        {
            Debug.Log("InteractState.OnEnter");
            animator.CrossFade(InteractHash, crossFadeDuration);
        }
        public override void FixedUpdate()
        {
            player.HandleMovement();
        }
    }
}