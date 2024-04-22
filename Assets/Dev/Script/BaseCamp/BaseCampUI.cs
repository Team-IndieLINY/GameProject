using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class BaseCampUI : MonoBehaviour
{
    private UIDocument _uiDocument;
    private VisualElement _rootVisualElement;
    private VisualElement _containerVisualElement;
    private Button _button;

    private void Awake()
    {
        _uiDocument = GetComponent<UIDocument>();
        _rootVisualElement = _uiDocument.rootVisualElement;
        _containerVisualElement = _rootVisualElement.Q<VisualElement>("Container");
        _button = _containerVisualElement.Q<Button>("GoOutsideButton");
        _button.RegisterCallback<ClickEvent>(LoadOutSideScene);
    }

    public void LoadOutSideScene(ClickEvent evt)
    {
        SceneManager.LoadScene("hapchigi");
    }
}
