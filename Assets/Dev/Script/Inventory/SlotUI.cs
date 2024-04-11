using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Cysharp.Threading.Tasks;
using IndieLINY.Singleton;
using UnityEngine;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;

public class SlotUI : VisualElement
{
    private VisualElement _itemIcon;
    private Label _itemAmountLabel;
    private ProgressBar _palmingProgressBar;
    
    public Slot Slot { get; set; }
    
    #region public method
    public SlotUI()
    {
        SetSlotUI();
    }

    public void UpdateSlotUI()
    {
        if (Slot.Item == null)
        {
            _itemIcon.style.backgroundImage = null;
            _itemAmountLabel.text = " ";
            return;
        }
        
        _itemIcon.style.backgroundImage = Slot.Item.ItemData.ItemSprite.texture;

        if (Slot.Item is not CountableItem)
        {
            _itemAmountLabel.text = " ";
        }

        _itemAmountLabel.text = ((CountableItem)Slot.Item).CurrentAmount.ToString();
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
            _itemAmountLabel.style.alignSelf = Align.FlexEnd;
            
            _palmingProgressBar = new ProgressBar();
            
            Add(itemFrameUI);
            
            itemFrameUI.AddToClassList("item_frame");
            itemFrameUI.Add(_itemIcon);
            
            _itemIcon.AddToClassList("item_icon");
            _itemIcon.Add(_palmingProgressBar);
            _itemIcon.Add(_itemAmountLabel);

            _palmingProgressBar.style.visibility = Visibility.Hidden;
            
            RegisterCallback<MouseEnterEvent>(OnMouseEnterEvent);
            RegisterCallback<MouseLeaveEvent>(OnMouseLeaveEvent);
            RegisterCallback<ClickEvent>(OnClickEvent);
        }

        private void OnMouseEnterEvent(MouseEnterEvent evt)
        {
            AddToClassList("highlighted_slot");
        }

        private async void OnClickEvent(ClickEvent evt)
        {
            if(panel.visualTree.name != "BoxInventory")
                return;

            if (Slot.IsEmpty())
                return;
            
            RemoveFromClassList("highlighted_slot");
            
            await Loot();
        }
        
        private void OnMouseLeaveEvent(MouseLeaveEvent evt)
        {
            RemoveFromClassList("highlighted_slot");
        }

        public async UniTask Loot()
        {
            _palmingProgressBar.style.visibility = Visibility.Visible;
            while (_palmingProgressBar.value <= _palmingProgressBar.highValue)
            {
                await UniTask.Delay((int)(Time.deltaTime * 10000));

                _palmingProgressBar.value += (int)(Time.deltaTime * 1000);
            }
            
            _palmingProgressBar.style.visibility = Visibility.Hidden;
            _palmingProgressBar.value = 0;

            if (PlayerInventory.Instance.IsFull())
            {
                return;
            }

            if (Slot.Item is CountableItem)
            {
                Item item = new CountableItem(Slot.Item.ItemData as CountableItemData, 1);
                (item as CountableItem).SetAmount(1);
                
                PlayerInventory.Instance.AddItem(item);
            }
            else
            {
                
                // 셀 수 있는 아이템이 아니면 해당 타입 아이템으로 생성하여 추가해야함
            }
            Slot.RemoveItem();
        }
    #endregion
}