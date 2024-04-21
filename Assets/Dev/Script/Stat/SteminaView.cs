using System;
using System.Collections;
using System.Collections.Generic;
using IndieLINY;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class SteminaView : MonoBehaviour
{
    private UIDocument _uiDocument;
    private VisualElement _rootVisualElement;
    private ProgressBar _enduranceProgressBar;
    
    private void Awake()
    {
        _uiDocument = GetComponent<UIDocument>();
        _rootVisualElement = _uiDocument.rootVisualElement;
        _enduranceProgressBar = _rootVisualElement.Q<ProgressBar>("EnduranceProgressBar");
    }

    public void SetSteminaView(int minValue, int maxValue, int currentValue)
    {
        _enduranceProgressBar.highValue = maxValue;
        _enduranceProgressBar.lowValue = minValue;
        _enduranceProgressBar.value = currentValue;
    }
    public void UpdateView(int currentStemina)
    {
        _enduranceProgressBar.value = currentStemina;
    }
}
