using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor {

    public override void OnInspectorGUI()
    {
        var managerScript = (GameManager)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Create Random Jewel"))
        {
            managerScript.CreateRandomJewel();
        }
    }

}
