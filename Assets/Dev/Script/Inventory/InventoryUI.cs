using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

[RequireComponent(typeof(Inventory))]
[RequireComponent(typeof(UIDocument))]
public class InventoryUI : MonoBehaviour
{
    private UIDocument _inventoryUIDocument;
    private VisualElement _rootVisualElement;
    private VisualElement _containerElement;
    private VisualElement _bodyVisualElement;
    private Label _headLabel;

    private bool _isOpen;
    
    #region event method
    private void Awake()
    {
        _inventoryUIDocument = GetComponent<UIDocument>();
        _rootVisualElement = _inventoryUIDocument.rootVisualElement;
        _containerElement = _rootVisualElement.Q<VisualElement>("Container");
        _headLabel = _rootVisualElement.Q<Label>("HeadLabel");
        _bodyVisualElement = _rootVisualElement.Q<VisualElement>("Body");

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

    #endregion

    #region public method
    public void OpenInventory()
    {
        _rootVisualElement.style.visibility = Visibility.Visible;
    }
    
    public void CloseInventory()
    {
        _rootVisualElement.style.visibility = Visibility.Hidden;
    }

    public void SetInventoryUI(InventoryData inventoryData)
    {
        _headLabel.text = inventoryData.InventoryName;

        for (int i = 0; i < inventoryData.CellCount; i++)
            _bodyVisualElement.Add(GenerateSlotVisual());

        
    }
    #endregion
    #region private method

    private VisualElement GenerateSlotVisual()
    {
        VisualElement slotVisual = new VisualElement();
        slotVisual.AddToClassList("slot");
        slotVisual.style.flexGrow = 0f;
        
        return slotVisual;
    }
    #endregion
}
