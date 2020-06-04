using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;

[CustomEditor(typeof(WL_Wheel))]
public class WL_WheelEditor : Editor
{
    WL_Wheel tWhl;

    bool editorOpen = true;
    bool imageOpen = true;

    public override void OnInspectorGUI()
    {
        editorOpen = EditorGUILayout.Foldout(editorOpen, (editorOpen ? "Close" : "Open") + " custom editor", true);
        if (!editorOpen) base.OnInspectorGUI();
        tWhl = (WL_Wheel)target;
        if (editorOpen && CheckAxis()) CEditor();
    }

    bool CheckAxis()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Horizontal axis of the controller :");
        tWhl.JoystickAxisHorizontal = EditorGUILayout.TextField(tWhl.JoystickAxisHorizontal);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Vertical axis of the controller :");
        tWhl.JoystickAxisVertical = EditorGUILayout.TextField(tWhl.JoystickAxisVertical);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Submit button of the controller :");
        tWhl.JoystickSubmit = EditorGUILayout.TextField(tWhl.JoystickSubmit);
        EditorGUILayout.EndHorizontal();
        try
        {
            Input.GetAxis(tWhl.JoystickAxisHorizontal);
        }
        catch(Exception e)
        {
            Debug.LogError($"Your Axis \"{tWhl.JoystickAxisHorizontal}\" is not valid for Horizontal axis. Please verify the spelling");
            return false;
        }
        try
        {
            Input.GetAxis(tWhl.JoystickAxisVertical);
        }
        catch (Exception e)
        {
            Debug.LogError($"Your Axis \"{tWhl.JoystickAxisVertical}\" is not valid for Vertical axis. Please verify the spelling");
            return false;
        }
        try
        {
            Input.GetButton(tWhl.JoystickSubmit);
        }
        catch (Exception e)
        {
            Debug.LogError($"Your button \"{tWhl.JoystickSubmit}\" is not valid for Submit button. Please verify the spelling");
            return false;
        }
        return true;
    }

    void CEditor()
    {
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Number of cuts :");
        tWhl.Cuts = EditorGUILayout.IntSlider(tWhl.Cuts, 1, 180);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Circle size :");
        tWhl.Size = EditorGUILayout.FloatField(tWhl.Size);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        tWhl.HighlighColor = EditorGUILayout.ColorField("Highlight color : ", tWhl.HighlighColor);
        EditorGUILayout.Space();
        CEditorImages();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        CEditorEvents();
    }

    void CEditorImages()
    {
        imageOpen = EditorGUILayout.Foldout(imageOpen, "All sprites", true);
        if (!imageOpen) return;
        for (int i = 0; i < tWhl.Cuts; i++)
        {
            if (tWhl.AllImages.Count > i) tWhl.AllImages[i] = (Sprite)EditorGUILayout.ObjectField(tWhl.AllImages[i], typeof(Sprite), true);
            else tWhl.AllImages.Add(default);
        }
    }


    void CEditorEvents()
    {
        SerializedProperty _s = serializedObject.FindProperty("AllEvents");
        EditorGUILayout.PropertyField(_s, true);
        serializedObject.ApplyModifiedProperties();
        List<UnityEvent> _oldEvents = new List<UnityEvent>();
        for (int i = 0; i < tWhl.Cuts; i++)
        {
            if (tWhl.AllEvents.Count > i) _oldEvents.Add(tWhl.AllEvents[i]);
        }
        tWhl.AllEvents.Clear();
        for (int i = 0; i < tWhl.Cuts; i++)
        {
            if (_oldEvents.Count > i) tWhl.AllEvents.Add(_oldEvents[i]);
            else tWhl.AllEvents.Add(new UnityEvent());
        }
    }
}
