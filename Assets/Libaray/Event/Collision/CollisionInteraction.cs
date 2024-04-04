using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using IndieLINY.Singleton;

namespace IndieLINY.Event
{
    public interface ICollisionInteraction
    {
        public event Action<ActorContractInfo> OnContractActor;
        public event Action<ObjectContractInfo> OnContractObject;
        public event Action<ClickContractInfo> OnContractClick;
        public BaseContractInfo ContractInfo { get; }

        public bool IsEnabled { get; set; }

        public T GetContractInfoOrNull<T>() where T : BaseContractInfo;

        public bool TryGetContractInfo<T>(out T info) where T : BaseContractInfo;
        public void Activate(BaseContractInfo info);

        public void ClearContractEvent();
        public object Owner { get; }
    }

    public abstract class CollisionInteractionMono : MonoBehaviour, ICollisionInteraction
    {
        public abstract object Owner { get; internal set; }
        public abstract event Action<ActorContractInfo> OnContractActor;
        public abstract event Action<ObjectContractInfo> OnContractObject;
        public abstract event Action<ClickContractInfo> OnContractClick;
        public abstract BaseContractInfo ContractInfo { get; internal set; }
        
        public abstract bool IsEnabled { get; set; }
        public abstract T GetContractInfoOrNull<T>() where T : BaseContractInfo;
        public abstract bool TryGetContractInfo<T>(out T info) where T : BaseContractInfo;
        public abstract void Activate(BaseContractInfo info);
        public abstract void ClearContractEvent();
    }
    
    public class CollisionInteraction : CollisionInteractionMono
    {
        public override object Owner { get; internal set; }
        public override event Action<ActorContractInfo> OnContractActor;
        public override event Action<ObjectContractInfo> OnContractObject;
        public override event Action<ClickContractInfo> OnContractClick;
        public override BaseContractInfo ContractInfo { get; internal set; }

        [SerializeField] private bool _isBindChildProxy = true;

        private List<CollisionInteractionProxy> _proxies;
        public IReadOnlyCollection<CollisionInteractionProxy> Proxies => _proxies;
        
        public void SetContractInfo(BaseContractInfo info, object owner)
        {
            Owner = owner;
            ContractInfo = info;
        }

        public override bool IsEnabled
        {
            get => enabled;
            set => enabled = value;
        }

        public override T GetContractInfoOrNull<T>()
            => ContractInfo as T;

        public override bool TryGetContractInfo<T>(out T info)
        {
            info = GetContractInfoOrNull<T>();
            return info is not null;
        }

        public override void Activate(BaseContractInfo info)
        {
            switch (info)
            {
                case ActorContractInfo actorContractInfo:
                    OnContractActor?.Invoke(actorContractInfo);
                    break;
                case ClickContractInfo clickContractInfo:
                    OnContractClick?.Invoke(clickContractInfo);
                    break;
                case ObjectContractInfo objectContractInfo:
                    OnContractObject?.Invoke(objectContractInfo);
                    break;
                default:
                    //throw new ArgumentOutOfRangeException(nameof(ContractInfo));
                    break;
            }
        }

        public override void ClearContractEvent()
        {
            OnContractActor = null;
            OnContractObject = null;
            OnContractClick = null;
        }

        private CollisionBridge _collisionBridge;

        private void Awake()
        {
            if (_isBindChildProxy)
            {
                var childs  =GetComponentsInChildren<CollisionInteractionProxy>(true);
                
                AddProxyRange(childs);
            }
        }

        public void AddProxy(CollisionInteractionProxy proxy)
        {
            _proxies.Add(proxy);
            proxy.MainInteraction = this;
        }

        public void AddProxyRange(params CollisionInteractionProxy[] proxies)
        {
            if (proxies.Length == 0) return;
            
            _proxies.AddRange(proxies);
            foreach (var proxy in proxies)
            {
                proxy.MainInteraction = this;
            }
        }
        
        public bool RemoveProxy(CollisionInteractionProxy proxy)
        {
            proxy.MainInteraction = null;
            return _proxies.Remove(proxy);
        }

        private void Start()
        {
            _collisionBridge = Singleton.Singleton.GetSingleton<EventController>().GetBridge<CollisionBridge>();
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (!IsEnabled) return;
            if (other.gameObject.TryGetComponent<CollisionInteraction>(out var com))
            {
                if (!com.IsEnabled) return;
                Activate(com.ContractInfo);

                _collisionBridge.Push(this, com);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!IsEnabled) return;
            if (other.gameObject.TryGetComponent<CollisionInteraction>(out var com))
            {
                if (!com.IsEnabled) return;
                Activate(com.ContractInfo);

                _collisionBridge.Push(this, com);
            }
        }
    }
}