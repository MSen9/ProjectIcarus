using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(AllPointManager))]
public class PointManagerEditor : Editor
{
    
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        //DrawDefaultInspector();
        if (GUILayout.Button("Set all vertexes to full size"))
        {
            AllPointManager myScript = ((AllPointManager)target);
            myScript.InstantGrowAllVertexes();
        }
    }
    
}
