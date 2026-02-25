using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using TacticalDroneCommander.Core.States;
using TacticalDroneCommander.Core.Events;
using TacticalDroneCommander.Infrastructure;

namespace TacticalDroneCommander.Core
{
    public enum GameState
    {
        Pregame,    // Before game starts (menu, setup)
        Wave,       // Active wave with enemies
        Postwave,    // Between waves (upgrades)
        Pause,      // Game paused
        GameOver    // Game ended
    }
    
    public interface IGameStateMachine
    {
        GameState CurrentState { get; }
        event Action<GameState> OnStateChanged;
        void SwitchState(GameState newState);
        bool IsInState(GameState state);
        void TogglePause();
        void Update();
        void Dispose();
    }
    
    public class GameStateMachine : IGameStateMachine, IDisposable
    {
        private readonly Dictionary<GameState, IGameState> _states;
        private readonly IEventBus _eventBus;
        private readonly ISaveLoadService _saveLoadService;
        private IGameState _currentStateHandler;
        private IGameState _stateHandlerBeforePause;
        private GameState _stateBeforePause;
        private CancellationTokenSource _stateCancellationTokenSource;
        private CancellationTokenSource _cancellationTokenSourceBeforePause;
        
        public GameStateMachine(IEventBus eventBus, GameConfig config, ISaveLoadService saveLoadService)
        {
            _eventBus = eventBus;
            _saveLoadService = saveLoadService;
            _stateCancellationTokenSource = new CancellationTokenSource();
            
            _states = new Dictionary<GameState, IGameState>
            {
                { GameState.Pregame, new PregameState() },
                { GameState.Wave, new WaveState() },
                { GameState.Postwave, new PostwaveState(config, this) },
                { GameState.GameOver, new GameOverState(_saveLoadService) }
            };
            
            CurrentState = GameState.Pregame;
            _currentStateHandler = _states[CurrentState];
            
            _eventBus.Subscribe<WaveCompletedEvent>(OnWaveCompleted);
            _eventBus.Subscribe<GameOverEvent>(OnGameOver);
            
            Debug.Log("GameStateMachine: Initialized with State Pattern");
        }
        private void OnGameOver(GameOverEvent evt)
        {
            Debug.Log($"GameStateMachine: Game Over event received (PlayerWon={evt.PlayerWon}, Wave={evt.WaveNumber}), transitioning to GameOver state");
            
            var gameOverState = _states[GameState.GameOver] as GameOverState;
            gameOverState?.SetGameOverData(evt.PlayerWon, evt.WaveNumber);
            
            SwitchState(GameState.GameOver);
        }

        public GameState CurrentState { get; private set; }
        
        public event Action<GameState> OnStateChanged;
        
        public void SwitchState(GameState newState)
        {
            if (CurrentState == newState)
            {
                Debug.LogWarning($"GameStateMachine: Already in state {newState}");
                return;
            }

            if (!_currentStateHandler.CanTransitionTo(newState))
            {
                Debug.LogError($"GameStateMachine: Invalid transition from {CurrentState} to {newState}");
                return;
            }

            var previousState = CurrentState;
            
            _currentStateHandler.Exit();
            
            _stateCancellationTokenSource?.Cancel();
            _stateCancellationTokenSource?.Dispose();
            _stateCancellationTokenSource = new CancellationTokenSource();
            
            CurrentState = newState;
            _currentStateHandler = _states[newState];
            _currentStateHandler.Enter(_stateCancellationTokenSource.Token);
            
            _eventBus.Publish(new GameStateChangedEvent(previousState, newState));
            OnStateChanged?.Invoke(CurrentState);
            
            Debug.Log($"GameStateMachine: Transitioned from {previousState} to {CurrentState}");
        }

        public void Update()
        {
            _currentStateHandler?.Update();
        }

        private void OnWaveCompleted(WaveCompletedEvent evt)
        {
            Debug.Log($"GameStateMachine: Wave {evt.WaveNumber} completed, transitioning to Postwave");
            SwitchState(GameState.Postwave);
        }
        
        public bool IsInState(GameState state)
        {
            return CurrentState == state;
        }

        public void TogglePause()
        {
            if (CurrentState == GameState.Pause)
            {
                if (_stateBeforePause == GameState.Pause)
                    return;

                var previousState = CurrentState;
                CurrentState = _stateBeforePause;
                _currentStateHandler = _stateHandlerBeforePause;
                _stateCancellationTokenSource = _cancellationTokenSourceBeforePause;
                Time.timeScale = 1f;
                Debug.Log($"GameStateMachine: Resumed to {CurrentState}");
                _eventBus.Publish(new GameStateChangedEvent(previousState, CurrentState));
                OnStateChanged?.Invoke(CurrentState);
            }
            else
            {
                _stateBeforePause = CurrentState;
                _stateHandlerBeforePause = _currentStateHandler;
                _cancellationTokenSourceBeforePause = _stateCancellationTokenSource;

                var previousState = CurrentState;
                CurrentState = GameState.Pause;
                Time.timeScale = 0f;
                Debug.Log("GameStateMachine: Paused");
                _eventBus.Publish(new GameStateChangedEvent(previousState, CurrentState));
                OnStateChanged?.Invoke(CurrentState);
            }
        }

        public void Dispose()
        {
            _stateCancellationTokenSource?.Cancel();
            _stateCancellationTokenSource?.Dispose();
            _currentStateHandler?.Exit();
            Debug.Log("GameStateMachine: Disposed");
        }
    }
}
