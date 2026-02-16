using System.Threading;
using UnityEngine;

namespace TacticalDroneCommander.Core.States
{
    public class GameOverState : IGameState
    {
        public GameState StateType => GameState.GameOver;

        public void Enter(CancellationToken cancellationToken)
        {
            Time.timeScale = 0f;
            Debug.LogError("GameOverState: Entered. Game Over!");
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

