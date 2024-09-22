using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Psychonaut
{
    public class NoInteractState : BaseState
    {
        public NoInteractState(PlayerController player, Animator animator) : base(player, animator)
        {
        }
        public override void OnEnter()
        {
            Debug.Log("NoInteractState.OnEnter");
            animator.CrossFade(NoInteractHash, crossFadeDuration);
        }
        public override void FixedUpdate()
        {
            player.HandleMovement();
        }
    }
}