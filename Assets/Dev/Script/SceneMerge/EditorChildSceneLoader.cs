#if UNITY_EDITOR
 
using UnityEditor;
using UnityEngine;
 
[CustomEditor(typeof(EditorChildSceneLoader))]
public class ChildSceneLoaderInspectorGUI : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
 
        var currentInspectorObject = (EditorChildSceneLoader)target;
 
        if (GUILayout.Button("Save scene setup to config"))
        {
            currentInspectorObject.SaveSceneSetup();
        }
 
        if (GUILayout.Button("Reset scene setup from config..."))
        {
            currentInspectorObject.ResetSceneSetupToConfig();
        }
    }
}
 
#endif