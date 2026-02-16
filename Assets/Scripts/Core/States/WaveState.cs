using System.Threading;
using UnityEngine;

namespace TacticalDroneCommander.Core.States
{
    public class WaveState : IGameState
    {
        public GameState StateType => GameState.Wave;

        public WaveState()
        {
        }

        public void Enter(CancellationToken cancellationToken)
        {
            Time.timeScale = 1f;
            Debug.Log("WaveState: Entered. Wave will be started by WaveManager listening to state change.");
        }

        public void Exit()
        {
            Debug.Log("WaveState: Exited.");
        }

        public void Update()
        {
        }

        public bool CanTransitionTo(GameState newState)
        {
            return newState == GameState.Postwave || newState == GameState.GameOver || newState == GameState.Pause;
        }
    }
}

