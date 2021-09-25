using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(VertexHandling))]
public class VertexHandlerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        //DrawDefaultInspector();
        if (GUILayout.Button("Set to full size"))
        {
            VertexHandling myScript = ((VertexHandling)target);
            myScript.InstantGrowVertexes();
        }
    }
}
