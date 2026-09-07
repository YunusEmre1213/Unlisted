using System;
using System.Collections.Generic;

namespace Backrooms.Core
{
    public interface IGameEvent { }

    public static class EventBus
    {
        private static readonly Dictionary<Type, List<Delegate>> subscribers = new Dictionary<Type, List<Delegate>>();

        public static void Subscribe<T>(Action<T> handler) where T : IGameEvent
        {
            var type = typeof(T);
            if (!subscribers.ContainsKey(type))
                subscribers[type] = new List<Delegate>();

            subscribers[type].Add(handler);
        }

        public static void Unsubscribe<T>(Action<T> handler) where T : IGameEvent
        {
            var type = typeof(T);
            if (subscribers.ContainsKey(type))
                subscribers[type].Remove(handler);
        }

        public static void Publish<T>(T gameEvent) where T : IGameEvent
        {
            var type = typeof(T);
            if (!subscribers.ContainsKey(type)) return;

            var handlers = new List<Delegate>(subscribers[type]);
            foreach (var handler in handlers)
            {
                (handler as Action<T>)?.Invoke(gameEvent);
            }
        }

        public static void Clear()
        {
            subscribers.Clear();
        }
    }
}