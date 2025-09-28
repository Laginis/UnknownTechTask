using System;
using System.Collections.Generic;

public static class EventBus //TODO: change to ScriptableObject-based Event Channels???
{
    private static readonly Dictionary<Type, List<Delegate>> subscribers = new();

    public static void Subscribe<T>(Action<T> callback)
    {
        var type = typeof(T);

        if (!subscribers.ContainsKey(type))
        {
            subscribers[type] = new List<Delegate>();
        }

        subscribers[type].Add(callback);
    }

    public static void Unsubscribe<T>(Action<T> callback)
    {
        var type = typeof(T);

        if (subscribers.TryGetValue(type, out var list))
        {
            list.Remove(callback);
        }
    }

    public static void Publish<T>(T eventData)
    {
        var type = typeof(T);

        if (subscribers.TryGetValue(type, out var list))
        {
            foreach (var callback in new List<Delegate>(list))
            {
                (callback as Action<T>)?.Invoke(eventData);
            }
        }
    }
}

/// usage

// public class PlayerDiedEvent    - create an event
// {
//     public int LivesLeft;
// }

// void OnEnable()    - subscribe to the event
// {
//     EventBus.Subscribe<PlayerDiedEvent>(OnPlayerDied);
// }

// void OnDisable()    - unsubscribe from the event
// {
//     EventBus.Unsubscribe<PlayerDiedEvent>(OnPlayerDied);
// }

// private void OnPlayerDied(PlayerDiedEvent e)    - method for handling event
// {
//     Debug.Log("Player died! Lives left: " + e.LivesLeft);
// }

// EventBus.Publish(new PlayerDiedEvent { LivesLeft = 2 });    - event invoke
