using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class LootingManager
{
    private static UniTask _lootingTask;

    public static void RegisterLootingTask(UniTask lootingTask)
    {
        _lootingTask = lootingTask;
    }
}