using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TooltipUI
{
    private VisualElement _tooltipVisualElement;
    private Label _itemNameLabel;
    private Label _itemDescriptionLabel;
    private VisualElement _itemIconVisualElement;

    private readonly Vector2 _tooltipPadding = new(35f, 35f);

    public TooltipUI(VisualElement tooltipVisualElement)
    {
        _tooltipVisualElement = tooltipVisualElement;
        _itemNameLabel = tooltipVisualElement.Q<Label>("ItemName");
        _itemDescriptionLabel = tooltipVisualElement.Q<Label>("ItemDescription");
        _itemIconVisualElement = tooltipVisualElement.Q<VisualElement>("ItemIcon");
    }

    public void OpenTooltipUI()
    {
        _tooltipVisualElement.style.visibility = Visibility.Visible;
    }

    public void CloseTooltipUI()
    {
        _tooltipVisualElement.style.visibility = Visibility.Hidden;
    }

    public bool IsOpened()
    {
        return _tooltipVisualElement.visible;
    }

    public void SetTooltipPosition(Vector2 mousePosition)
    {
        _tooltipVisualElement.style.left = mousePosition.x + _tooltipPadding.x;
        _tooltipVisualElement.style.top = mousePosition.y + _tooltipPadding.y;
    }

    public void SetTooltipUI(Item item)
    {
        SetItemName(item.ItemData.ItemName);
        SetItemDescription(item.ItemData.ItemDescription);
        SetItemIcon(item.ItemData.ItemSprite);
    }

    private void SetItemName(string itemName)
    {
        _itemNameLabel.text = itemName;
    }
    private void SetItemDescription(string itemDescription)
    {
        _itemDescriptionLabel.text = itemDescription;
    }
    private void SetItemIcon(Sprite itemIcon)
    {
        _itemIconVisualElement.style.backgroundImage = itemIcon.texture;
    }
}
