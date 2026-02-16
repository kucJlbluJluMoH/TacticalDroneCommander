using System.Threading;
using UnityEngine;
using TacticalDroneCommander.Infrastructure;

namespace TacticalDroneCommander.Core.States
{
    public class GameOverState : IGameState
    {
        public GameState StateType => GameState.GameOver;
        
        private readonly ISaveLoadService _saveLoadService;
        private bool _playerWon;
        private int _waveNumber;

        public GameOverState(ISaveLoadService saveLoadService)
        {
            _saveLoadService = saveLoadService;
        }

        public void SetGameOverData(bool playerWon, int waveNumber)
        {
            _playerWon = playerWon;
            _waveNumber = waveNumber;
        }

        public void Enter(CancellationToken cancellationToken)
        {
            Time.timeScale = 0f;
            Debug.Log($"GameOverState: Entered. PlayerWon={_playerWon}, Wave={_waveNumber}");
            
            _saveLoadService.UpdateHighScore(_waveNumber);
        }

        public void Exit()
        {
            Debug.Log("GameOverState: Exited.");
        }

        public void Update()
        {
        }

        public bool CanTransitionTo(GameState newState)
        {
            return newState == GameState.Pregame;
        }
    }
}
