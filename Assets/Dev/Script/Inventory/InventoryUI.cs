using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public abstract class InventoryUI : MonoBehaviour
{
    protected UIDocument _inventoryUIDocument;
    protected VisualElement _rootVisualElement;
    protected VisualElement _containerElement;
    protected VisualElement _slotContainerVisualElement;
    protected Label _headLabel;

    protected Inventory _inventory;
    protected TooltipUI _tooltipUI;

    public VisualElement SlotContainerVisualElement => _slotContainerVisualElement;
    
    #region public method
    public void OpenInventory()
    {
        _rootVisualElement.style.visibility = Visibility.Visible;
    }
    
    public void CloseInventory()
    {
        _rootVisualElement.style.visibility = Visibility.Hidden;
    }

    public bool IsOpened()
    {
        return _rootVisualElement.visible;
    }

    public void SetTooltipUI(Item item)
    {
        _tooltipUI.SetTooltipUI(item);
    }

    public void OpenTooltipUI()
    {
        _tooltipUI.OpenTooltipUI();
    }
    
    public void CloseTooltipUI()
    {
        _tooltipUI.CloseTooltipUI();
    }

    public void SetTooltipUIPosition(Vector2 mousePosition)
    {
        _tooltipUI.SetTooltipPosition(mousePosition);
    }
    
    public void SetInventoryUI(InventoryData inventoryData)
    {
        _headLabel.text = inventoryData.InventoryName;
    }

    public void ResetInventoryUI(InventoryData inventoryData)
    {
        while (_slotContainerVisualElement.childCount <= 0)
        {
            _slotContainerVisualElement.RemoveAt(0);
        }
        
        SetInventoryUI(inventoryData);
    }
    
    #endregion
    #region private method
    
    #endregion
}
