using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WL_Wheel))]
public class WL_WheelEditor : Editor
{
    WL_Wheel tWhl;

    bool editorOpen = true;

    public override void OnInspectorGUI()
    {
        if(!editorOpen) base.OnInspectorGUI();
        tWhl = (WL_Wheel)target;

        editorOpen = EditorGUILayout.Foldout(editorOpen, (editorOpen ? "Close" : "Open") + " custom editor", true);
        if (editorOpen) CEditor();


    }

    void CEditor()
    {
        EditorGUILayout.Space();
        int _oldCuts = tWhl.Cuts;
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Number of cuts :");
        tWhl.Cuts = EditorGUILayout.IntSlider(tWhl.Cuts, 1, 180);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Circle size :");
        tWhl.Size = EditorGUILayout.FloatField(tWhl.Size);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        if(_oldCuts != tWhl.Cuts)
        {
            // update list

            List<Color> _oldColors = tWhl.AllColors;
            List<Material> _oldMats = tWhl.AllMats;
            tWhl.AllColors.Clear();
            tWhl.AllMats.Clear();
            for (int i = 0; i < tWhl.Cuts; i++)
            {
                tWhl.AllColors.Add(Color.black);
                tWhl.AllMats.Add(null);
            }
            for (int i = 0; i < tWhl.Cuts; i++)
            {
                if (i > _oldColors.Count - 1) break;
                tWhl.AllColors[i] = _oldColors[i];
            }
            for (int i = 0; i < tWhl.Cuts; i++)
            {
                if (i > _oldMats.Count - 1) break;
                tWhl.AllMats[i] = _oldMats[i];
            }

        }

        tWhl.UseColor = EditorGUILayout.ToggleLeft("Use custom " + (tWhl.UseColor ? "color" : "material"),tWhl.UseColor);
        if(tWhl.UseColor)
        {
            tWhl.MultipleColor = EditorGUILayout.ToggleLeft("Use multiple color", tWhl.MultipleColor);
            if(tWhl.MultipleColor)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("AllColors"), new GUIContent("All colors"), true);
            }
            else
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("SingleColor"), new GUIContent("Custom color"));
            }
        }
        else
        {
            tWhl.MultipleMat = EditorGUILayout.ToggleLeft("Use multiple Materials", tWhl.MultipleMat);
            if(tWhl.MultipleMat)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("AllMats"), new GUIContent("All materials"), true);
            }
            else
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("SingleMat"), new GUIContent("Custom material"));
            }
        }

        CEditorEvents();
    }

    void CEditorEvents()
    {

    }
}
