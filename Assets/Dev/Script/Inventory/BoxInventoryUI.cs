using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BoxInventoryUI : InventoryUI
{
    private Button _takeAllButton;
    private void Awake()
    {
        Debug.Assert(TryGetComponent(out BoxInventory inventory), "inventory is null");
        _inventory = inventory;
        
        _inventoryUIDocument = GetComponent<UIDocument>();
        _rootVisualElement = _inventoryUIDocument.rootVisualElement;
        _containerElement = _rootVisualElement.Q<VisualElement>("Container");
        _headLabel = _rootVisualElement.Q<Label>("HeadLabel");
        _slotContainerVisualElement = _rootVisualElement.Q<VisualElement>("SlotContainer");
        
        _takeAllButton = _rootVisualElement.Q<Button>("TakeAllButton");
        _takeAllButton.RegisterCallback<ClickEvent>(OnClickTakeAllButton);
        
        // _rootVisualElement.style.visibility = Visibility.Hidden;
        
        _isOpen = _rootVisualElement.visible;
    }
    
    

    public async void OnClickTakeAllButton(ClickEvent evt)
    {
        if (PlayerInventory.Instance.IsFull())
        {
            return;
        }

        if (BoxInventory.Instance.IsLooting())
        {
            BoxInventory.Instance.CancelLooting();
            return;
        }

        
        for (int i = 0; i < BoxInventory.Instance.Slots.Length; i++)
        {
            if (BoxInventory.Instance.Slots[i].IsEmpty())
            {
                continue;
            }

            try
            {
                await BoxInventory.Instance.Slots[i].SlotUI.Loot();
            }
            catch
            {
                return;
            }

            i--;
        }
    }
}
