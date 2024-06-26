using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Cysharp.Threading.Tasks;
using IndieLINY.Singleton;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;

public class SlotUI : VisualElement
{
    private VisualElement _itemIcon;
    private Label _itemAmountLabel;
    private ProgressBar _palmingProgressBar;
    private CancellationTokenSource _cancellationTokenSource = new();

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

    //해당 슬롯이 루팅되고 있는 지 알려주는 함수
    public bool IsLooting()
    {
        if (_palmingProgressBar.value != 0)
        {
            return true;
        }

        return false;
    }

    public void CancelLoot()
    {
        if (_cancellationTokenSource == null)
        {
            return;
        }

        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
        _cancellationTokenSource = null;
    }

    public void SetCancellaionTokenSoruce()
    {
        if (_cancellationTokenSource == null)
        {
            _cancellationTokenSource = new CancellationTokenSource();
        }
    }

    public async UniTask Loot()
    {
        SetCancellaionTokenSoruce();
        _palmingProgressBar.style.visibility = Visibility.Visible;

        while (_palmingProgressBar.value <= _palmingProgressBar.highValue)
        {
            try
            {
                await UniTask.Delay((int)Time.deltaTime * 1000, cancellationToken: _cancellationTokenSource.Token);
            }
            catch
            {
                _palmingProgressBar.style.visibility = Visibility.Hidden;
                _palmingProgressBar.value = 0;
                throw;
            }

            _palmingProgressBar.value +=
                _palmingProgressBar.highValue * Time.deltaTime / Slot.Item.ItemData.LootingTime;
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

    private void OnClickEvent(ClickEvent evt)
    {
        if (panel.visualTree.name != "BoxInventory")
            return;

        if (Slot.IsEmpty())
            return;

        RemoveFromClassList("highlighted_slot");

        if (_cancellationTokenSource == null)
        {
            _cancellationTokenSource = new CancellationTokenSource();
        }

        if (BoxInventory.Instance.IsLooting())
        {
            BoxInventory.Instance.CancelLooting();
            return;
        }



        if (_palmingProgressBar.value != 0)
            CancelLoot();
        else
            Loot();
    }

    private void OnMouseLeaveEvent(MouseLeaveEvent evt)
    {
        RemoveFromClassList("highlighted_slot");
    }

    #endregion
}