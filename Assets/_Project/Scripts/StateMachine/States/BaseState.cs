using Psychonaut;
using UnityEngine;

namespace Psychonaut
{
    public abstract class BaseState : IState
    {
        protected readonly PlayerController player;
        protected readonly Animator animator;

        protected static readonly int LocomotionHash = Animator.StringToHash("Locomotion");
        protected static readonly int JumpHash = Animator.StringToHash("Jump");
        protected static readonly int InteractHash = Animator.StringToHash("Interact");
        protected static readonly int NoInteractHash = Animator.StringToHash("NoInteract");

        // time blending between animations
        protected const float crossFadeDuration = 0.1f;

        protected BaseState(PlayerController player, Animator animator)
        {
            this.player = player;
            this.animator = animator;
        }

        public virtual void FixedUpdate()
        {
            // noop
        }

        public virtual void OnEnter()
        {
            // noop
        }

        public virtual void OnExit()
        {
            Debug.Log("BaseState.OnExit");
        }

        public virtual void Update()
        {
            // noop
        }
    }

}
