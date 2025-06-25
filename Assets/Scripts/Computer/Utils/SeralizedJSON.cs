using UnityEngine;
using UnityEditor;
using System.IO;

namespace SerializedJSONSystem
{
    public static class SerializedJSON<T> where T : ScriptableObject
    {
        // adding JSON Serialization (copy + paste from texticon.cs)
        internal static void LoadFromJSON(string path, out T instance){
            instance = ScriptableObject.CreateInstance<T>();
            JsonUtility.FromJsonOverwrite(System.IO.File.ReadAllText(path), instance);
            instance.hideFlags = HideFlags.HideAndDontSave;
        }

        internal static void LoadFromResources(string filename, out T instance){
            Debug.Log($"Loading from Resources at\n{filename}");
            // theres a really stupid bug where if the texticon is 
            // of a different type than the default
            // it will be null
            instance = (T)Resources.Load(filename);
            instance.hideFlags = HideFlags.HideAndDontSave;
        }

        private static string GetSavePath(string filename)
        {
            string scenePath = Path.Combine(Application.persistentDataPath, 
                                          UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
            
            // Create directories if they don't exist
            if (!Directory.Exists(scenePath))
            {
                Directory.CreateDirectory(scenePath);
            }

            return Path.Combine(scenePath, $"{filename}.json");
        }

        internal static void SaveToJSON(T obj, string path)
        {
            Debug.Log($"Saving to: {path}");
            string jsonData = JsonUtility.ToJson(obj, true);
            File.WriteAllText(path, jsonData);
        }

        // public
        public static void LoadScriptableObject(string filename, out T _instance)
        {
            string jsonPath = GetSavePath(filename);
            string resourcesPath = "Computer/Inventory/" + filename;
            
            if (File.Exists(jsonPath))
            {
                Debug.Log($"Loading from: {jsonPath}");
                LoadFromJSON(jsonPath, out _instance);
            }
            else
            {
                Debug.LogWarning($"No save file found at: {jsonPath}. Attempting to load from Resources.");
                LoadFromResources(resourcesPath, out _instance);
            }
        }

        public static void SaveScriptableObject(string filename, T scriptableObject)
        {
            string jsonPath = GetSavePath(filename);
            SaveToJSON(scriptableObject, jsonPath);
        }

        public static void DeleteScriptableObject(string filename)
        {
            string jsonPath = GetSavePath(filename);
            if (File.Exists(jsonPath))
            {
                File.Delete(jsonPath);
                Debug.Log($"Deleted save file at: {jsonPath}");
            }
            else
            {
                Debug.Log($"No save file found to delete at: {jsonPath}");
            }
        }
    }
}