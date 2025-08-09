using UnityEngine;
using DG.Tweening;
using TMPro;
using Shapes;

/*
 * Hard Coded Child Convention - Parent- state empty GO. Child 0 - Stroke. Child 1 - Background. Child 1 Child 0 - Text.
 * 
 */
public class TXRButtonVisuals : MonoBehaviour
{
    protected TXRButtonState _state;
    protected Sequence buttonAnimation;
    [SerializeField] protected float _transitionDuration;

    [Header("UI Elements")]
    [SerializeField] protected Rectangle _background;
    [SerializeField] protected Rectangle _stroke;
    [SerializeField] protected TextMeshPro _text;

    [Header("UI States")]
    [SerializeField] protected Transform _activeState;
    [SerializeField] protected Transform _disableState;
    [SerializeField] protected Transform _pressState;
    [SerializeField] protected Transform _hiddenState;
    [SerializeField] protected Transform _hoverState;

    public virtual void SetState(TXRButtonState state)
    {
        switch (state)
        {
            case TXRButtonState.Active:
                SetStateAnimation(_activeState);
                break;
            case TXRButtonState.Pressed:
                SetStateAnimation(_pressState);
                break;
            case TXRButtonState.Hidden:
                SetStateAnimation(_hiddenState);
                break;
            case TXRButtonState.Disabled:
                SetStateAnimation(_disableState);
                break;
            case TXRButtonState.Hover:
                SetStateAnimation(_hoverState);
                break;
        }
        _state = state;
    }

    protected void SetStateAnimation(Transform stateTransform)
    {
        Rectangle targetBackground = GetBackground(stateTransform);
        Rectangle targetStroke = GetStroke(stateTransform);
        TextMeshPro targetText = GetText(stateTransform);

        buttonAnimation.Kill();
        buttonAnimation.Append(ComponentAnimator.RectangleTween(_background, targetBackground, _transitionDuration));
        buttonAnimation.Join(ComponentAnimator.TransformTween(_background.transform, targetBackground.transform, _transitionDuration, true));
        buttonAnimation.Join(ComponentAnimator.RectangleTween(_stroke, targetStroke, _transitionDuration));
        buttonAnimation.Join(ComponentAnimator.TransformTween(_stroke.transform, targetStroke.transform, _transitionDuration, true));
        buttonAnimation.Join(ComponentAnimator.TextMeshProTween(_text, targetText, _transitionDuration));
    }

    public virtual void SetAllStatesSizeFromMainUI()
    {
        Transform stateTransform = _activeState;
        for (int i = 0; i <= 4; i++)
        {
            switch (i)
            {
                case 0: stateTransform = _activeState; break;
                case 1: stateTransform = _disableState; break;
                case 2: stateTransform = _hoverState; break;
                case 3: stateTransform = _pressState; break;
                case 4: stateTransform = _hiddenState; break;
                default: stateTransform = _activeState; break;
            }
            SetStatesSizesFromMainUI(stateTransform);
        }
    }
    public virtual void SetStatesSizesFromMainUI(Transform stateTransform)
    {
        Rectangle targetBackground = GetBackground(stateTransform);
        Rectangle targetStroke = GetStroke(stateTransform);
        TextMeshPro targetText = GetText(stateTransform);

        targetBackground.Width = _background.Width;
        targetBackground.Height = _background.Height;

        targetStroke.Width = _stroke.Width;
        targetStroke.Height = _stroke.Height;

        targetText.rectTransform.sizeDelta = _text.rectTransform.sizeDelta;
        targetText.fontSize = _text.fontSize;
    }

    private Rectangle GetBackground(Transform stateTransform)
    {
        return stateTransform.GetChild(1).GetComponent<Rectangle>();
    }

    private Rectangle GetStroke(Transform stateTransform)
    {
        return stateTransform.GetChild(0).GetComponent<Rectangle>();
    }

    private TextMeshPro GetText(Transform stateTransform)
    {
        return stateTransform.GetChild(1).GetChild(0).GetComponent<TextMeshPro>();
    }

    public void SetBackfaceColor(TXRButtonState state, Color color, float duration = 0.25f)
    {
        Transform stateTransform = null;
        switch (state)
        {
            case TXRButtonState.Active:
                stateTransform = _activeState;
                break;
            case TXRButtonState.Pressed:
                stateTransform = _pressState;
                break;
            case TXRButtonState.Disabled:
                stateTransform = _disableState;
                break;
        }
        if (stateTransform == null) return;

        Rectangle targetBackground = GetBackground(stateTransform);
        targetBackground.FillColorEnd = color;

        // update color change if changed the color of current state
        if (_state == state)
        {
            SetState(state);
        }
    }

    public Color GetColor(TXRButtonState state)
    {
        Transform stateTransform = null;
        switch (state)
        {
            case TXRButtonState.Active:
                stateTransform = _activeState;
                break;
            case TXRButtonState.Pressed:
                stateTransform = _pressState;
                break;
            case TXRButtonState.Disabled:
                stateTransform = _disableState;
                break;
        }
        if (stateTransform == null)
        {

            Debug.LogError("No color defined for state: " + state + ", Returning solid black");
            return Color.black;
        }

        Rectangle targetBackground = GetBackground(stateTransform);
        return targetBackground.FillColorEnd;
    }
}