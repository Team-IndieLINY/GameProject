using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(PlayerInventory))]
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
        _slotContainerVisualElement = _rootVisualElement.Q<VisualElement>("SlotContainer");

        _rootVisualElement.style.visibility = Visibility.Hidden;
        
        _isOpen = _rootVisualElement.visible;
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (_isOpen)
            {
                _isOpen = false;
                CloseInventory();
            }
            else
            {
                _isOpen = true;
                OpenInventory();
            }
        }
    }
}
