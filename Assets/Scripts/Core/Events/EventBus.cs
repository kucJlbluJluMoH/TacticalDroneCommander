using System;
using System.Collections.Generic;
using UnityEngine;

namespace TacticalDroneCommander.Core.Events
{
    public interface IEventBus
    {
        void Subscribe<T>(Action<T> handler) where T : IGameEvent;
        void Unsubscribe<T>(Action<T> handler) where T : IGameEvent;
        void Publish<T>(T gameEvent) where T : IGameEvent;
    }
    
    public class EventBus : IEventBus
    {
        private readonly Dictionary<Type, List<Delegate>> _subscribers = new();

        public void Subscribe<T>(Action<T> handler) where T : IGameEvent
        {
            var eventType = typeof(T);
            
            if (!_subscribers.ContainsKey(eventType))
            {
                _subscribers[eventType] = new List<Delegate>();
            }
            
            _subscribers[eventType].Add(handler);
        }

        public void Unsubscribe<T>(Action<T> handler) where T : IGameEvent
        {
            var eventType = typeof(T);
            
            if (_subscribers.ContainsKey(eventType))
            {
                _subscribers[eventType].Remove(handler);
            }
        }

        public void Publish<T>(T gameEvent) where T : IGameEvent
        {
            var eventType = typeof(T);
            
            if (!_subscribers.ContainsKey(eventType))
                return;
            
            var handlers = _subscribers[eventType];
            
            foreach (var handler in handlers.ToArray())
            {
                try
                {
                    ((Action<T>)handler)?.Invoke(gameEvent);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"EventBus: Error invoking handler for {eventType.Name}: {ex.Message}");
                }
            }
        }
    }
}

