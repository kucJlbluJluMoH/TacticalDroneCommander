using System.Collections.Generic;
using UnityEngine;
using TacticalDroneCommander.Core;
using TacticalDroneCommander.Core.Events;
using Entities;
using Controllers;

namespace Gameplay
{
    public interface IPlayerDroneManager
    {
        void SpawnInitialDrone();
        void SpawnDrone();
        void SelectDrone(PlayerDroneController drone);
        void CommandSelectedDroneToPosition(Vector3 position);
        PlayerDroneController GetSelectedDrone();
        List<PlayerEntity> GetAllPlayerDrones();
        void Initialize();
    }
    
    public class PlayerDroneManager : IPlayerDroneManager
    {
        private readonly IPlayerSpawner _playerSpawner;
        private readonly GameConfig _config;
        private readonly IEventBus _eventBus;
        
        private List<PlayerEntity> _playerDrones = new List<PlayerEntity>();
        private PlayerDroneController _selectedDrone;
        private int _droneCounter;
        
        public PlayerDroneManager(IPlayerSpawner playerSpawner, GameConfig config, IEventBus eventBus)
        {
            _playerSpawner = playerSpawner;
            _config = config;
            _eventBus = eventBus;
        }
        
        public void Initialize()
        {
            _eventBus.Subscribe<WaveCompletedEvent>(OnWaveCompleted);
            Debug.Log("PlayerDroneManager: Subscribed to WaveCompletedEvent");
        }
        
        private void OnWaveCompleted(WaveCompletedEvent evt)
        {
            Debug.Log($"PlayerDroneManager: Wave {evt.WaveNumber} completed. Spawning reward drone...");
            SpawnDrone();
        }
        
        public void SpawnInitialDrone()
        {
            SpawnDrone();
        }
        public void SpawnDrone()
        {
            var randomOffset = new Vector3(Random.Range(-2f, 2f), 0f, Random.Range(-2f, 2f));
            Vector3 spawnPosition = _config.BaseCoordinates + randomOffset;
            PlayerEntity drone = _playerSpawner.SpawnPlayer(spawnPosition);
            if (drone != null)
            {
                _playerDrones.Add(drone);
                _droneCounter++;
                
                if (_playerDrones.Count == 1)
                {
                    GameObject droneObject = drone.GetTransform().gameObject;
                    PlayerDroneController controller = droneObject.GetComponent<PlayerDroneController>();
                    if (controller != null)
                    {
                        SelectDrone(controller);
                    }
                }
                
                Debug.Log($"PlayerDroneManager: Spawned drone {_droneCounter}. Total drones: {_playerDrones.Count}");
            }
        }
        
        public void SelectDrone(PlayerDroneController drone)
        {
            if (_selectedDrone != null)
            {
                _selectedDrone.SetSelected(false);
            }
            
            _selectedDrone = drone;
            
            if (_selectedDrone != null)
            {
                _selectedDrone.SetSelected(true);
                Debug.Log("PlayerDroneManager: Drone selected");
            }
        }
        
        public void CommandSelectedDroneToPosition(Vector3 position)
        {
            if (_selectedDrone != null)
            {
                _selectedDrone.MoveToPosition(position);
            }
            else
            {
                Debug.LogWarning("PlayerDroneManager: No drone selected!");
            }
        }
        
        public PlayerDroneController GetSelectedDrone()
        {
            return _selectedDrone;
        }
        
        public List<PlayerEntity> GetAllPlayerDrones()
        {
            return _playerDrones;
        }
    }
}

