using UnityEngine;
using System;
using System.Collections;

public enum ConditionalDisplayMode
{
    Disable = 0,    // Show property but disable it (grayed out)
    Hide = 1        // Completely hide property from inspector
}
 
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property |
    AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
public class ConditionalHideAttribute : PropertyAttribute
{
    //The name of the field that will be in control
    public string ConditionalSourceField = "";
    //How to display the property when condition is not met
    public ConditionalDisplayMode DisplayMode = ConditionalDisplayMode.Disable;
    //The value to compare against (for enums, ints, etc.)
    public object ConditionalValue = null;
    //Whether to use boolean comparison (legacy) or value comparison
    public bool UseBoolComparison = true;
    
    // Legacy property for backwards compatibility
    public bool HideInInspector 
    { 
        get { return DisplayMode == ConditionalDisplayMode.Hide; }
        set { DisplayMode = value ? ConditionalDisplayMode.Hide : ConditionalDisplayMode.Disable; }
    }

    // Constructor for boolean fields - disable by default
    public ConditionalHideAttribute(string conditionalSourceField)
    {
        this.ConditionalSourceField = conditionalSourceField;
        this.DisplayMode = ConditionalDisplayMode.Disable;
        this.UseBoolComparison = true;
    }
 
    // Constructor for boolean fields with display mode option
    public ConditionalHideAttribute(string conditionalSourceField, ConditionalDisplayMode displayMode)
    {
        this.ConditionalSourceField = conditionalSourceField;
        this.DisplayMode = displayMode;
        this.UseBoolComparison = true;
    }

    // Constructor for boolean fields with hide option (legacy support)
    public ConditionalHideAttribute(string conditionalSourceField, bool hideInInspector)
    {
        this.ConditionalSourceField = conditionalSourceField;
        this.DisplayMode = hideInInspector ? ConditionalDisplayMode.Hide : ConditionalDisplayMode.Disable;
        this.UseBoolComparison = true;
    }
    
    // Constructor for enum/int fields - disable by default
    public ConditionalHideAttribute(string conditionalSourceField, int conditionalValue)
    {
        this.ConditionalSourceField = conditionalSourceField;
        this.ConditionalValue = conditionalValue;
        this.DisplayMode = ConditionalDisplayMode.Disable;
        this.UseBoolComparison = false;
    }
    
    // Constructor for enum/int fields with display mode option
    public ConditionalHideAttribute(string conditionalSourceField, int conditionalValue, ConditionalDisplayMode displayMode)
    {
        this.ConditionalSourceField = conditionalSourceField;
        this.ConditionalValue = conditionalValue;
        this.DisplayMode = displayMode;
        this.UseBoolComparison = false;
    }

    // Constructor for enum/int fields with hide option (legacy support)
    public ConditionalHideAttribute(string conditionalSourceField, int conditionalValue, bool hideInInspector)
    {
        this.ConditionalSourceField = conditionalSourceField;
        this.ConditionalValue = conditionalValue;
        this.DisplayMode = hideInInspector ? ConditionalDisplayMode.Hide : ConditionalDisplayMode.Disable;
        this.UseBoolComparison = false;
    }
}

/// <summary>
/// Combines ConditionalHide functionality with Header attribute
/// This allows you to conditionally show/hide headers along with their spacing
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true)]
public class ConditionalHiddenHeaderAttribute : PropertyAttribute
{
    //The name of the field that will be in control
    public string ConditionalSourceField = "";
    //How to display the property when condition is not met
    public ConditionalDisplayMode DisplayMode = ConditionalDisplayMode.Hide;
    //The value to compare against (for enums, ints, etc.)
    public object ConditionalValue = null;
    //Whether to use boolean comparison (legacy) or value comparison
    public bool UseBoolComparison = true;
    //Header text to display
    public string HeaderText = "";
    
    // Constructor for boolean fields with header text
    public ConditionalHiddenHeaderAttribute(string headerText, string conditionalSourceField)
    {
        this.HeaderText = headerText;
        this.ConditionalSourceField = conditionalSourceField;
        this.DisplayMode = ConditionalDisplayMode.Hide;
        this.UseBoolComparison = true;
    }
    
    // Constructor for boolean fields with header text and display mode
    public ConditionalHiddenHeaderAttribute(string headerText, string conditionalSourceField, ConditionalDisplayMode displayMode)
    {
        this.HeaderText = headerText;
        this.ConditionalSourceField = conditionalSourceField;
        this.DisplayMode = displayMode;
        this.UseBoolComparison = true;
    }
    
    // Constructor for enum/int fields with header text
    public ConditionalHiddenHeaderAttribute(string headerText, string conditionalSourceField, int conditionalValue)
    {
        this.HeaderText = headerText;
        this.ConditionalSourceField = conditionalSourceField;
        this.ConditionalValue = conditionalValue;
        this.DisplayMode = ConditionalDisplayMode.Hide;
        this.UseBoolComparison = false;
    }
    
    // Constructor for enum/int fields with header text and display mode
    public ConditionalHiddenHeaderAttribute(string headerText, string conditionalSourceField, int conditionalValue, ConditionalDisplayMode displayMode)
    {
        this.HeaderText = headerText;
        this.ConditionalSourceField = conditionalSourceField;
        this.ConditionalValue = conditionalValue;
        this.DisplayMode = displayMode;
        this.UseBoolComparison = false;
    }
}