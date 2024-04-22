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
    private VisualElement _clockVisualElement;
    private VisualElement _fadingImageVisualElement;
    private Label _dayLabel;

    private void Awake()
    {
        _uiDocument = GetComponent<UIDocument>();
        _rootVisualElement = _uiDocument.rootVisualElement;
        _containerVisualElement = _rootVisualElement.Q<VisualElement>("Container");
        _button = _containerVisualElement.Q<Button>("GoOutsideButton");
        _clockVisualElement = _containerVisualElement.Q<VisualElement>("Clock");
        _fadingImageVisualElement = _containerVisualElement.Q<VisualElement>("FadingImage");
        _dayLabel = _containerVisualElement.Q<Label>("DayLabel");
        
        _clockVisualElement.RegisterCallback<ClickEvent>(OnClickClockVisualElement);
        _button.RegisterCallback<ClickEvent>(LoadOutSideScene);
        _fadingImageVisualElement.RegisterCallback<TransitionEndEvent>(OnFadingImageTransitionEnd);
        _dayLabel.RegisterCallback<TransitionEndEvent>(OnDayLabelTransitionEnd);
    }

    public void LoadOutSideScene(ClickEvent evt)
    {
        StartCoroutine(LoadOutSideSceneAsync());
    }

    private IEnumerator LoadOutSideSceneAsync()
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("hapchigi");
        asyncOperation.allowSceneActivation = false;
        
        _fadingImageVisualElement.AddToClassList("fade_out");
        
        yield return new WaitForSeconds(2f);
        
        asyncOperation.allowSceneActivation = true;
    }

    private void OnFadingImageTransitionEnd(TransitionEndEvent evt)
    {
        if(_fadingImageVisualElement.ClassListContains("fade_out"))
        {
            FadeInDayLabel();

            return;
        }
        
        _fadingImageVisualElement.pickingMode = PickingMode.Ignore;
    }

    private void OnDayLabelTransitionEnd(TransitionEndEvent evt)
    {
        if (_dayLabel.ClassListContains("day_label_fade_in"))
        {
            FadeOutDayLabel();
            FadeIn();
        }
    }
    
    private void OnClickClockVisualElement(ClickEvent evt)
    {
        TimeManager.Instance.EndRoutine();
        _dayLabel.text = $"{TimeManager.Instance.DayCount} Day";
        FadeOut();
    }
    
    private void FadeOut()
    {
        _fadingImageVisualElement.pickingMode = PickingMode.Position;
        _fadingImageVisualElement.AddToClassList("fade_out");
    }

    private void FadeIn()
    {
        _fadingImageVisualElement.RemoveFromClassList("fade_out");
    }
    
    private void FadeInDayLabel()
    {
        _dayLabel.AddToClassList("day_label_fade_in");
    }

    private void FadeOutDayLabel()
    {
        _dayLabel.RemoveFromClassList("day_label_fade_in");
    }
}
