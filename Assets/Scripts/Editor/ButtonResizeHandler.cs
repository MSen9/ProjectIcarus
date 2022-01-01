using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(Button))]
public class ButtonResizeHandler : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        //DrawDefaultInspector();
        if (GUILayout.Button("Set Button"))
        {
            Button myScript = ((Button)target);
            myScript.ResizeButton();
            myScript.EditorMakeTextObj();
        }
    }
}
