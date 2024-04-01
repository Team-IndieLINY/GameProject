using System;
using System.Collections;
using System.Collections.Generic;
using IndieLINY.MessagePipe;
using IndieLINY.Singleton;
using UnityEngine;

public class TestSubscriber : MonoBehaviour, IMessagePipeSubscriber<PubsubMessageEventTest>
{
    private PubSubMessageChannel<PubsubMessageEventTest> _channel;
    private void Awake()
    {
        _channel = Singleton
            .GetSingleton<MessagePipe>()
            .GetChannel<PubSubMessageChannel<PubsubMessageEventTest>>("Test1");
        
        _channel.Subscribe(this);
    }

    public void ReceiveEvent(PubsubMessageEventTest evt)
    {
        print($"recv({evt.Text}) + " + name);
    }
}
