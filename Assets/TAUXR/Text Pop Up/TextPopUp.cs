using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextPopUp : MonoBehaviour
{
    private const float ScaleMultiplierScalar = 0.5f;
    [SerializeField] private GameObject _backFace;
    [SerializeField] private GameObject _pointer;
    [SerializeField] private TextMeshPro _textUI;

    [SerializeField] private int _numberOfLettersWhenScaleIsOne = 55;

    [TextArea(1, 10)] [SerializeField] private string _text;

    public void GetTextFromComponent()
    {
        _text = _textUI.text;
    }

    public void SetText(string newText)
    {
        _textUI.text = newText;
    }

    public void SetTextAndScale(string newText)
    {
        _textUI.text = newText;
        SetNewScale();
    }

    public void SetTextAndScaleFromSerializedField()
    {
        _textUI.text = _text;
        SetNewScale();
    }


    public void SetNewScale()
    {
        float scaleMultiplier = (float)_textUI.text.Length / _numberOfLettersWhenScaleIsOne;
        scaleMultiplier = Mathf.Lerp(1, scaleMultiplier, ScaleMultiplierScalar);
        if (_textUI.text.Length > 300)
        {
            scaleMultiplier /= 1.5f;
        }

        Debug.Log("New text background scale multiplier is: " + scaleMultiplier);
        _backFace.transform.localScale = new Vector3(scaleMultiplier, scaleMultiplier, 1);
        _textUI.rectTransform.sizeDelta = new Vector2(0.48f * scaleMultiplier,
            0.08f * scaleMultiplier);
        _pointer.transform.localPosition = new Vector3(-0.0034f, 0,
            _textUI.rectTransform.offsetMax.x + 0.02f * scaleMultiplier);
        _pointer.transform.localScale = new Vector3(scaleMultiplier, scaleMultiplier, 1);
    }

#if UNITY_EDITOR
    public void ResetScale()
    {
        _textUI.rectTransform.sizeDelta = new Vector2(0.48f, 0.08f);
        _pointer.transform.localPosition = new Vector3(-0.0034f, 0, 0.26f);
        _pointer.transform.localScale = Vector3.one;
        _backFace.transform.localScale = Vector3.one;
    }

    public void DebugNumberOfWords()
    {
        Debug.Log(_textUI.text.Length);
    }
#endif
}