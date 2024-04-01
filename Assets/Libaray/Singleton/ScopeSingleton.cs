using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using XRProject.Utils.Log;
using Object = UnityEngine.Object;

namespace IndieLINY.Singleton
{
    [Singleton(ESingletonType.Global)]
    public class ScopeSingleton : IGeneralSingleton
    {
        public delegate void ScopeChangedCallback(ISingleton singleton);
        
        private List<IMonoBehaviourSingleton> _singletons;

        private Dictionary<System.Type, List<ScopeChangedCallback>> _callbackTable;
        public void Initialize()
        {
            SceneManager.activeSceneChanged += OnActivateSceneChanged;
            
            _singletons = new(5);
            _callbackTable = new();

            SceneLoaded();
        }

        public void Release()
        {
            SceneManager.activeSceneChanged -= OnActivateSceneChanged;

            SceneUnloaded();
            
            _singletons = null;
            _callbackTable = null;
        }

        private void OnActivateSceneChanged(Scene currentScene, Scene nextScene)
        {
            SceneUnloaded();
            SceneLoaded();
        }
        

        private void SceneUnloaded()
        {
            foreach (var singleton in _singletons)
            {
                singleton.Release();
            }
            
            _singletons.Clear();
        }

        private void SceneLoaded()
        {
            var initlaizers = Object.FindObjectsOfType<ScopeSingletonInitializer>();

            if (initlaizers.Length > 1)
            {
                XLog.LogError("scope initializer가 scene에 2개 이상입니다.", "Singleton");
            }

            if (initlaizers.Length == 0) return;

            var initializer = initlaizers[0];
            if (initializer.CheckValid())
            {
                foreach (var singleton in initializer._singletons)
                {
                    singleton.Initialize();
                    _singletons.Add(singleton);

                    TryInvokeTable(singleton);
                }
            }
        }

        private void TryInvokeTable(ISingleton singleton)
        {
            if (_callbackTable.TryGetValue(singleton.GetType(), out var callbacks))
            {
                callbacks.ForEach(x=>x.Invoke(singleton));
            }
        }

        public T GetScopeSingleton<T>() where T : class, IMonoBehaviourSingleton
        {
            return _singletons.Find(x => x is T) as T;
        }

        public void RegisterScopeSingletonChanged(System.Type type, ScopeChangedCallback callback)
        {
            if (_callbackTable.TryGetValue(type, out var callbacks))
            {
                callbacks.Add(callback);
            }
            else
            {
                var list = new List<ScopeChangedCallback>(10);
                _callbackTable.Add(type, list);
                list.Add(callback);
            }
        }
        public void UnRegisterScopeSingletonChanged(System.Type type, ScopeChangedCallback callback)
        {
            if (_callbackTable.TryGetValue(type, out var callbacks))
            {
                callbacks.Remove(callback);
            }
        }

        public void RegisterScopeSingletonChanged<T>(ScopeChangedCallback callback)
            => RegisterScopeSingletonChanged(typeof(T), callback);

        public void UnRegisterScopeSingletonChanged<T>(ScopeChangedCallback callback)
            => UnRegisterScopeSingletonChanged(typeof(T), callback);
    }
}