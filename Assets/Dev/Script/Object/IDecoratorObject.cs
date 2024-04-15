using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace IndieLINY
{
    public interface IBObjectDecoratedObject: IObjectBehaviour
    {
        public UniTask<IBObjectDecoratedObject> Interact(CancellationToken token);
    }
}
