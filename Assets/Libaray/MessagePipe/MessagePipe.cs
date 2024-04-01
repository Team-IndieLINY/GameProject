using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IndieLINY.Singleton;
using JetBrains.Annotations;

namespace IndieLINY.MessagePipe
{
    public interface IMessagePipeObject
    {
    }
    
    public interface IMessagePipePublisher : IMessagePipeObject
    {
    }
    public interface IMessagePipeSubscriber : IMessagePipeObject
    {
    }

    public interface IMessagePipeEvent
    {
        
    }

    public interface IMessagePipeChannel
    {
    }

    
    [Singleton(ESingletonType.Global)]
    public class MessagePipe : MonoBehaviourSingleton<MessagePipe>
    {
        private Dictionary<string, IMessagePipeChannel> _channels;

        public override void PostInitialize()
        {
            _channels = new();
        }

        public override void PostRelease()
        {
            _channels.Clear();
            _channels = null;
        }
        
        public T GetChannel<T>(string key) where T : class, IMessagePipeChannel, new()
        {
            if (_channels.TryGetValue(key, out var channel))
            {
                if (channel is T)
                {
                    return channel as T;
                }
                
                throw new Exception($"MessagePipe error: invalid class type({typeof(T).Name})");
            }
            
            T newChannel = new T();
            _channels.Add(key, newChannel);

            return newChannel;
        }
    }   
}
