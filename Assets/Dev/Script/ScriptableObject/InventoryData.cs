using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "IndieLINY/InventoryData", fileName = "InventoryData", order = int.MaxValue)]
public class InventoryData : ScriptableObject
{
    [SerializeField] private string _inventoryName;
    [SerializeField] private int _cellCount = 16;
    [SerializeField] private int _rowCount = 4;
    [SerializeField] private int _columnCount = 4;
    [SerializeField] private float _cellHorizontalSize = 1f;
    [SerializeField] private float _cellVerticalSize = 1f;
    
    public string InventoryName => _inventoryName;
    
    public int CellCount => _cellCount;
    
    [Obsolete("사용하지마시오")]
    public int RowCount => _rowCount;
    [Obsolete("사용하지마시오")]
    public int ColumnCount => _columnCount;

    [Obsolete("사용하지마시오")]
    public float CellHorizontalSize => _cellHorizontalSize;
    [Obsolete("사용하지마시오")]
    public float CellVerticalSize => _cellVerticalSize;
}
