using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WL_Wheel))]
public class WL_WheelEditor : Editor
{
    WL_Wheel targetWheel;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        targetWheel = (WL_Wheel)target;
    }
}

