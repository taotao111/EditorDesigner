using System;
using UnityEngine;
public class NodeTooltipAttribute : TooltipAttribute
{
    public NodeTooltipAttribute(string tooltip) : base(tooltip)
    {
    }

    public Type type;
}
