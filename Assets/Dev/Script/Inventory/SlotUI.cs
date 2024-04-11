using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SlotUI : VisualElement
{
    private VisualElement _itemIcon;
    private Label _itemAmountLabel;
    
    #region public method
    public SlotUI()
    {
        SetSlotUI();
    }

    public void UpdateSlotUI(Item item)
    {
        if (item == null)
        {
            _itemIcon = null;
            _itemAmountLabel.text = " ";
            return;
        }
        
        _itemIcon.style.backgroundImage = item.ItemData.ItemSprite.texture;

        if (item is not CountableItem)
        {
            _itemAmountLabel.text = " ";
        }

        _itemAmountLabel.text = ((CountableItem)item).CurrentAmount.ToString();
    }

    #endregion

    #region private method

        private void SetSlotUI()
        {
            AddToClassList("slot");
            style.flexGrow = 0f;
    
            VisualElement itemFrameUI = new VisualElement();
            _itemIcon = new VisualElement();
            _itemAmountLabel = new Label();
            _itemAmountLabel.style.fontSize = 25;
            _itemAmountLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            
            Add(itemFrameUI);
            
            itemFrameUI.AddToClassList("item_frame");
            itemFrameUI.Add(_itemIcon);
            
            _itemIcon.AddToClassList("item_icon");
            _itemIcon.Add(_itemAmountLabel);

        }
    #endregion
}