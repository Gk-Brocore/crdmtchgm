using Game.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(TextUpdate))]
public class TextUpdateEditor : Editor
{
    private readonly string[] popupOptions =
    { "int", "float", "string" };

    private int selectedIndex = 0;
    private GUIStyle popupStyle;
    public override void OnInspectorGUI()
    {
        TextUpdate textUpdate = (TextUpdate)target;
        if (popupStyle == null)
        {
            popupStyle = new GUIStyle(GUI.skin.GetStyle("PaneOptions"));
            popupStyle.imagePosition = ImagePosition.ImageOnly;
        }
        selectedIndex = EditorGUILayout.Popup("Variable Type", selectedIndex, popupOptions);

        switch(selectedIndex)
        {
            case 0:
                
                textUpdate.IntVar = (Game.Variables.IntVar)EditorGUILayout.ObjectField("Int Variable", textUpdate.IntVar, typeof(Game.Variables.IntVar), false);
                textUpdate.FloatVar = null;
                textUpdate.StringVar = null;
                break;
            case 1:
                textUpdate.FloatVar = (Game.Variables.FloatVar)EditorGUILayout.ObjectField("Float Variable", textUpdate.FloatVar, typeof(Game.Variables.FloatVar), false);
                textUpdate.IntVar = null;
                textUpdate.StringVar = null;
                break;
            case 2:
                textUpdate.StringVar = (Game.Variables.StringVar)EditorGUILayout.ObjectField("String Variable", textUpdate.StringVar, typeof(Game.Variables.StringVar), false);
                textUpdate.IntVar = null;
                textUpdate.FloatVar = null;
                break;
        }

        textUpdate.UiText = (TMPro.TMP_Text)EditorGUILayout.ObjectField("UI Text", textUpdate.UiText, typeof(TMPro.TMP_Text), true);

    }

    
}
