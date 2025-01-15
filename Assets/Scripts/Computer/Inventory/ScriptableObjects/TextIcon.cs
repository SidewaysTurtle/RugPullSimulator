using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

using SeralizedJSONSystem;

/// <summary>
/// A variant of the Icon ScriptableObject that is used to represent a file.
/// </summary>
//requireComponent:
[CreateAssetMenu(menuName = "Icons/Text", fileName = "TextName.asset")]
[System.Serializable]
public class TextIcon : ScriptableObject {
    public enum TextType { Json, Lua, txt }
    public TextType textType;
    public string FileData;

    public Sprite image;

    public void Awake()
    {
        image = Resources.Load<Sprite>("Art/UI/file.png");
    }

    // public string FileData {
    //     get {
    //         // cant return fileData directly, because 
    //         // when we save the file, it will be overwritten.
    //         // so we need to send the FILEs data
    //         return FileSystem.File.ReadFile(name, textType.ToString());
    //     }
    //     set {
    //         FileSystem.File.WriteFile(name, value);
    //     }
    // }

    
}