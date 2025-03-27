using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build; // For NamedBuildTarget support
using UnityEngine;

namespace DebugDrawer.Editor
{
    static class DebugDrawSettingsProvider
    {
        const string k_SettingsPath = "Project/DebugDraw";

        // Static field to persist the foldout state.
        private static bool _foldoutBuildTargets = true;

        [SettingsProvider]
        public static SettingsProvider CreateDebugDrawSettingsProvider()
        {
            var provider = new SettingsProvider(k_SettingsPath, SettingsScope.Project)
            {
                label = "DebugDraw",
                guiHandler = (searchContext) =>
                {
                    // --- Global Toggle for DebugDraw ---
                    EditorGUILayout.BeginHorizontal();
                    {
                        GUIContent enableInBuildlabelContent = new GUIContent("Enable in Build", "Toggle this option to enable DebugDraw in the build. When enabled, DebugDraw visuals will be included in targeted platforms. Only enable this if you want to see the DebugDraw visuals while running a build. Disable if only using DebugDraw in the Unity Editor.");
                        EditorGUILayout.LabelField(enableInBuildlabelContent);

                        EditorGUI.BeginChangeCheck();
                        DebugDrawSettings.instance.EnableDebugDraw = EditorGUILayout.Toggle(DebugDrawSettings.instance.EnableDebugDraw);
                        if(EditorGUI.EndChangeCheck())
                        {
                            EditorUtility.SetDirty(DebugDrawSettings.instance);
                            // Update the current build target's defines according to the global setting.
                            UpdateScriptingDefineSymbolsForCurrentTarget(DebugDrawSettings.instance.EnableDebugDraw);
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.HelpBox("Warning: Toggling 'Enable in Build' causes a recompile to strip DebugDraw from Build Targets.", MessageType.Warning, true);
                    EditorGUILayout.Space();

                    // --- Collapsible Build Target Overrides Section ---
                    _foldoutBuildTargets = EditorGUILayout.Foldout(_foldoutBuildTargets, "Build Targets", true);
                    if(_foldoutBuildTargets)
                    {
                        // If the global toggle is false, disable the build target override controls.
                        EditorGUI.BeginDisabledGroup(!DebugDrawSettings.instance.EnableDebugDraw);

                        // Sort the build target preferences alphabetically based on their BuildTargetGroup name.
                        List<DebugDrawBuildTargetPreference> sortedList = new List<DebugDrawBuildTargetPreference>(DebugDrawSettings.instance.buildTargetPreferences);
                        sortedList.Sort((a, b) => string.Compare(a.buildTargetGroup.ToString(), b.buildTargetGroup.ToString(), true));

                        int count = sortedList.Count;
                        int leftCount = Mathf.CeilToInt(count / 2f);

                        EditorGUILayout.BeginHorizontal();
                        {
                            // Left column.
                            EditorGUILayout.BeginVertical();
                            for(int i = 0; i < leftCount; i++)
                            {
                                DebugDrawBuildTargetPreference pref = sortedList[i];
                                EditorGUI.BeginChangeCheck();
                                bool newVal = EditorGUILayout.ToggleLeft(pref.buildTargetGroup.ToString(), pref.includeDebugDraw);
                                if(EditorGUI.EndChangeCheck())
                                {
                                    pref.includeDebugDraw = newVal;
                                    UpdateScriptingDefineSymbolsForTarget(pref);
                                }
                            }
                            EditorGUILayout.EndVertical();

                            // Right column.
                            EditorGUILayout.BeginVertical();
                            for(int i = leftCount; i < count; i++)
                            {
                                DebugDrawBuildTargetPreference pref = sortedList[i];
                                EditorGUI.BeginChangeCheck();
                                bool newVal = EditorGUILayout.ToggleLeft(pref.buildTargetGroup.ToString(), pref.includeDebugDraw);
                                if(EditorGUI.EndChangeCheck())
                                {
                                    pref.includeDebugDraw = newVal;
                                    UpdateScriptingDefineSymbolsForTarget(pref);
                                }
                            }
                            EditorGUILayout.EndVertical();
                        }
                        EditorGUILayout.EndHorizontal();

                        EditorGUI.EndDisabledGroup();
                    }
                },
                keywords = new HashSet<string>(new[] { "Debug", "Draw", "DebugDraw", "Build Targets" })
            };

            return provider;
        }

        /// <summary>
        /// Updates the scripting define symbols for the currently active build target.
        /// This is used for the global toggle.
        /// </summary>
        private static void UpdateScriptingDefineSymbolsForCurrentTarget(bool enable)
        {
            const string defineSymbol = "DEBUG_DRAW";

            // Determine the currently active build target group.
            BuildTargetGroup buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            // Convert BuildTargetGroup to NamedBuildTarget.
            NamedBuildTarget currentTarget = NamedBuildTarget.FromBuildTargetGroup(buildTargetGroup);

            // Retrieve current scripting defines.
            string defines = PlayerSettings.GetScriptingDefineSymbols(currentTarget);
            List<string> definesList = new List<string>(defines.Split(new char[] { ';' }, System.StringSplitOptions.RemoveEmptyEntries));

            if(enable)
            {
                if(!definesList.Contains(defineSymbol))
                    definesList.Add(defineSymbol);
            }
            else
            {
                definesList.Remove(defineSymbol);
            }

            string newDefines = string.Join(";", definesList);

            // Set DEBUG_DRAW compilation for current build target
            PlayerSettings.SetScriptingDefineSymbols(currentTarget, newDefines);

            Debug.Log("Updated scripting define symbols for " + currentTarget + ": " + newDefines);
        }

        /// <summary>
        /// Updates the scripting define symbols for a specific build target stored in the preference.
        /// </summary>
        private static void UpdateScriptingDefineSymbolsForTarget(DebugDrawBuildTargetPreference targetPref)
        {
            const string defineSymbol = "DEBUG_DRAW";
            // Convert the BuildTargetGroup to a NamedBuildTarget.
            NamedBuildTarget namedTarget = NamedBuildTarget.FromBuildTargetGroup(targetPref.buildTargetGroup);
            // Retrieve the current scripting defines for this target.
            string defines = PlayerSettings.GetScriptingDefineSymbols(namedTarget);
            List<string> definesList = new List<string>(defines.Split(new char[] { ';' }, System.StringSplitOptions.RemoveEmptyEntries));

            if(targetPref.includeDebugDraw)
            {
                if(!definesList.Contains(defineSymbol))
                    definesList.Add(defineSymbol);
            }
            else
            {
                definesList.Remove(defineSymbol);
            }

            string newDefines = string.Join(";", definesList);
            PlayerSettings.SetScriptingDefineSymbols(namedTarget, newDefines);

            Debug.Log("Updated scripting define symbols for " + targetPref.buildTargetGroup + " (" + namedTarget + "): " + newDefines);
        }
    }
}