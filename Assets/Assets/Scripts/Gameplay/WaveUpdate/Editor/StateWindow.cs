using System;
using KabreetGames.BladeSpinner.TowerSystem.Stats;
using UnityEditor;
using UnityEngine;

namespace KabreetGames.BladeSpinner.Editor
{
    public class StateWindow : EditorWindow
    {
        private WaveUpgradeData waveUpgradeData;

        [MenuItem("Window/Stats/State Window")]
        public static void ShowWindow()
        {
            GetWindow<StateWindow>("StateWindow");
        }

        private void OnEnable()
        {
            waveUpgradeData = WaveUpgradeData.Instance;
        }


        private void OnGUI()
        {
            DrawToolbar();
            DisplayStats();
        }

        private void DisplayStats()
        {
            var statNames = waveUpgradeData.GetStatNames();
            foreach (var statName in statNames)
            {
                var hasStat = waveUpgradeData.TryGetStat(statName, out var stat);
                if (!hasStat) continue;
                float newValue;
                string displayName;
                switch (stat)
                {
                    case Stat<float> statFloat:
                    {
                        GUILayout.BeginHorizontal();
                        displayName = $"{statName} (float) => {statFloat.ModifierCount} : {statFloat.Value}";
                        newValue = EditorGUILayout.FloatField(displayName, statFloat.baseValue,
                            GUILayout.ExpandWidth(true));
                        if (GUILayout.Button("X", GUILayout.Width(20)))
                        {
                            waveUpgradeData.RemoveStat(statName);
                            EditorUtility.SetDirty(waveUpgradeData);
                        }

                        GUILayout.EndHorizontal();
                        if (Mathf.Approximately(newValue, statFloat.baseValue)) continue;
                        statFloat.baseValue = newValue;
                        EditorUtility.SetDirty(waveUpgradeData);
                        break;
                    }
                    case Stat<int> statInt:
                    {
                        displayName = $"{statName} (int) => {statInt.ModifierCount} : {statInt.Value}";
                        GUILayout.BeginHorizontal();
                        newValue = EditorGUILayout.IntField(displayName, statInt.baseValue,
                            GUILayout.ExpandWidth(true));
                        if (GUILayout.Button("X", GUILayout.Width(20)))
                        {
                            waveUpgradeData.RemoveStat(statName);
                            EditorUtility.SetDirty(waveUpgradeData);
                        }

                        GUILayout.EndHorizontal();
                        if (Mathf.Approximately(newValue, statInt.baseValue)) continue;
                        statInt.baseValue = (int)newValue;
                        EditorUtility.SetDirty(waveUpgradeData);
                        break;
                    }
                    default:
                        Debug.Log("Not Found");
                        break;
                }
            }
        }

        private void DrawToolbar()
        {
            GUILayout.BeginHorizontal("Toolbar");
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Add Stat", "ToolbarButton"))
            {
                CreateStatWindow.ShowWindow(new Rect(position.x, position.y, 0, 0));
            }

            GUILayout.EndHorizontal();
        }
    }
}