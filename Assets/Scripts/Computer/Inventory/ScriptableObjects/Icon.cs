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
[CreateAssetMenu(menuName = "Icons/DefaultIcon", fileName = "IconName.asset")]
[System.Serializable]
public class Icon : ScriptableObject
{
    public Sprite image;

    public void Awake()
    {
        image = Resources.Load<Sprite>("Art/UI/file.png");
    }
}