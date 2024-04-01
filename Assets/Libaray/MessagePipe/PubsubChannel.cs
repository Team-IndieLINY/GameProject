using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieLINY.MessagePipe
{
    public interface IMessagePipePublisher<in T> : IMessagePipePublisher
        where T : IMessagePipeEvent
    {
    }
    public interface IMessagePipeSubscriber<in T> : IMessagePipeSubscriber
        where T : IMessagePipeEvent
    {
        public void ReceiveEvent(T evt);
    }
    
    public class PubSubMessageChannel<T> : IMessagePipeChannel
        where T : IMessagePipeEvent
    {
        private const int Capacity = 2;
        private List<IMessagePipePublisher<T>> _publishers = new(Capacity);
        private List<IMessagePipeSubscriber<T>> _subscribers = new(Capacity);

        public IReadOnlyCollection<IMessagePipePublisher<T>> Publishers => _publishers;
        public IReadOnlyCollection<IMessagePipeSubscriber<T>> Subscribers => _subscribers;

        public void Subscribe(IMessagePipeSubscriber<T> subscriber)
        {
            _subscribers.Add(subscriber);
        }

        public void DeSubscribe(IMessagePipeSubscriber<T> subscriber)
        {
            _subscribers.Remove(subscriber);
        }

        public void RegisterPublisher(IMessagePipePublisher<T> publisher)
        {
            _publishers.Add(publisher);
        }

        public void UnregisterPublisher(IMessagePipePublisher<T> publisher)
        {
            _publishers.Remove(publisher);
        }

        public void PublishEvent(T evt)
        {
            foreach (var subscriber in Subscribers)
            {
                subscriber.ReceiveEvent(evt);
            }
        }
    }
}