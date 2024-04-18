using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BoxInventoryUI : InventoryUI
{
    private Button _takeAllButton;
    private void Awake()
    {
        var inventory = GetComponent<BoxInventory>();
        Debug.Assert(inventory, "inventory is null");
        _inventory = inventory;
        
        _inventoryUIDocument = GetComponent<UIDocument>();
        _rootVisualElement = _inventoryUIDocument.rootVisualElement;
        _containerElement = _rootVisualElement.Q<VisualElement>("Container");
        _headLabel = _rootVisualElement.Q<Label>("HeadLabel");
        _slotContainerVisualElement = _rootVisualElement.Q<VisualElement>("SlotContainer");
        
        _takeAllButton = _rootVisualElement.Q<Button>("TakeAllButton");
        _takeAllButton.RegisterCallback<ClickEvent>(OnClickTakeAllButton);

        _tooltipUI = new TooltipUI(_rootVisualElement.Q<VisualElement>("Tooltip"));

        _rootVisualElement.style.visibility = Visibility.Hidden;
    }
    
    private void OnClickTakeAllButton(ClickEvent evt)
    {
        BoxInventory.Instance.TakeAll();
    }
}
