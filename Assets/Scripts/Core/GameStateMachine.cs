using System;
using UnityEngine;

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
    }
    
    public class GameStateMachine : IGameStateMachine
    {
        public GameStateMachine()
        {
            CurrentState = GameState.Pregame;
            Debug.Log("GameStateMachine: Initialized with state Pregame");
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

            ExitState(CurrentState);
            CurrentState = newState;
            EnterState(CurrentState);
            
            OnStateChanged?.Invoke(CurrentState);
            Debug.Log($"GameStateMachine: Switched to state {CurrentState}");
        }

        private void EnterState(GameState state)
        {
            switch (state)
            {
                case GameState.Pregame:
                    Time.timeScale = 1f;
                    Debug.Log("GameStateMachine: Entering Pregame state");
                    break;
                    
                case GameState.Wave:
                    Time.timeScale = 1f;
                    Debug.Log("GameStateMachine: Entering Wave state - enemies spawning!");
                    break;
                case GameState.Postwave:
                    Time.timeScale = 1f;
                    Debug.Log("GameStateMachine: Entering post Wave state - player can pick up upgrade");
                    break;
                case GameState.Pause:
                    Time.timeScale = 0f;
                    Debug.Log("GameStateMachine: Entering Pause state");
                    break;
                    
                case GameState.GameOver:
                    Time.timeScale = 0f;
                    Debug.Log("GameStateMachine: Entering GameOver state");
                    break;
            }
        }

        private void ExitState(GameState state)
        {
            switch (state)
            {
                case GameState.Pregame:
                    Debug.Log("GameStateMachine: Exiting Pregame state");
                    break;
                    
                case GameState.Wave:
                    Debug.Log("GameStateMachine: Exiting Wave state");
                    break;
                
                case GameState.Postwave:
                    Debug.Log("GameStateMachine: Exiting post Wave state");
                    break;
                
                case GameState.Pause:
                    Time.timeScale = 1f;
                    Debug.Log("GameStateMachine: Exiting Pause state");
                    break;
                    
                case GameState.GameOver:
                    Debug.Log("GameStateMachine: Exiting GameOver state");
                    break;
            }
        }

        public bool IsInState(GameState state)
        {
            return CurrentState == state;
        }

        public void TogglePause()
        {
            if (CurrentState == GameState.Pause)
            {
                SwitchState(GameState.Wave);
            }
            else if (CurrentState == GameState.Wave)
            {
                SwitchState(GameState.Pause);
            }
        }
    }
}

