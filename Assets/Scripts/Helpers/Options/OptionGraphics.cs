using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionGraphics : OptionHorizontalDropdown
{
    public override void OnValueChanged(int value)
    {
        base.OnValueChanged(value);
        QualitySettings.SetQualityLevel(value);
    }
}