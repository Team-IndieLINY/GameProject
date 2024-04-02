using JetBrains.Annotations;
using UnityEngine;

namespace IndieLINY.AI
{
    public interface IParam
    {
        
    }

    public interface IAction
    {
        public void SetParam(IParam param);
        public IParam GetParam();
    }

    public abstract class BaseAction : ScriptableObject, IAction
    {
        [CanBeNull] internal IParam _param;
        
        public void SetParam([CanBeNull] IParam param)
        {
            _param = param;
        }

        [CanBeNull]
        public IParam GetParam()
        {
            return _param;
        }
    }
    public abstract class BaseActionT<TParam> : BaseAction
        where TParam : class, IParam
    {
        public TParam Param => _param as TParam;
    }
}