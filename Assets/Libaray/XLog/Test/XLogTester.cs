using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XRProject.Utils.Log;

public class XLogTester : MonoBehaviour
{
    public string Signature;
    void Update()
    {
        XLog.LogError("Test error", Signature);
        XLog.LogWarn("Test warn", Signature);
        XLog.LogDebug("Test debug", Signature);

    }
}
