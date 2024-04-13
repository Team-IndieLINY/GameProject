using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class LootingManager
{
    private static Func<UniTask> _lootingTask;
    private CancellationTokenSource _lootingCancellationToken = new CancellationTokenSource();

    public static void RegisterLootingTask(Func<UniTask> lootingTask)
    {
        _lootingTask = lootingTask;
    }

    public static bool IsProcessingTask()
    {
        return false;
    }
}