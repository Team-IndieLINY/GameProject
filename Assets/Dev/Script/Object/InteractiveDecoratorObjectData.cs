using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieLINY
{
    [CreateAssetMenu(menuName = "IndieLINY/Object/InteractiveDecoratorData", fileName = "InteractiveDecoratorData")]
    public class InteractiveDecoratorObjectData : ScriptableObject
    {
        [SerializeField] private string _triggerAiAnimationKey;
        [SerializeField] private string _triggerAiEventKey;

        //TODO: 이펙트와 사운드 관련 인터페이스 구축하여 이 부분 수정
        [SerializeField] private GameObject _effect;
        [SerializeField] private GameObject _sound;

        public string TriggerAiAnimationKey => _triggerAiAnimationKey;

        public string TriggerAiEventKey => _triggerAiEventKey;
    }
}
