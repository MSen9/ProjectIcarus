using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(PointManager))]
public class PointManagerEditor : Editor
{
    /*
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        //DrawDefaultInspector();
        if (GUILayout.Button("Set all vertexes to full size"))
        {
            PointManager myScript = ((PointManager)target);
            myScript.InstantGrowAllVertexes();
        }
    }
    */
}
