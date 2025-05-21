using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using KabreetGames.BladeSpinner.TowerSystem.Stats;
using UnityEditor;
using UnityEngine;

namespace KabreetGames.BladeSpinner.Editor
{
    public class CreateModifiedStatWindow : EditorWindow
    {
        private readonly List<Type> statModifierTypes = new();
        private readonly List<string> waveUpgradeTypesNames = new();
        private readonly List<string> statNames = new();
        private List<Type> waveUpgradeEventTypes = new();

        private WaveUpgradeData waveUpgradeData;

        private int upgradeTypeIndex;
        private int statIndex;
        private int eventIndex;
        private float statValue;
        private int modifierType;
        private string statName;

        public static void ShowWindow(Rect buttonRect)
        {
            var window = CreateInstance<CreateModifiedStatWindow>();
            var windowSize = new Vector2(250, 180);
            window.ShowAsDropDown(new Rect(buttonRect.x + 50, buttonRect.y, 0, 0), windowSize);
        }

        private void OnEnable()
        {
            waveUpgradeData = WaveUpgradeData.Instance;

            foreach (var type in Assembly.GetAssembly(typeof(WaveUpgrade)).GetTypes())
            {
                if (!type.IsSubclassOf(typeof(WaveUpgrade))) continue;
                waveUpgradeTypesNames.Add(type.Name);
            }          
            
            foreach (var type in Assembly.GetAssembly(typeof(StatModifier)).GetTypes())
            {
                if (!type.IsSubclassOf(typeof(StatModifier))) continue;
                statModifierTypes.Add(type);
            }
            

            waveUpgradeEventTypes = Assembly.GetAssembly(typeof(WaveUpgradeEvent))
                .GetTypes().Where(t => t.IsSubclassOf(typeof(WaveUpgradeEvent))).ToList();

            upgradeTypeIndex = 0;
            statIndex = 0;
            statValue = 1;
        }

        private void OnGUI()
        {
            var statType = DrawDropdown();
            if (statType.Equals(nameof(StatWaveUpgrade)))
            {
                statIndex = EditorGUILayout.Popup("Stat", statIndex, waveUpgradeData.GetStatNames().ToArray());
                modifierType = EditorGUILayout.Popup("Modifier Type", modifierType, statModifierTypes.Select(t => t.Name).ToArray());
                statValue = EditorGUILayout.FloatField("Value", statValue);
                var stat = waveUpgradeData.Stats[statIndex].statName;

                GUILayout.FlexibleSpace();
                if (!GUILayout.Button("Create Modifier")) return;
                var statModifier = (StatModifier)Activator.CreateInstance(statModifierTypes[modifierType], statValue);
                var waveUpgrade = new StatWaveUpgrade(stat, statModifier);
                waveUpgradeData.AddWaveUpgrade(waveUpgrade);
                EditorUtility.SetDirty(waveUpgradeData);
                Close();
            }
            else if (statType.Equals(nameof(OtherWaveUpgrade)))
            {
                statName = EditorGUILayout.TextField("Stat Name", statName);
                GUILayout.FlexibleSpace();
                if (!GUILayout.Button("Create Modifier")) return;
                var waveUpgrade = new OtherWaveUpgrade(statName);
                waveUpgradeData.AddWaveUpgrade(waveUpgrade);
                EditorUtility.SetDirty(waveUpgradeData);
                Close();
            }
            else if (statType.Equals(nameof(EventWaveUpgrade)))
            {
                statName = EditorGUILayout.TextField("Stat Name", statName);
                eventIndex = EditorGUILayout.Popup("Event", eventIndex,
                    waveUpgradeEventTypes.Select(t => t.Name).ToArray());
                GUILayout.FlexibleSpace();
                if (!GUILayout.Button("Create Modifier")) return;
                var eventType = waveUpgradeEventTypes[eventIndex];
                var waveUpgrade = new EventWaveUpgrade(statName, eventType.Name);
                waveUpgradeData.AddWaveUpgrade(waveUpgrade);
                EditorUtility.SetDirty(waveUpgradeData);
                Close();
            }
        }

        private string DrawDropdown()
        {
            upgradeTypeIndex = EditorGUILayout.Popup("Stat Type", upgradeTypeIndex, waveUpgradeTypesNames.ToArray());
            return waveUpgradeTypesNames[upgradeTypeIndex];
        }
    }
}