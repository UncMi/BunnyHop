using UnityEngine;

namespace Psychonaut
{
    public class LocomotionState : BaseState
    {
        public LocomotionState(PlayerController player, Animator animator) : base(player, animator)
        {
        }
        public override void OnEnter()
        {
            Debug.Log("LocomotionState.OnEnter");
            animator.CrossFade(LocomotionHash, crossFadeDuration);
        }
        public override void OnExit()
        {
            Debug.Log("LocomotionState.OnExit");
            animator.CrossFade(LocomotionHash, crossFadeDuration);
        }
        public override void FixedUpdate()
        {
            player.HandleMovement();
        }
    }
}
