using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GetColours
{
    public static Color GetColourOfElement(Element element)
    {
        switch (element)
        {
            case Element.KINETIC:
                return new Color(1, 1, 1);
            case Element.SOLAR:
                return new Color(0.93f, 0.46f, 0.05f);
            case Element.VOID:
                return new Color(0.47f, 0.2f, 0.85f);
            case Element.ARC:
                return new Color(0.2f, 0.84f, 0.84f);
            default:
                return new Color(0, 0, 0);
        }
    }
}