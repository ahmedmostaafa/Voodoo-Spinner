using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace KabreetGames.BladeSpinner.Editor
{
    public class StatModifierWindow : EditorWindow
    {
        private WaveUpgradeData waveUpgradeData;
        private readonly List<string> waveUpgradeEventNames = new();
        private Vector2 scrollPosition = Vector2.zero;

        [MenuItem("Window/Stats/Stat Modifier")]
        public static void ShowWindow()
        {
            GetWindow(typeof(StatModifierWindow));
        }

        private void OnEnable()
        {
            waveUpgradeData = WaveUpgradeData.Instance;

            foreach (var type in Assembly.GetAssembly(typeof(WaveUpgradeEvent)).GetTypes())
            {
                if (!type.IsSubclassOf(typeof(WaveUpgradeEvent))) continue;
                waveUpgradeEventNames.Add(type.Name);
            }
        }

        private void OnGUI()
        {
            DrawToolbar();
            DisplayUpgrade();
        }

        private void DisplayUpgrade()
        {
            var upgrades = waveUpgradeData.waveUpgrades;
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandHeight(true));
            foreach (var upgrade in upgrades)
            {
                switch (upgrade)
                {
                    case StatWaveUpgrade statUpgrade:
                    {
                        GUILayout.BeginHorizontal();
                        var newIcon = (Sprite)EditorGUILayout.ObjectField(upgrade.Icon, typeof(Sprite), false,
                            GUILayout.Height(50), GUILayout.Width(50));
                        GUILayout.Label(upgrade.Name);

                        var newValue = EditorGUILayout.FloatField("Value", statUpgrade.statModifier.Value);
                        if (GUILayout.Button("X", GUILayout.Width(20)))
                        {
                            waveUpgradeData.RemoveWaveUpgrade(upgrade);
                            EditorUtility.SetDirty(waveUpgradeData);
                        }

                        GUILayout.EndHorizontal();
                        if (newIcon != upgrade.Icon)
                        {
                            upgrade.Icon = newIcon;
                            EditorUtility.SetDirty(waveUpgradeData);
                        }

                        if (Mathf.Approximately(newValue, statUpgrade.statModifier.Value)) continue;
                        statUpgrade.statModifier.SetValue(newValue);
                        EditorUtility.SetDirty(waveUpgradeData);
                        break;
                    }

                    case OtherWaveUpgrade otherUpgrade:
                    {
                        GUILayout.BeginHorizontal();
                        var newIcon = (Sprite)EditorGUILayout.ObjectField(upgrade.Icon, typeof(Sprite), false,
                            GUILayout.Height(50), GUILayout.Width(50));

                        GUILayout.Label(upgrade.Name);

                        otherUpgrade.displayName = EditorGUILayout.TextField("Value", otherUpgrade.displayName);

                        if (GUILayout.Button("X", GUILayout.Width(20)))
                        {
                            waveUpgradeData.RemoveWaveUpgrade(upgrade);
                            EditorUtility.SetDirty(waveUpgradeData);
                        }

                        GUILayout.EndHorizontal();
                        if (newIcon != upgrade.Icon)
                        {
                            upgrade.Icon = newIcon;
                            EditorUtility.SetDirty(waveUpgradeData);
                        }

                        break;
                    }

                    case EventWaveUpgrade eventUpgrade:
                    {
                        GUILayout.BeginHorizontal();
                        var newIcon = (Sprite)EditorGUILayout.ObjectField(upgrade.Icon, typeof(Sprite), false,
                            GUILayout.Height(50), GUILayout.Width(50));

                        GUILayout.Label(upgrade.Name);
                        eventUpgrade.displayName = EditorGUILayout.TextField("Value", eventUpgrade.displayName);
                        if (GUILayout.Button("X", GUILayout.Width(20)))
                        {
                            waveUpgradeData.RemoveWaveUpgrade(upgrade);
                            EditorUtility.SetDirty(waveUpgradeData);
                        }

                        GUILayout.EndHorizontal();
                        if (newIcon != upgrade.Icon)
                        {
                            upgrade.Icon = newIcon;
                            EditorUtility.SetDirty(waveUpgradeData);
                        }

                        break;
                    }
                }
            }

            GUILayout.EndScrollView();
        }

        private void DrawToolbar()
        {
            GUILayout.BeginHorizontal("Toolbar");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Add Modifier", "ToolbarButton"))
            {
                CreateModifiedStatWindow.ShowWindow(new Rect(position.x, position.y, 0, 0));
            }

            GUILayout.EndHorizontal();
        }
    }
}