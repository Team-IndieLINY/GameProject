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
    protected VisualElement _bodyVisualElement;
    protected Label _headLabel;

    protected Inventory _inventory;
    protected bool _isOpen;

    public VisualElement BodyVisualElement => _bodyVisualElement;
    
    #region event method


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
    }

    public void ResetInventoryUI(InventoryData inventoryData)
    {
        while (_bodyVisualElement.childCount <= 0)
        {
            _bodyVisualElement.RemoveAt(0);
        }
        
        SetInventoryUI(inventoryData);
    }
    #endregion
    #region private method
    
    #endregion
}
