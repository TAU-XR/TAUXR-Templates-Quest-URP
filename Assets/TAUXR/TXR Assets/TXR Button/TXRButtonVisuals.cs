using UnityEngine;
using DG.Tweening;
using TMPro;
using Shapes;

public class TXRButtonVisuals : MonoBehaviour
{
    protected TXRButtonState _state;
    protected Shapes.Rectangle _backface;
    //protected Shapes.Rectangle _stroke;
    //protected TextMeshPro _text;
    protected ButtonVisualsConfigurations _configurations;
    protected TXRButtonReferences _references;

    protected Sequence buttonAnimation;

    [SerializeField] protected Rectangle _background;
    [SerializeField] protected Rectangle _stroke;
    [SerializeField] protected TextMeshPro _text;
    [SerializeField] protected float _transitionDuration;

    [SerializeField] protected Transform _activeState;
    [SerializeField] protected Transform _disableState;
    [SerializeField] protected Transform _pressState;
    [SerializeField] protected Transform _hiddenState;
    [SerializeField] protected Transform _hoverState;

    public virtual void Init(TXRButtonReferences references)
    {

    }

    public void SetState(TXRButtonState state)
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

    private void SetStateAnimation(Transform stateTransform)
    {
        Rectangle targetBackground = stateTransform.GetChild(1).GetComponent<Rectangle>();
        Rectangle targetStroke = stateTransform.GetChild(0).GetComponent<Rectangle>();
        TextMeshPro targetText = stateTransform.GetChild(1).GetChild(0).GetComponent<TextMeshPro>();

        buttonAnimation.Kill();
        buttonAnimation.Append(ComponentAnimator.RectangleTween(_background, targetBackground, _transitionDuration));
        buttonAnimation.Join(ComponentAnimator.TransformTween(_background.transform, targetBackground.transform, _transitionDuration, true));
        buttonAnimation.Join(ComponentAnimator.RectangleTween(_stroke, targetStroke, _transitionDuration));
        buttonAnimation.Join(ComponentAnimator.TransformTween(_stroke.transform, targetStroke.transform, _transitionDuration, true));
        buttonAnimation.Join(ComponentAnimator.TextMeshProTween(_text, targetText, _transitionDuration));
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

        Rectangle targetBackground = stateTransform.GetChild(1).GetComponent<Rectangle>();
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

        Rectangle targetBackground = stateTransform.GetChild(1).GetComponent<Rectangle>();
        return targetBackground.FillColorEnd;
    }
}