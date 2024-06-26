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
        public event Action<ActorContractInfo> OnExitActor;
        public event Action<ObjectContractInfo> OnExitObject;
        public event Action<ClickContractInfo> OnExitClick;
        
        public LayerMask LayerMask { get; }
        public bool ListeningOnly { get; }
        public bool DetectedOnly { get; }
        public BaseContractInfo ContractInfo { get; }

        public bool IsEnabled { get; set; }

        public T GetContractInfoOrNull<T>() where T : BaseContractInfo;

        public bool TryGetContractInfo<T>(out T info) where T : BaseContractInfo;
        public void Activate(BaseContractInfo info);
        public void DeActivate(BaseContractInfo info);

        public void ClearContractEvent();
        public object Owner { get; }
    }

    public abstract class CollisionInteractionMono : MonoBehaviour, ICollisionInteraction
    {
        public abstract object Owner { get; internal set; }
        public abstract event Action<ActorContractInfo> OnContractActor;
        public abstract event Action<ObjectContractInfo> OnContractObject;
        public abstract event Action<ClickContractInfo> OnContractClick;
        public abstract event Action<ActorContractInfo> OnExitActor;
        public abstract event Action<ObjectContractInfo> OnExitObject;
        public abstract event Action<ClickContractInfo> OnExitClick;
        public abstract LayerMask LayerMask { get; }
        public abstract bool ListeningOnly { get; }
        public abstract bool DetectedOnly { get; }
        public abstract BaseContractInfo ContractInfo { get; internal set; }
        
        public abstract bool IsEnabled { get; set; }
        public abstract T GetContractInfoOrNull<T>() where T : BaseContractInfo;
        public abstract bool TryGetContractInfo<T>(out T info) where T : BaseContractInfo;
        public abstract void Activate(BaseContractInfo info);
        public abstract void DeActivate(BaseContractInfo info);

        public abstract void ClearContractEvent();
    }

    public static class CollisionInteractionUtil
    {
        public static bool OnCollision(Collider2D other, ICollisionInteraction interaction, bool isActivating, out CollisionInteraction result)
        {
            result = null;
            
            if (interaction.DetectedOnly) return false;
            if (!interaction.IsEnabled) return false;
            
            int layer = 1 << other.gameObject.layer;
            if ((layer & interaction.LayerMask.value) != layer) return false;
            
            if (other.gameObject.TryGetComponent<CollisionInteraction>(out var com))
            {
                if (!com.IsEnabled) return false;
                if (com.ListeningOnly) return false;

                result = com;

                if (isActivating)
                {
                    interaction.Activate(com.ContractInfo);
                }
                else
                {
                    interaction.DeActivate(com.ContractInfo);
                }
            }

            return true;
        }
    }
    
    public class CollisionInteraction : CollisionInteractionMono
    {
        public override object Owner { get; internal set; }
        public override event Action<ActorContractInfo> OnContractActor;
        public override event Action<ObjectContractInfo> OnContractObject;
        public override event Action<ClickContractInfo> OnContractClick;
        public override event Action<ActorContractInfo> OnExitActor;
        public override event Action<ObjectContractInfo> OnExitObject;
        public override event Action<ClickContractInfo> OnExitClick;

        public override LayerMask LayerMask => _layerMask;
        public override bool ListeningOnly => _listeningOnly;
        public override bool DetectedOnly => _detectedOnly;
        public override BaseContractInfo ContractInfo { get; internal set; }

        [SerializeField] private bool _isBindChildProxy = true;
        [SerializeField] private LayerMask _layerMask;
        [SerializeField] private bool _listeningOnly;
        [SerializeField] private bool _detectedOnly;

        private List<CollisionInteractionProxy> _proxies;
        public IReadOnlyCollection<CollisionInteractionProxy> Proxies => _proxies;
        
        public void SetContractInfo(BaseContractInfo info, object owner)
        {
            Owner = owner;
            ContractInfo = info;
            info._interaction = this;
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
            Debug.Assert(info != null, "ContractInfo can't be null");
            
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
            }
        }

        public override void DeActivate(BaseContractInfo info)
        {
            Debug.Assert(info != null, "ContractInfo can't be null");
            
            switch (info)
            {
                case ActorContractInfo actorContractInfo:
                    OnExitActor?.Invoke(actorContractInfo);
                    break;
                case ClickContractInfo clickContractInfo:
                    OnExitClick?.Invoke(clickContractInfo);
                    break;
                case ObjectContractInfo objectContractInfo:
                    OnExitObject?.Invoke(objectContractInfo);
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
                InitProxies(transform);
            }
        }

        private void InitProxies(Transform parent)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i);

                if (child.GetComponent<CollisionInteraction>())
                {
                    continue;
                }
                
                if (child.TryGetComponent(out CollisionInteractionProxy proxy))
                {
                    AddProxy(proxy);
                }
                
                InitProxies(child);
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
            if (CollisionInteractionUtil.OnCollision(other.collider, this, true, out var com))
            {
                _collisionBridge.Push(this, com);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (CollisionInteractionUtil.OnCollision(other, this, true, out var com))
            {
                _collisionBridge.Push(this, com);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            CollisionInteractionUtil.OnCollision(other, this, false, out var com);
        }
        private void OnCollisionExit2D(Collision2D other)
        {
            CollisionInteractionUtil.OnCollision(other.collider, this, false, out var com);
        }
    }
}