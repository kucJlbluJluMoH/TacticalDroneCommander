using System;
using System.IO;
using UnityEngine;
using VContainer;
using Newtonsoft.Json;

namespace TacticalDroneCommander.Infrastructure
{
    
    [Serializable]
    public class SaveData
    {
        public int highScore;
        public int unlockedDrones;
        public string lastPlayDate;

        public int HighScore
        {
            get => highScore;
            set => highScore = value;
        }

        public SaveData()
        {
            highScore = 0;
            unlockedDrones = 1;
            lastPlayDate = DateTime.Now.ToString();
        }
    }
    
    public interface ISaveLoadService
    {
        SaveData CurrentSave { get; }
        void Save();
        void Load();
        void UpdateHighScore(int newScore);
        SaveData GetSaveData();
    }
    
    public class SaveLoadService : ISaveLoadService
    {
        private readonly Core.GameConfig _config;
        private string SavePath => Path.Combine(Application.persistentDataPath, _config.SaveFileName);

        public SaveData CurrentSave { get; private set; }

        [Inject]
        public SaveLoadService(Core.GameConfig config)
        {
            _config = config;
            CurrentSave = new SaveData();
        }

        public void Save()
        {
            try
            {
                CurrentSave.lastPlayDate = DateTime.Now.ToString();
                string json = JsonConvert.SerializeObject(CurrentSave, Formatting.Indented);
                File.WriteAllText(SavePath, json);
                Debug.Log($"SaveLoadService: Game saved to {SavePath}");
            }
            catch (Exception e)
            {
                Debug.LogError($"SaveLoadService: Failed to save game - {e.Message}");
            }
        }

        public void Load()
        {
            try
            {
                if (File.Exists(SavePath))
                {
                    string json = File.ReadAllText(SavePath);
                    CurrentSave = JsonConvert.DeserializeObject<SaveData>(json);
                    Debug.Log($"SaveLoadService: Game loaded from {SavePath}");
                    Debug.Log($"SaveLoadService: High Score = {CurrentSave.highScore}");
                }
                else
                {
                    Debug.Log("SaveLoadService: No save file found, creating new save");
                    CurrentSave = new SaveData();
                    Save();
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"SaveLoadService: Failed to load game - {e.Message}");
                CurrentSave = new SaveData();
            }
        }

        public void UpdateHighScore(int newScore)
        {
            if (newScore > CurrentSave.highScore)
            {
                CurrentSave.highScore = newScore;
                Debug.Log($"SaveLoadService: New high score! {newScore}");
                Save();
            }
        }

        public SaveData GetSaveData()
        {
            return CurrentSave;
        }
    }
}
