using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace DebugDrawer.Editor
{
    public class DebugDrawSettings : ScriptableObject
    {
        private const string k_SettingsAssetPath = "Assets/DebugDraw/Editor/DebugDrawSettings.asset";

        [SerializeField]
        private bool enableDebugDraw = false;
        public bool EnableDebugDraw
        {
            get => enableDebugDraw;
            set => enableDebugDraw = value;
        }

        // List of build target preferences.
        // Initially empty; it will be filled in OnEnable.
        public List<DebugDrawBuildTargetPreference> buildTargetPreferences = new List<DebugDrawBuildTargetPreference>();

        // Called automatically when the asset is loaded or created.
        void OnEnable()
        {
            // Retrieve all BuildTargetGroup values.
            var allGroups = (BuildTargetGroup[])Enum.GetValues(typeof(BuildTargetGroup));
            foreach(var group in allGroups)
            {
                // Skip the Unknown target.
                if(group == BuildTargetGroup.Unknown)
                    continue;

                // If this group isn't already present, add it.
                bool exists = buildTargetPreferences.Exists(x => x.buildTargetGroup == group);
                if(!exists)
                {
                    // Enable DebugDraw for certain targets by default.
                    // Here, we enable Standalone by default.
                    bool defaultEnabled = (group == BuildTargetGroup.Standalone);
                    buildTargetPreferences.Add(new DebugDrawBuildTargetPreference { buildTargetGroup = group, includeDebugDraw = defaultEnabled });
                }
            }
        }

        // Singleton Instance Accessor.
        static DebugDrawSettings Instance;
        public static DebugDrawSettings instance
        {
            get
            {
                if(Instance == null)
                {
                    Instance = AssetDatabase.LoadAssetAtPath<DebugDrawSettings>(k_SettingsAssetPath);
                    if(Instance == null)
                    {
                        Instance = CreateInstance<DebugDrawSettings>();
                        AssetDatabase.CreateAsset(Instance, k_SettingsAssetPath);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }
                }
                return Instance;
            }
        }

        // Loads Settings and Enqueues a Delayed Preferences Reset.
        public void LoadSettings()
        {
            EditorApplication.delayCall += ResetDebugDrawPreferences;
        }

        void ResetDebugDrawPreferences()
        {
            Debug.Log("Resetting DebugDraw Preferences...");
            // Instert internal state restting logic here/
        }
    }
}
