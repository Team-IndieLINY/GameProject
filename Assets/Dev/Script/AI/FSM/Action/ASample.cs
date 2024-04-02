using UnityEngine;

namespace IndieLINY.AI
{
    [CreateAssetMenu(menuName = "IndieLINY/AI/Action/Sample", fileName = "Sample")]
    public class ASample : BaseActionT<PSample>
    {
        [SerializeField] private string _text;

        public void OnEnter()
        {
            Debug.Log(_text + " enter");
        }
        
        public void OnUpdate()
        {
            if (Param == null)
            {
                Debug.Log(_text);
            }
            else
            {
                Debug.Log(_text + " " + Param.Sample);
            }
        }
        
        public void OnExit()
        {
            Debug.Log(_text + " exit");
        }
    }

    public class PSample : IParam
    {
        public string Sample;
    }

}