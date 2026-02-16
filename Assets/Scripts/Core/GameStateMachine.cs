using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using TacticalDroneCommander.Core.States;
using TacticalDroneCommander.Core.Events;

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
        private IGameState _currentStateHandler;
        private GameState _stateBeforePause;
        private CancellationTokenSource _stateCancellationTokenSource;
        
        public GameStateMachine(IEventBus eventBus, GameConfig config)
        {
            _eventBus = eventBus;
            _stateCancellationTokenSource = new CancellationTokenSource();
            
            _states = new Dictionary<GameState, IGameState>
            {
                { GameState.Pregame, new PregameState() },
                { GameState.Wave, new WaveState() },
                { GameState.Postwave, new PostwaveState(config, this) },
                { GameState.GameOver, new GameOverState() }
            };
            
            CurrentState = GameState.Pregame;
            _currentStateHandler = _states[CurrentState];
            
            _eventBus.Subscribe<WaveCompletedEvent>(OnWaveCompleted);
            
            Debug.Log("GameStateMachine: Initialized with State Pattern");
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
                if (_stateBeforePause != GameState.Pause)
                {
                    SwitchState(_stateBeforePause);
                }
            }
            else
            {
                _stateBeforePause = CurrentState;
                Time.timeScale = 0f;
                CurrentState = GameState.Pause;
                Debug.Log("GameStateMachine: Paused");
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
