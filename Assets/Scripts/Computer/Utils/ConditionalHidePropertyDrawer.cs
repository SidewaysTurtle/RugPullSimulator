using UnityEngine;
using UnityEditor;
 
[CustomPropertyDrawer(typeof(ConditionalHideAttribute))]
public class ConditionalHidePropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ConditionalHideAttribute condHAtt = (ConditionalHideAttribute)attribute;
        bool conditionMet = GetConditionalHideAttributeResult(condHAtt, property);
 
        // Determine how to display the property based on the condition and display mode
        if (conditionMet)
        {
            // Condition is met - show property normally
            EditorGUI.PropertyField(position, property, label, true);
        }
        else
        {
            // Condition is not met - apply display mode
            switch (condHAtt.DisplayMode)
            {
                case ConditionalDisplayMode.Hide:
                    // Don't draw anything - property is completely hidden
                    break;
                    
                case ConditionalDisplayMode.Disable:
                    // Draw property but disabled (grayed out)
                    bool wasEnabled = GUI.enabled;
                    GUI.enabled = false;
                    EditorGUI.PropertyField(position, property, label, true);
                    GUI.enabled = wasEnabled;
                    break;
            }
        }
    }
 
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        ConditionalHideAttribute condHAtt = (ConditionalHideAttribute)attribute;
        bool conditionMet = GetConditionalHideAttributeResult(condHAtt, property);
 
        // If condition is met OR we're in disable mode (not hide mode), show the property
        if (conditionMet || condHAtt.DisplayMode == ConditionalDisplayMode.Disable)
        {
            return EditorGUI.GetPropertyHeight(property, label);
        }
        else
        {
            // Hide mode and condition not met - return negative spacing to hide completely
            return -EditorGUIUtility.standardVerticalSpacing;
        }
    }
 
    private bool GetConditionalHideAttributeResult(ConditionalHideAttribute condHAtt, SerializedProperty property)
    {
        bool conditionMet = true;
        string propertyPath = property.propertyPath; //returns the property path of the property we want to apply the attribute to
        string conditionPath = propertyPath.Replace(property.name, condHAtt.ConditionalSourceField); //changes the path to the conditionalsource property path
        SerializedProperty sourcePropertyValue = property.serializedObject.FindProperty(conditionPath);
 
        if (sourcePropertyValue != null)
        {
            if (condHAtt.UseBoolComparison)
            {
                // Boolean comparison - condition is met when source property is true
                conditionMet = sourcePropertyValue.boolValue;
            }
            else
            {
                // Value comparison for enums, ints, etc. - condition is met when values match
                conditionMet = CheckPropertyValueEquals(sourcePropertyValue, condHAtt.ConditionalValue);
            }
        }
        else
        {
            Debug.LogWarning("Attempting to use a ConditionalHideAttribute but no matching SourcePropertyValue found in object: " + condHAtt.ConditionalSourceField);
        }
 
        return conditionMet;
    }
    
    private bool CheckPropertyValueEquals(SerializedProperty property, object value)
    {
        switch (property.propertyType)
        {
            case SerializedPropertyType.Boolean:
                return property.boolValue.Equals(value);
            case SerializedPropertyType.Integer:
                return property.intValue.Equals(value);
            case SerializedPropertyType.Enum:
                return property.enumValueIndex.Equals(value);
            case SerializedPropertyType.Float:
                return property.floatValue.Equals(value);
            case SerializedPropertyType.String:
                return property.stringValue.Equals(value);
            default:
                Debug.LogError("ConditionalHide: Data type not implemented: " + property.propertyType);
                return true;
        }
    }
}

[CustomPropertyDrawer(typeof(ConditionalHiddenHeaderAttribute))]
public class ConditionalHiddenHeaderPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ConditionalHiddenHeaderAttribute condHAtt = (ConditionalHiddenHeaderAttribute)attribute;
        bool conditionMet = GetConditionalHideAttributeResult(condHAtt, property);
 
        // Determine how to display the property based on the condition and display mode
        if (conditionMet)
        {
            // Condition is met - show header and property normally
            float headerHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            
            // Draw header
            Rect headerRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(headerRect, condHAtt.HeaderText, EditorStyles.boldLabel);
            
            // Draw property below header
            Rect propertyRect = new Rect(position.x, position.y + headerHeight, position.width, position.height - headerHeight);
            EditorGUI.PropertyField(propertyRect, property, label, true);
        }
        else
        {
            // Condition is not met - apply display mode
            switch (condHAtt.DisplayMode)
            {
                case ConditionalDisplayMode.Hide:
                    // Don't draw anything - both header and property are completely hidden
                    break;
                    
                case ConditionalDisplayMode.Disable:
                    // Draw both header and property but disabled (grayed out)
                    bool wasEnabled = GUI.enabled;
                    GUI.enabled = false;
                    
                    float headerHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                    
                    // Draw disabled header
                    Rect headerRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
                    EditorGUI.LabelField(headerRect, condHAtt.HeaderText, EditorStyles.boldLabel);
                    
                    // Draw disabled property below header
                    Rect propertyRect = new Rect(position.x, position.y + headerHeight, position.width, position.height - headerHeight);
                    EditorGUI.PropertyField(propertyRect, property, label, true);
                    
                    GUI.enabled = wasEnabled;
                    break;
            }
        }
    }
 
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        ConditionalHiddenHeaderAttribute condHAtt = (ConditionalHiddenHeaderAttribute)attribute;
        bool conditionMet = GetConditionalHideAttributeResult(condHAtt, property);
 
        // If condition is met OR we're in disable mode (not hide mode), show the property and header
        if (conditionMet || condHAtt.DisplayMode == ConditionalDisplayMode.Disable)
        {
            float headerHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            float propertyHeight = EditorGUI.GetPropertyHeight(property, label);
            return headerHeight + propertyHeight;
        }
        else
        {
            // Hide mode and condition not met - return negative spacing to hide completely
            return -EditorGUIUtility.standardVerticalSpacing;
        }
    }
 
    private bool GetConditionalHideAttributeResult(ConditionalHiddenHeaderAttribute condHAtt, SerializedProperty property)
    {
        bool conditionMet = true;
        string propertyPath = property.propertyPath;
        string conditionPath = propertyPath.Replace(property.name, condHAtt.ConditionalSourceField);
        SerializedProperty sourcePropertyValue = property.serializedObject.FindProperty(conditionPath);
 
        if (sourcePropertyValue != null)
        {
            if (condHAtt.UseBoolComparison)
            {
                conditionMet = sourcePropertyValue.boolValue;
            }
            else
            {
                conditionMet = CheckPropertyValueEquals(sourcePropertyValue, condHAtt.ConditionalValue);
            }
        }
        else
        {
            Debug.LogWarning("Attempting to use a ConditionalHiddenHeaderAttribute but no matching SourcePropertyValue found in object: " + condHAtt.ConditionalSourceField);
        }
 
        return conditionMet;
    }
    
    private bool CheckPropertyValueEquals(SerializedProperty property, object value)
    {
        switch (property.propertyType)
        {
            case SerializedPropertyType.Boolean:
                return property.boolValue.Equals(value);
            case SerializedPropertyType.Integer:
                return property.intValue.Equals(value);
            case SerializedPropertyType.Enum:
                return property.enumValueIndex.Equals(value);
            case SerializedPropertyType.Float:
                return property.floatValue.Equals(value);
            case SerializedPropertyType.String:
                return property.stringValue.Equals(value);
            default:
                Debug.LogError("ConditionalHiddenHeader: Data type not implemented: " + property.propertyType);
                return true;
        }
    }
}