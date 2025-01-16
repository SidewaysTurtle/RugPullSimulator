using UnityEngine;
using UnityEditor;

namespace SerializedJSONSystem{
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
        internal static void SaveToJSON(T obj, string path) {
            Debug.Log(obj.ToString());
            System.IO.File.WriteAllText(path, JsonUtility.ToJson(obj, true));
        }

        // public
        public static void LoadScriptableObject(string filename, out T _instance)
        {
            string jsonPath = System.IO.Path.Combine(Application.persistentDataPath,UnityEngine.SceneManagement.SceneManager.GetActiveScene().name,$"{filename}.json");
            
            //FIXME: Using split("something") all over the place will blow up in my face
            if(filename.Contains("."))
                filename = filename.Split(".")[0];
            string resourcesPath = "Computer/Icon/" + filename;

            T instance = ScriptableObject.CreateInstance<T>();
            if (System.IO.File.Exists(jsonPath))
                {
                    Debug.Log("Loading from " + jsonPath);
                    LoadFromJSON(jsonPath, out _instance);
                }
            else
            {
                try
                {
                    Debug.LogError($"Could not load {typeof(T)} from ({jsonPath})\nLoading most recent from {resourcesPath} instead.");
                    LoadFromResources(resourcesPath, out _instance);
                }
                catch(System.Exception e)
                {
                    Debug.LogAssertion($"No {typeof(T)} file found.\n{e.Message}");
                    _instance = null;
                }
            }
        }
        
        public static void SaveScriptableObject(T scriptableObject, string filename) {
            string jsonPath = System.IO.Path.Combine(Application.persistentDataPath,UnityEngine.SceneManagement.SceneManager.GetActiveScene().name,$"{filename}.json");
            
            Debug.Log($"Saving at {jsonPath}");
            SaveToJSON(scriptableObject, jsonPath);

            // Bad solution.

            // if(filename.Contains("."))
            //     filename = filename.Split(".")[0];
            // string resourcesPath = "Computer/Icon/" + filename;
            // AssetDatabase.CreateAsset ((T)obj, resourcesPath);

            // EditorUtility.SetDirty(scriptableObject);
            // AssetDatabase.SaveAssets();
            // AssetDatabase.Refresh();
        }
    }
}