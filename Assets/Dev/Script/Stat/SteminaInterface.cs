using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieLINY
{
    public enum ESteminaType
    {
        Endurance,
    }
    
    public interface ISteminaController
    {
        /// <summary>
        /// 타입에 따른 현재 스테미나 값을 (0~1) 범위의 정규화된 값을 반환합니다.
        /// </summary>
        /// <param name="type">스테미나 타입</param>
        /// <returns>0~1 범위의 정규화된 스테미나 값</returns>
        public float GetStemina();
        /// <summary>
        /// 타입에 따른 현재 스테미나 값을 (0~1) 범위의 정규화된 값으로 설정합니다.
        /// </summary>
        /// <param name="type">스테미나 타입</param>
        /// <param name="statValue">0~1 범위의 정규화된 스테미나 값</param>
        public void SetStemina(int statValue);

        /// <summary>
        /// (0~1)범위의 정규화된 값 만큼 실제 값을 증가시킵니다.
        /// </summary>
        /// <param name="type">스테미나 타입</param>
        /// <param name="statValue">0~1 범위의 정규화된 스테미나 값</param>
        public void Increase(int statValue);
        /// <summary>
        /// (0~1)범위의 정규화된 값 만큼 실제 값을 감소시킵니다.
        /// </summary>
        /// <param name="type">스테미나 타입</param>
        /// <param name="statValue">0~1 범위의 정규화된 스테미나 값</param>
        public void Decrease(int statValue);

        /// <summary>
        /// 해당 타입의 스테미나를 초기값으로 리셋합니다.
        /// </summary>
        /// <param name="type">스테미나 타입</param>
        public void ResetStemina();
    }

}