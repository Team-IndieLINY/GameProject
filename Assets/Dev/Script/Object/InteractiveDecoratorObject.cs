using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

using IndieLINY.Event;

namespace IndieLINY
{
    public class InteractiveDecoratorObject : MonoBehaviour, IBObjectDecoratedObject
    {
        [SerializeField] private CollisionInteraction _interaction;
        [SerializeField] private Vector2 _interactingPivot;
        [SerializeField] private float _waitDuration;

        [SerializeField] private InteractiveDecoratorObjectData _data;

        public InteractiveDecoratorObjectData Data => _data;

        public Vector2 InteractingPositionWorld
        {
            get => transform.TransformPoint(InteractingPositionLocal);
            set=> InteractingPositionLocal = transform.InverseTransformPoint(value);
        }

        private Vector2 InteractingPositionLocal
        {
            get => _interactingPivot;
            set => _interactingPivot = value;
        }
        
        public CollisionInteraction Interaction => _interaction;
        public async UniTask<IBObjectDecoratedObject> Interact(CancellationToken token)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_waitDuration), DelayType.DeltaTime, PlayerLoopTiming.Update, token);

            return this;
        }

        private void Awake()
        {
            var info = ObjectContractInfo.Create(transform, ()=>gameObject);
            Interaction.SetContractInfo(info, this);

            info.AddBehaivour<IBObjectDecoratedObject>(this);
        }
    }
}