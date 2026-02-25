using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace TacticalDroneCommander.Core.States
{
    public class PostwaveState : IGameState
    {
        private readonly GameConfig _config;
        private readonly IGameStateMachine _stateMachine;
        private CancellationToken _cancellationToken;
        
        public GameState StateType => GameState.Postwave;

        public PostwaveState(GameConfig config, IGameStateMachine stateMachine)
        {
            _config = config;
            _stateMachine = stateMachine;
        }

        public void Enter(CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
            Time.timeScale = 1f;
            Debug.Log("PostwaveState: Entered. Waiting before next wave...");
            
            WaitForNextWave().Forget();
        }

        public void Exit()
        {
            Debug.Log("PostwaveState: Exited.");
        }

        public void Update()
        {
        }

        public bool CanTransitionTo(GameState newState)
        {
            return newState == GameState.Wave || newState == GameState.GameOver || newState == GameState.Pregame;
        }

        private async UniTaskVoid WaitForNextWave()
        {
            await UniTask.Delay((int)(_config.TimeBetweenWaves * 1000), cancellationToken: _cancellationToken);
            
            if (!_cancellationToken.IsCancellationRequested && _stateMachine.CurrentState == GameState.Postwave)
            {
                _stateMachine.SwitchState(GameState.Wave);
            }
        }
    }
}

