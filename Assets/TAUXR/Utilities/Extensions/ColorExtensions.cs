using System.Collections;
using System.Collections.Generic;
using Shapes;
using TMPro;
using UnityEngine;

public static class ColorExtensions
{
    public static void SetAlpha(this TextMeshPro text, float alpha)
    {
        text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
    }

    public static void SetAlpha(this Rectangle rectangle, float alpha)
    {
        rectangle.Color = new Color(rectangle.Color.r, rectangle.Color.g, rectangle.Color.b, alpha);
    }
}