using System.Collections.Generic;
using UnityEngine;
using TacticalDroneCommander.Core;
using Entities;
using Controllers;

namespace Gameplay
{
    public interface IPlayerDroneManager
    {
        void SpawnInitialDrone();
        void SpawnDrone(Vector3 position);
        void SelectDrone(PlayerDroneController drone);
        void CommandSelectedDroneToPosition(Vector3 position);
        PlayerDroneController GetSelectedDrone();
        List<PlayerEntity> GetAllPlayerDrones();
    }
    
    public class PlayerDroneManager : IPlayerDroneManager
    {
        private readonly IPlayerSpawner _playerSpawner;
        private readonly GameConfig _config;
        
        private List<PlayerEntity> _playerDrones = new List<PlayerEntity>();
        private PlayerDroneController _selectedDrone;
        private int _droneCounter;
        
        public PlayerDroneManager(IPlayerSpawner playerSpawner, GameConfig config)
        {
            _playerSpawner = playerSpawner;
            _config = config;
        }
        
        public void SpawnInitialDrone()
        {
            Vector3 spawnPosition = _config.BaseCoordinates + new Vector3(2f, 0f, 0f);
            SpawnDrone(spawnPosition);
        }
        
        public void SpawnDrone(Vector3 position)
        {
            PlayerEntity drone = _playerSpawner.SpawnPlayer(position);
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

