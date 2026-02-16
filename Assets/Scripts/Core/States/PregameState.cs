using System.Threading;
using UnityEngine;

namespace TacticalDroneCommander.Core.States
{
    public class PregameState : IGameState
    {
        public GameState StateType => GameState.Pregame;

        public void Enter(CancellationToken cancellationToken)
        {
            Time.timeScale = 0f;
            Debug.Log("PregameState: Entered. Game paused, waiting for player to start.");
        }

        public void Exit()
        {
            Debug.Log("PregameState: Exited.");
        }

        public void Update()
        {
        }

        public bool CanTransitionTo(GameState newState)
        {
            return newState == GameState.Wave;
        }
    }
}

