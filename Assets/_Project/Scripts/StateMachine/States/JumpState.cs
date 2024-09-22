﻿using Psychonaut;
using UnityEngine;


namespace Psychonaut
{
        public class JumpState : BaseState
        {
            public JumpState(PlayerController player, Animator animator) : base(player, animator)
            {
            }
            public override void OnEnter()
            {
                Debug.Log("JumpState.OnEnter");
                animator.CrossFade(JumpHash, crossFadeDuration);
            }
            public override void FixedUpdate()
            {
                player.HandleJump();
                player.HandleMovement();
            }
        }
}

