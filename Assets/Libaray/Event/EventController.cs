using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using IndieLINY.Singleton;
using UnityEngine;


namespace IndieLINY.Event
{
    public interface IEventCommand
    {
    }

    public abstract class EventBridge
    {
        public abstract IEventCommand GetEventCommand();
    }

    [Singleton(ESingletonType.Global)]
    public class EventController : MonoBehaviourSingleton<EventController>
    {
        private delegate void EventHandler(IEventCommand command);

        private Dictionary<Type, List<EventHandler>> _eventPageTable;
        private Dictionary<Type, EventBridge> _bridgeTable;

        public override void PostInitialize()
        {
            _eventPageTable = new();
            _bridgeTable = new();

            var methods = typeof(EventPage).GetMethods(
                BindingFlags.Static |
                BindingFlags.Public |
                BindingFlags.NonPublic
            );

            foreach (var method in methods)
            {
                var action = method.CreateDelegate(typeof(EventHandler)) as EventHandler;
                var att = method.GetCustomAttribute<EventHandlerAttribute>();

                if (att == null || action == null) continue;

                if (_eventPageTable.TryGetValue(att.Type, out var list))
                {
                    list.Add(action);
                }
                else
                {
                    list = new List<EventHandler>(10);
                    _eventPageTable.Add(att.Type, list);
                    list.Add(action);
                }
            }
        }

        public override void PostRelease()
        {
            _eventPageTable.Clear();
            _eventPageTable = null;
            
            _bridgeTable.Clear();
            _bridgeTable = null;
        }

        public T GetBridge<T>() where T : EventBridge, new()
        {
            if (_bridgeTable.TryGetValue(typeof(T), out var bridge))
            {
                return bridge as T;
            }
            else
            {
                bridge = new T();
                _bridgeTable.Add(typeof(T), bridge);
                return bridge as T;
            }
        }

        private void LateUpdate()
        {
            foreach (var bridge in _bridgeTable.Values)
            {
                var command = bridge.GetEventCommand();

                SignalEvent(command);
            }
        }

        public void SignalEvent(IEventCommand command)
        {
            if (_eventPageTable.TryGetValue(command.GetType(), out var list))
            {
                foreach (var handler in list)
                {
                    handler.Invoke(command);
                }
            }
        }
    }

    [System.AttributeUsage(AttributeTargets.Method)]
    public class EventHandlerAttribute : Attribute
    {
        public System.Type Type;

        public EventHandlerAttribute(System.Type type)
        {
            this.Type = type;
        }
    }
    public static partial class EventPage
    {
    }
}