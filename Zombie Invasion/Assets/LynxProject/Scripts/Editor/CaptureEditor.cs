using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Capture))]
public class CaptureEditor : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Capture captureScript = (Capture)target;
        if (GUILayout.Button("Capture"))
        {
            captureScript.SaveScreenshot();
        }
    }
}
