using System;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer.Unity;

namespace TacticalDroneCommander.Core
{
    public abstract class InputController : IDisposable
    {
        private InputSystem_Actions _inputActions;

        public event Action OnEscapePressed;
        public event Action OnAttackPressed;
        public event Action OnAttackReleased;

        public Vector2 MoveInput { get; private set; }
        public Vector2 LookInput { get; private set; }
        public bool IsAttacking { get; private set; }

        public InputController()
        {
            _inputActions = new InputSystem_Actions();

            _inputActions.Player.Escape.performed += OnEscape;
            _inputActions.Player.Attack.started += OnAttackStart;
            _inputActions.Player.Attack.canceled += OnAttackCancel;

            _inputActions.Enable();
        }

        private void OnEscape(InputAction.CallbackContext context)
        {
            OnEscapePressed?.Invoke();
        }

        private void OnAttackStart(InputAction.CallbackContext context)
        {
            IsAttacking = true;
            OnAttackPressed?.Invoke();
        }

        private void OnAttackCancel(InputAction.CallbackContext context)
        {
            IsAttacking = false;
            OnAttackReleased?.Invoke();
        }

        public void Dispose()
        {
            _inputActions.Player.Escape.performed -= OnEscape;
            _inputActions.Player.Attack.started -= OnAttackStart;
            _inputActions.Player.Attack.canceled -= OnAttackCancel;

            _inputActions.Disable();
            _inputActions.Dispose();
        }
    }
}