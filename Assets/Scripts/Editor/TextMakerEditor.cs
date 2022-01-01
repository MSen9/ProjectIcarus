using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(TextMaker))]
public class TextMakerEditor : Editor
{
    
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        //DrawDefaultInspector();
        if (GUILayout.Button("Make Vectors"))
        {
            TextMaker myScript = ((TextMaker)target);
            myScript.EditorMakeString();
        }
    }
    
}
