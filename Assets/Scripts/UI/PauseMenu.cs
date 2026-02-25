using TacticalDroneCommander.Core;
using TacticalDroneCommander.Core.Events;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace UI
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] private GameObject _panel;
        [SerializeField] private Button _resumeButton;
        [SerializeField] private Button _mainMenuButton;
        [SerializeField] private Button _exitButton;

        [Inject] private IGameStateMachine _gameStateMachine;
        [Inject] private IEventBus _eventBus;
        [Inject] private InputController _inputController;

        private void Start()
        {
            _resumeButton.onClick.AddListener(OnResumeClicked);
            _mainMenuButton.onClick.AddListener(OnMainMenuClicked);
            _exitButton.onClick.AddListener(OnExitClicked);

            _eventBus.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
            _inputController.OnEscapePressed += OnEscapePressed;

            _panel.SetActive(false);
        }

        private void OnDestroy()
        {
            _eventBus?.Unsubscribe<GameStateChangedEvent>(OnGameStateChanged);
            if (_inputController != null)
                _inputController.OnEscapePressed -= OnEscapePressed;
        }

        private void OnEscapePressed()
        {
            if (_gameStateMachine.IsInState(GameState.Wave) || _gameStateMachine.IsInState(GameState.Pause))
                _gameStateMachine.TogglePause();
        }

        private void OnGameStateChanged(GameStateChangedEvent evt)
        {
            _panel.SetActive(evt.NewState == GameState.Pause);
        }

        private void OnResumeClicked()
        {
            _gameStateMachine.TogglePause();
        }

        private void OnMainMenuClicked()
        {
            if (_gameStateMachine.IsInState(GameState.Pause))
                _gameStateMachine.TogglePause();

            _gameStateMachine.SwitchState(GameState.Pregame);
        }

        private void OnExitClicked()
        {
            Application.Quit();
        }
    }
}

