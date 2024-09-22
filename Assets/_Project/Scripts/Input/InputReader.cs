using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

using static PlayerInputActions;


namespace Psychonaut
{
    [CreateAssetMenu(fileName = "InputReader", menuName = "Psychonaut/Inputreader")]
    public class InputReader : ScriptableObject, IPlayerActions
    {
        public event UnityAction<Vector2> Move = delegate { };
        public event UnityAction<Vector2, bool> Look = delegate { };
        public event UnityAction<bool> Jump = delegate { };
        public event UnityAction<bool> Interact = delegate { };
        public event UnityAction<bool> Interact2 = delegate { };
        public event UnityAction<bool> Drop = delegate { };
        public event UnityAction EnableMouseControlCamera = delegate { };
        public event UnityAction DisableMouseControlCamera = delegate { };

        PlayerInputActions inputActions;

        private static InputReader _instance;
        public static InputReader Instance
        {
            get
            {
                return _instance;
            }
        }
        private void Awake()
        {
            if (_instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                _instance = this;
            }
        }

        public Vector3 Direction => inputActions.Player.Move.ReadValue<Vector2>();

        void OnEnable()
        {
            if (inputActions == null)
            {
                inputActions = new PlayerInputActions();
                inputActions.Player.SetCallbacks(this);
            }
        }
        public void EnablePlayerActions()
        {
            inputActions.Enable();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            Move.Invoke(context.ReadValue<Vector2>());
        }


        public void OnJump(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    Jump.Invoke(true);
                    break;
                case InputActionPhase.Canceled:
                    Jump.Invoke(false);
                    break;
            }
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started)
            {
                Debug.Log("OnInteract called");
                Interact.Invoke(true); 
                Interact.Invoke(false); 
            }
        }
        public void OnInteractSecondary(InputAction.CallbackContext context)
        {
            Debug.Log("OnInteract2 called");
            Interact2.Invoke(true);
            Interact2.Invoke(false);
        }

        public void OnAim(InputAction.CallbackContext context)
        {
            //noop
        }

        public void OnFire(InputAction.CallbackContext context)
        {
            //noop
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            Look.Invoke(context.ReadValue<Vector2>(), IsDeviceMouse(context));
        }
        bool IsDeviceMouse(InputAction.CallbackContext context) => context.control.device.name == "Mouse";



        public void OnMouseControlCamera(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    EnableMouseControlCamera.Invoke();
                    break;
                case InputActionPhase.Canceled:
                    DisableMouseControlCamera.Invoke();
                    break;
            }
        }

        public Vector2 GetMouseDelta()
        {
            return inputActions.Player.Look.ReadValue<Vector2>();
        }

        public void OnRun(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        public void OnDrop(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}

