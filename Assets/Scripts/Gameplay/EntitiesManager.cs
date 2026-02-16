using System;
using System.Collections.Generic;
using System.Linq;
using Entities;
using UnityEngine;

namespace Gameplay
{
    public interface IEntitiesManager
    {
        void RegisterEntity(Entity entity);
        void UnregisterEntity(Entity entity);
        IReadOnlyList<Entity> GetAllEntities();
        IEnumerable<T> GetEntitiesOfType<T>() where T : Entity;
        Entity GetEntity(String id);
    }
    
    public class EntitiesManager : IEntitiesManager
    {
        private readonly List<Entity> _entities = new();
        
        public void RegisterEntity(Entity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
                
            if (!_entities.Contains(entity))
            {
                _entities.Add(entity);
            }
        }
        
        public void UnregisterEntity(Entity entity)
        {
            if (entity == null)
                return;
                
            _entities.Remove(entity);
        }
        
        public IReadOnlyList<Entity> GetAllEntities()
        {
            return _entities.AsReadOnly();
        }
        
        public IEnumerable<T> GetEntitiesOfType<T>() where T : Entity
        {
            return _entities.OfType<T>();
        }
        
        public Entity GetEntity(String id)
        {
            return _entities.FirstOrDefault(e => e.GetId() == id);
        }
    }
}
