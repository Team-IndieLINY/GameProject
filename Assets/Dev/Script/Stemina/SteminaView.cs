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

    public void UpdateView(float currentNormalizedValue)
    {
        _enduranceProgressBar.value = currentNormalizedValue * _enduranceProgressBar.highValue;
    }
}
