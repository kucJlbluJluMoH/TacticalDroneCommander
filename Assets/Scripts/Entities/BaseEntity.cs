using UnityEngine;
using TacticalDroneCommander.Core;

namespace Entities
{
    public class BaseEntity : Entity
    {
        public BaseEntity(string id, GameObject entityObject, GameConfig config) 
            : base(id, config.BaseHP, config.BaseHP, 0, entityObject)
        {
        }
    }
}

