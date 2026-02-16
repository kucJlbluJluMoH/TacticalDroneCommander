using TacticalDroneCommander.Core;
using TacticalDroneCommander.Core.Events;
using TacticalDroneCommander.Infrastructure;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace UI
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private GameObject _hud;
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private Button _startButton;
        [SerializeField] private Button _exitButton;
        [Inject] private IGameStateMachine _gameStateMachine;
        [Inject] private IEventBus _eventBus;
        [Inject] private ISaveLoadService _saveLoadService;
        private void Start()
        {
            _startButton.onClick.AddListener(OnStartButtonClicked);
            _exitButton.onClick.AddListener(OnExitButtonClicked);
            _eventBus.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
            
            if (_gameStateMachine.CurrentState == GameState.Pregame)
            {
                Show();
            }
        }
        
        private void OnGameStateChanged(GameStateChangedEvent obj)
        {
            if (obj.NewState == GameState.Pregame)
            {
                Show();
            }
        }

        private void OnStartButtonClicked()
        {
            Debug.Log("Start button clicked! Starting game...");
            _gameStateMachine.SwitchState(GameState.Wave);
            _hud.SetActive(false);
        }
        
        private void OnExitButtonClicked()
        {
            Debug.Log("Exit button clicked! Exiting game...");
            Application.Quit();
        }

        public void Show()
        {
            int highScore = _saveLoadService.GetSaveData().highScore;
            Debug.Log($"MainMenu.Show(): Displaying high score: {highScore}"); 
            _scoreText.text = $"High Score: {highScore}";
            _hud.SetActive(true);
        }

    }
}
