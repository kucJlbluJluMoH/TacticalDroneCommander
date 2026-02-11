using UnityEngine;

namespace TacticalDroneCommander.Core
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "TacticalDroneCommander/Game Config")]
    public class GameConfig : ScriptableObject
    {
        [Header("Save Settings")]
        [Tooltip("Name of save file")]
        public string SaveFileName = "save.json";
    }
}

