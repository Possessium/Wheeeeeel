using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;

[CustomEditor(typeof(WL_Wheel))]
public class WL_WheelEditor : Editor
{
    WL_Wheel tWhl;

    bool editorOpen = true;
    bool openMultiple = true;

    public override void OnInspectorGUI()
    {
        if (!editorOpen) base.OnInspectorGUI();
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
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Circle size :");
        tWhl.Size = EditorGUILayout.FloatField(tWhl.Size);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        if (_oldCuts != tWhl.Cuts)
        {
            // update list

            List<Color> _oldColors = new List<Color>();
            for (int i = 0; i < tWhl.AllColors.Count; i++)
            {
                _oldColors.Add(tWhl.AllColors[i]);
            }
            List<UnityEvent> _oldEvents = new List<UnityEvent>();
            for (int i = 0; i < tWhl.AllEvents.Count; i++)
            {
                _oldEvents.Add(tWhl.AllEvents[i]);
            }
            List<Material> _oldMats = new List<Material>();
            for (int i = 0; i < tWhl.AllMats.Count; i++)
            {
                _oldMats.Add(tWhl.AllMats[i]);
            }
            tWhl.AllColors.Clear();
            tWhl.AllMats.Clear();
            tWhl.AllEvents.Clear();
            for (int i = 0; i < tWhl.Cuts; i++)
            {
                tWhl.AllColors.Add(Color.black);
                tWhl.AllMats.Add(null);
                tWhl.AllEvents.Add(new UnityEvent());
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
            for (int i = 0; i < tWhl.Cuts; i++)
            {
                if (i > _oldEvents.Count - 1) break;
                tWhl.AllEvents[i] = _oldEvents[i];
            }

        }

        EditorGUILayout.BeginHorizontal();
        tWhl.UseColor = EditorGUILayout.ToggleLeft("Use custom Color :", tWhl.UseColor);
        tWhl.UseColor = !EditorGUILayout.ToggleLeft("Use custom Material :", !tWhl.UseColor);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        if (tWhl.UseColor) CEditorColor();
        else CEditorMaterial();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        CEditorEvents();
    }

    void CEditorColor()
    {
        tWhl.HighlighColor = EditorGUILayout.ColorField("Highlight color : ", tWhl.HighlighColor);
        tWhl.MultipleColor = EditorGUILayout.ToggleLeft("Use multiple color", tWhl.MultipleColor);
        if (tWhl.MultipleColor)
        {
            openMultiple = EditorGUILayout.Foldout(openMultiple, (openMultiple ? "Close" : "Open") + " color list", true);
            if (openMultiple)
            {
                EditorGUILayout.BeginVertical();
                for (int i = 0; i < tWhl.AllColors.Count; i++)
                {
                    tWhl.AllColors[i] = EditorGUILayout.ColorField(tWhl.AllColors[i]);
                }
                EditorGUILayout.EndVertical();
            }
        }
        else
        {
            tWhl.SingleColor = EditorGUILayout.ColorField(tWhl.SingleColor);
        }
    }

    void CEditorMaterial()
    {
        tWhl.MultipleMat = EditorGUILayout.ToggleLeft("Use multiple Materials", tWhl.MultipleMat);
        if (tWhl.MultipleMat)
        {
            openMultiple = EditorGUILayout.Foldout(openMultiple, (openMultiple ? "Close" : "Open") + " material list", true);
            if (openMultiple)
            {
                EditorGUILayout.BeginVertical();
                for (int i = 0; i < tWhl.AllMats.Count; i++)
                {
                    tWhl.AllMats[i] = (Material)EditorGUILayout.ObjectField(tWhl.AllMats[i], typeof(Material), true);
                }
                EditorGUILayout.EndVertical();
            }
        }
        else
        {
            tWhl.SingleMat = (Material)EditorGUILayout.ObjectField(tWhl.SingleMat, typeof(Material), true);
        }
    }

    void CEditorEvents()
    {
        SerializedProperty _s = serializedObject.FindProperty("AllEvents");
        EditorGUILayout.PropertyField(_s, true);
        serializedObject.ApplyModifiedProperties();
        if (tWhl.AllEvents.Count != tWhl.Cuts)
        {
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
}
