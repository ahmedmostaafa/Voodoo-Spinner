using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace KabreetGames.BladeSpinner.Editor
{
    public class CreateStatWindow : EditorWindow
    {
        private readonly List<string> statTypes = new(new[] { "Float", "Int" });
        private WaveUpgradeData waveUpgradeData;

        private int statTypeIndex;
        private float startValue;
        private string statName;

        public static void ShowWindow(Rect buttonRect)
        {
            var window = CreateInstance<CreateStatWindow>();
            var windowSize = new Vector2(250, 180);
            window.ShowAsDropDown(new Rect(buttonRect.x + 50, buttonRect.y, 0, 0), windowSize);
        }

        private void OnEnable()
        {
            waveUpgradeData = WaveUpgradeData.Instance;
            statTypeIndex = 0;
            startValue = 0;
            statName = "";
        }

        private void OnGUI()
        {
            var statType = DrawDropdown();
            statName = EditorGUILayout.TextField("Stat Name", statName);

            if (statType == "Float")
            {
                startValue = EditorGUILayout.FloatField("Start Value", startValue);
            }
            else
            {
                startValue = EditorGUILayout.IntField("Start Value", (int)startValue);
            }


            GUI.enabled = !string.IsNullOrEmpty(statName);
            if (!GUILayout.Button("Create Stat")) return;
            if (statType == "Float")
            {
                waveUpgradeData.AddFloatStat(statName, startValue);
            }
            else
            {
                waveUpgradeData.AddIntStat(statName, (int)startValue);
            }

            Close();
        }

        private string DrawDropdown()
        {
            statTypeIndex = EditorGUILayout.Popup("Stat Type", statTypeIndex, statTypes.ToArray());
            return statTypes[statTypeIndex];
        }
    }
}