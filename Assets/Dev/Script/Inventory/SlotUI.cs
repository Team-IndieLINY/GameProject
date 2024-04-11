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


    #endregion

    #region private method

        private void SetSlotUI()
        {
            AddToClassList("slot");
            style.flexGrow = 0f;
    
            VisualElement itemFrameUI = new VisualElement();
            _itemIcon = new VisualElement();
            _itemAmountLabel = new Label();
            
            Add(itemFrameUI);
            
            itemFrameUI.AddToClassList("item_frame");
            itemFrameUI.Add(_itemIcon);
            
            _itemIcon.AddToClassList("item_icon");
            _itemIcon.Add(_itemAmountLabel);

        }
    

    #endregion
}