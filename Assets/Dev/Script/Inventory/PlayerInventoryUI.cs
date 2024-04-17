using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(PlayerInventory))]
public class PlayerInventoryUI : InventoryUI
{
    private void Awake()
    {
        var inventory = GetComponent<PlayerInventory>();
        Debug.Assert(inventory, "inventory is null");
        _inventory = inventory;
        
        _inventoryUIDocument = GetComponent<UIDocument>();
        _rootVisualElement = _inventoryUIDocument.rootVisualElement;
        _containerElement = _rootVisualElement.Q<VisualElement>("Container");
        _headLabel = _rootVisualElement.Q<Label>("HeadLabel");
        _slotContainerVisualElement = _rootVisualElement.Q<VisualElement>("SlotContainer");
        
        _tooltipUI = new TooltipUI(_rootVisualElement.Q<VisualElement>("Tooltip"));
        
        _rootVisualElement.style.visibility = Visibility.Hidden;
    }
}
