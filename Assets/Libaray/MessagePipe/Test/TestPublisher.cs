using System;
using System.Collections;
using System.Collections.Generic;
using IndieLINY.MessagePipe;
using IndieLINY.Singleton;
using UnityEngine;

public class PubsubMessageEventTest : IMessagePipeEvent
{
    public string Text = "";
}
public class TestPublisher : MonoBehaviour, IMessagePipePublisher<PubsubMessageEventTest>
{
    private PubSubMessageChannel<PubsubMessageEventTest> _channel;
    private void Awake()
    {
        _channel = Singleton
            .GetSingleton<MessagePipe>()
            .GetChannel<PubSubMessageChannel<PubsubMessageEventTest>>("Test1");
        
        _channel.RegisterPublisher(this);
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Publish"))
        {
            _channel.PublishEvent(new PubsubMessageEventTest()
            {
                Text = name
            });
        }
    }
    
}
