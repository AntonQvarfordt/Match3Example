using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Jewel))]
public class JewelEditor : Editor {

    public override void OnInspectorGUI()
    {
        var jewelScript = (Jewel) target;

        DrawDefaultInspector();

        if (GUILayout.Button("Randomize Jewel"))
        {
            jewelScript.CreateJewelLocal();
        }
    }

}
