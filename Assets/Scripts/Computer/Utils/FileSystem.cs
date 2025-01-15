using UnityEngine;
using System.IO;
using System;
using UnityEngine.SceneManagement;

namespace FileSystem
{
    public static class File
    {
        [Obsolete("Use JSON system instead [SeralizedJSONSystem]")]
        public static bool WriteFile(string filename, string fileData)
        {
            try
            {
                string path = Path.Combine(Application.persistentDataPath,SceneManager.GetActiveScene().name,$"/ingamefiles/{filename}");
                Debug.Log(path);
                // Write some text to the test.txt file
                // https://stackoverflow.com/questions/7405828/streamwriter-rewrite-the-file-or-append-to-the-file
                StreamWriter writer = new StreamWriter(path, false);
                writer.WriteLineAsync(fileData);
                writer.Close();
                return true;
            }
            catch(System.Exception e)
            {
                Debug.LogError(e.Message);
                return false;
            }
        }
        [Obsolete("Use JSON system instead [SeralizedJSONSystem]")]
        public static string ReadFile(string filename, string fileTag)
        {
            try
            {
                string path = Path.Combine(Application.persistentDataPath,SceneManager.GetActiveScene().name,$"/ingamefiles/{filename}.{fileTag}");
                //Read the text from directly from the test.txt file
                StreamReader reader = new StreamReader(path);
                return reader.ReadToEnd();
            }
            catch(System.Exception e)
            {
                Debug.LogError(e.Message);
                return $"Hi, please report this bug to the github issues tab.\n{e.Message}";
            }
        }
    }
}

