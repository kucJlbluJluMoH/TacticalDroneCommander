using TacticalDroneCommander.Core;
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
        
        private void Start()
        {
            _startButton.onClick.AddListener(OnStartButtonClicked);
            _exitButton.onClick.AddListener(OnExitButtonClicked);
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
            _hud.SetActive(true);
        }

    }
}
