using UnityEngine;

[CreateAssetMenu(fileName = "ButtonVisualsConfigurations", menuName = "ScriptableObjects/ButtonVisualsConfigurations", order = 1)]
public class ButtonVisualsConfigurations : ScriptableObject
{
    [Header("Active Animation")]
    [Tooltip("Color of the backface when active")]
    public Color backfaceColorActive;
    [Tooltip("Gradient of the backface when active")]
    public float backfaceGradientActive;
    [Tooltip("Thickness of the stroke when active")]
    public float strokeThicknessActive;
    [Tooltip("Z position of the backfade when active")]
    public float backfadeZPositionActive;
    [Tooltip("Duration of the active state animation")]
    public float activeDuration;

    [Header("Press Animation")]
    [Tooltip("Duration of the press animation")]
    public float pressDuration;
    [Tooltip("Color of the backface on press")]
    public Color backfaceColorPress;
    [Tooltip("Thickness of the stroke on press")]
    public float strokeThicknessPress;
    [Tooltip("Z position of the backfade on press")]
    public float backfadeZPositionPress;

    [Header("Disabled")]
    [Tooltip("Color of the backface when disabled")]
    public Color backfaceColorDisabled;

    [Header("Hide Animation")]
    [Tooltip("Color of the backface when hidden")]
    public Color backfaceColorHide;
    [Tooltip("Duration of the hide animation")]
    public float hideDuration;

    [Header("Hover Animation")]
    [Tooltip("Duration of the hover animation")]
    public float hoverDuration;
    [Tooltip("Color gradient of the backface on hover")]
    public Color backfaceColorGradientHover;
    [Tooltip("Gradient of the backface on hover")]
    public float backfaceGradientHover;
    [Tooltip("Thickness of the stroke on hover")]
    public float strokeThicknessHover;
    [Tooltip("Z position of the backfade on hover")]
    public float backfadeZPositionHover;
}
