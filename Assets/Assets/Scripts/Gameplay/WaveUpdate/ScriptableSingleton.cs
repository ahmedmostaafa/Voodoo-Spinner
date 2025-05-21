using System.IO;
using UnityEngine;

namespace KabreetGames.BladeSpinner
{
    public abstract class ScriptableSingleton<T> : ScriptableObject where T : ScriptableSingleton<T>
    {
        private static T instance;
        private static string folderPath = "Assets/Resources/Singletons"; // Folder to save the asset
        private static string assetPath => $"{folderPath}/{typeof(T).Name}.asset";

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Resources.Load<T>($"Singletons/{typeof(T).Name}");

                    if (instance == null)
                    {
                        instance = CreateInstance<T>();
                        SaveInstance();
                    }
                }
                return instance;
            }
        }

        public static void SaveInstance()
        {
#if UNITY_EDITOR
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            UnityEditor.AssetDatabase.CreateAsset(instance, assetPath);
            UnityEditor.AssetDatabase.SaveAssets();
#endif
        }
    }
}