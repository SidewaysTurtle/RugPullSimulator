using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.Events;

using SerializedJSONSystem;

/// <summary>
/// A variant of the Icon ScriptableObject that is used to represent a file. It's the base.
/// </summary>
//requireComponent:
[System.Serializable]
public abstract class Icon : ScriptableObject
{
    [Header("Visual")]
    public Sprite image;

    [Tooltip("Event triggered when the icon is clicked after being focused/selected")]
    public UnityEvent OnFocusedClick;

    [Tooltip("Event triggered when the icon is left-clicked")]
    public UnityEvent OnLeftClick;
    
    [Tooltip("Event triggered when the icon is right-clicked")]
    public UnityEvent OnRightClick;
    
    [Tooltip("Event triggered when the icon is middle-clicked")]
    public UnityEvent OnMiddleClick;
    
    [Tooltip("Event triggered when the icon gains focus/selection")]
    public UnityEvent OnFocus;
    
    [Tooltip("Event triggered when the icon loses focus/selection")]
    public UnityEvent OnUnfocus;
    
    /// <summary>
    /// Invokes the appropriate click event based on the interaction type.
    /// </summary>
    /// <param name="clickType">The type of click that occurred</param>
    /// <param name="isFocused">Whether the icon was already focused when clicked</param>
    public virtual void InvokeClickEvent(ClickType clickType, bool isFocused = false)
    {
        switch (clickType)
        {
            case ClickType.Left:
                if (isFocused)
                    OnFocusedClick?.Invoke();
                else
                    OnLeftClick?.Invoke();
                break;
            case ClickType.Right:
                OnRightClick?.Invoke();
                break;
            case ClickType.Middle:
                OnMiddleClick?.Invoke();
                break;
        }
    }
    
    /// <summary>
    /// Invokes the focus event when the icon gains selection.
    /// </summary>
    public virtual void InvokeFocusEvent()
    {
        OnFocus?.Invoke();
    }
    
    /// <summary>
    /// Invokes the unfocus event when the icon loses selection.
    /// </summary>
    public virtual void InvokeUnfocusEvent()
    {
        OnUnfocus?.Invoke();
    }
}

/// <summary>
/// Enum representing different types of clicks that can occur on an icon.
/// </summary>
public enum ClickType
{
    Left,
    Right,
    Middle
}