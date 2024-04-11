using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerInventoryUI : InventoryUI
{
    private void Awake()
    {
        Debug.Assert(TryGetComponent(out PlayerInventory inventory), "inventory is null");
        _inventory = inventory;
        
        _inventoryUIDocument = GetComponent<UIDocument>();
        _rootVisualElement = _inventoryUIDocument.rootVisualElement;
        _containerElement = _rootVisualElement.Q<VisualElement>("Container");
        _headLabel = _rootVisualElement.Q<Label>("HeadLabel");
        _bodyVisualElement = _rootVisualElement.Q<VisualElement>("Body");

        _isOpen = _rootVisualElement.visible;
    }
}
