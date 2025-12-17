using UnityEditor;
using UnityEngine;
using Proselyte.DebugDrawer.Editor;

[CustomEditor(typeof(DebugDrawSettings))]
public class DebugDrawSettingsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Begin a vertical block that looks like a help box.
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        {
            // Reserve a rect for the custom help box.
            Rect rect = EditorGUILayout.GetControlRect(true, 50, EditorStyles.helpBox);

            // Get the built-in warning icon.
            GUIContent warningIcon = EditorGUIUtility.IconContent("console.warnicon");

            // Draw the warning icon inside the help box.
            Rect iconRect = new Rect(rect.x + 5, rect.y - 10, 64, 64);
            GUI.Label(iconRect, warningIcon);

            // Align the button to the right.
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                if(GUILayout.Button("Open Project Settings", GUILayout.MaxWidth(150)))
                {
                    // Open the DebugDraw settings page.
                    SettingsService.OpenProjectSettings("Project/DebugDraw");
                }
            }
            EditorGUILayout.EndHorizontal();

            // Draw the warning text. Adjust its X position so it doesn't overlap the icon.
            Rect labelRect = new Rect(rect.x + 64, rect.y + 5, rect.width - 64, rect.height - 10);
            GUI.Label(labelRect,
                "Modify DebugDraw settings from the Project Settings window.",
                EditorStyles.wordWrappedLabel);
        }
        EditorGUILayout.EndVertical();


        // Display a read-only label of the current setting
        DebugDrawSettings settings = (DebugDrawSettings)target;
        EditorGUI.BeginDisabledGroup(true);
        base.OnInspectorGUI();
        EditorGUI.EndDisabledGroup();
    }
}
