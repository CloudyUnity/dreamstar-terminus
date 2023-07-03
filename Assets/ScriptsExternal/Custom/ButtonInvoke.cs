using UnityEngine;

public class ButtonInvoke : PropertyAttribute
{
    public enum DisplayIn
    {
        PlayMode,
        EditMode,
        PlayAndEditModes
    }

    public readonly string customLabel;
    public readonly string methodName;
    public readonly object methodParameter;
    public readonly DisplayIn displayIn;

    public ButtonInvoke(string methodName, object methodParameter = null, DisplayIn displayIn = DisplayIn.PlayAndEditModes, string customLabel = "")
    {
        this.methodName = methodName;
        this.methodParameter = methodParameter;
        this.displayIn = displayIn;
        this.customLabel = customLabel;
    }
}
