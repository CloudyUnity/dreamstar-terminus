using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ButtonInvoke))]
public class ButtonInvokeDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        ButtonInvoke settings = (ButtonInvoke)attribute;
        return DisplayButton(ref settings) ? EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing : 0;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ButtonInvoke settings = (ButtonInvoke)attribute;

        if (!DisplayButton(ref settings))
            return;

        string buttonLabel = string.IsNullOrEmpty(settings.customLabel) ? label.text : settings.customLabel;

        if (!(property.serializedObject.targetObject is MonoBehaviour mb))
            return;

        if (GUI.Button(position, buttonLabel))
        {
            mb.SendMessage(settings.methodName, settings.methodParameter);
        }
    }

    private bool DisplayButton(ref ButtonInvoke settings)
    {
        return (settings.displayIn == ButtonInvoke.DisplayIn.PlayAndEditModes) ||
               (settings.displayIn == ButtonInvoke.DisplayIn.EditMode && !Application.isPlaying) ||
               (settings.displayIn == ButtonInvoke.DisplayIn.PlayMode && Application.isPlaying);
    }
}