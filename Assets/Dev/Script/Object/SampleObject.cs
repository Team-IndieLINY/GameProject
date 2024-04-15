using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IndieLINY.Event;

public interface IBOSample : IObjectBehaviour
{
    public void OpenInventory();
}

public class SampleObject : MonoBehaviour, IBOSample
{
    [SerializeField] private CollisionInteraction _interaction;
    [SerializeField] private List<CountableItem> _countableItems;
    private void Awake()
    {
        var info = ObjectContractInfo.Create(transform, () => gameObject == false);
        Interaction.SetContractInfo(info, this);

        info.AddBehaivour<IBOSample>(this);
    }

    public CollisionInteraction Interaction => _interaction;
    
    public void OpenInventory()
    {
        BoxInventory.Instance.ResetInventory();
        SetInventory();
        PlayerInventory.Instance.OpenInventory();
        BoxInventory.Instance.OpenInventory();
    }

    private void SetInventory()
    {
        foreach (var item in _countableItems)
        {
            BoxInventory.Instance.AddItem(item);
        }
    }
}
