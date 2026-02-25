using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;
using Controllers;
using TacticalDroneCommander.Core;
using TacticalDroneCommander.Core.Events;

namespace Gameplay
{
    public class PlayerInputHandler : MonoBehaviour
    {
        [Inject] private IPlayerDroneManager _droneManager;
        [Inject] private IGameStateMachine _gameStateMachine;

        private Camera _mainCamera;
        private InputSystem_Actions _inputActions;

        private void Awake()
        {
            _mainCamera = Camera.main;
            _inputActions = new InputSystem_Actions();
        }

        private void OnEnable()
        {
            _inputActions.Enable();
            _inputActions.Player.Attack.performed += OnAttackPerformed;
        }

        private void OnDisable()
        {
            _inputActions.Player.Attack.performed -= OnAttackPerformed;
            _inputActions.Disable();
        }

        private void OnAttackPerformed(InputAction.CallbackContext context)
        {
            if (!_gameStateMachine.IsInState(GameState.Wave))
                return;

            HandleLeftClick();
        }

        private void HandleLeftClick()
        {
            Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                PlayerDroneController droneController = hit.collider.GetComponent<PlayerDroneController>();

                if (droneController != null)
                {
                    _droneManager.SelectDrone(droneController);
                    Debug.Log("PlayerInputHandler: Drone selected");
                }
                else
                {
                    _droneManager.CommandSelectedDroneToPosition(hit.point);
                    Debug.Log($"PlayerInputHandler: Command to move to {hit.point}");
                }
            }
        }
    }
}
