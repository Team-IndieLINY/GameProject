using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using XRProject.Utils.Log;

namespace IndieLINY.Singleton
{
    public class ScopeSingletonInitializer : MonoBehaviour
    {
        [SerializeField] internal List<BaseMonoBehaviourSingleton> _singletons;

        internal bool CheckValid()
        {
            #if UNITY_EDITOR
            for (int i = 0; i < _singletons.Count; i++)
            {
                if(_singletons[i] == null)continue;

                var att = _singletons[i].GetType().GetCustomAttributes<SingletonAttribute>();
                if (att is SingletonAttribute sAtt && sAtt.Type != ESingletonType.Scope)
                {
                    XLog.LogError($"Singleton attribute가 없거나, 올바르지 않은 SingletonType attribute 입니다. ({_singletons[i].GetType().Name})", "Singleton");
                    continue;
                }
                
                for (int j = i + 1; j < _singletons.Count; j++)
                {
                    if(_singletons[j] == null)continue;
                    
                    if (_singletons[i].GetType() == _singletons[j].GetType())
                    {
                        XLog.LogError($"동일한 타입의 ScopeSingleton 객체({_singletons[i].GetType().Name})가 포함되어있습니다.", "Singleton");
                        return false;
                    }
                }
            }
            #endif

            return true;
        }
    }
}