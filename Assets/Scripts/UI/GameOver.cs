using TacticalDroneCommander.Core;
using TacticalDroneCommander.Core.Events;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class GameOver : MonoBehaviour
{
    [SerializeField] private GameObject hud;
    [SerializeField] private TextMeshProUGUI score;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button toMainMenuButton;
    [SerializeField] private Button exitButton;
    [Inject] private IGameStateMachine _gameStateMachine;
    [Inject] private EventBus _eventBus;
        
    private void Start()
    {
        restartButton.onClick.AddListener(OnRestartButtonClicked);
        exitButton.onClick.AddListener(OnExitButtonClicked);
        toMainMenuButton.onClick.AddListener(OnMenuButtonClicked);
        _eventBus.Subscribe<GameOverEvent>(OnGameOver);

        hud.SetActive(false);
    }

    private void OnGameOver(GameOverEvent playerWin)
    {
        Debug.Log("GameOver: Game over event received, showing game over screen...");
        Show(playerWin.PlayerWon,playerWin.WaveNumber);
    }

    private void OnRestartButtonClicked()
    {
        Debug.Log("Restart button clicked! Starting game...");
        _gameStateMachine.SwitchState(GameState.Wave);
        hud.SetActive(false);
    }
    
    private void OnMenuButtonClicked()
    {
        Debug.Log("Main Menu button clicked! Returning to main menu...");
        _gameStateMachine.SwitchState(GameState.Pregame);
        hud.SetActive(false);
    }

    private void OnExitButtonClicked()
    {
        Debug.Log("Exit button clicked! Exiting game...");
        Application.Quit();
    }

    private void Show(bool isWin,int num)
    {
        score.text = $"Wave: {num}";
        title.text = isWin ? "Mission Accomplished!" : "Mission Failed!";
        hud.SetActive(true);
    }
}
