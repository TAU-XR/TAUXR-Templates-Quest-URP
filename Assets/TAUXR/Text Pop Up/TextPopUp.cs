using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using TMPro;

public class TextPopUp : MonoBehaviour
{
    private float _defaultTextWidth;
    private float _defaultTextHeight;
    private const int NumberOfLettersWhenScaleIsOne = 55;

    [SerializeField] private bool _useAnimation = true;

    [SerializeField] private Transform _background;
    [SerializeField] private TextMeshPro _textUI;
    private RectTransform _textRectTransform;

    [TextArea(1, 10)] [SerializeField] private string _text;

    private void Awake()
    {
        _textRectTransform = _textUI.gameObject.GetComponent<RectTransform>();
    }

    private void Start()
    {
        _defaultTextWidth = _textRectTransform.rect.width;
        _defaultTextHeight = _textRectTransform.rect.height;
    }

    private void OnEnable()
    {
        if (!_useAnimation)
        {
            GetComponent<Animator>().Play("Birth", -1, 1);
        }
    }

    public void SetText(string newText)
    {
        _textUI.text = newText;
    }

    public void SetTextAndScale(string newText)
    {
        _textUI.text = newText;
    }

    private void UpdateTextUI(float scaleMultiplier)
    {
        _textUI.rectTransform.sizeDelta = new Vector2(_defaultTextWidth * scaleMultiplier,
            _defaultTextHeight * scaleMultiplier);
    }


#if UNITY_EDITOR
    [Button]
    public void GetTextFromComponent()
    {
        _text = _textUI.text;
    }

    [Button]
    public void SetTextAndScaleFromSerializedField()
    {
        _textUI.text = _text;
    }

    [Button]
    public void DebugNumberOfLetters()
    {
        Debug.Log(_textUI.text.Length);
    }

    [Button]
    public void ResetScale()
    {
        _textUI.rectTransform.sizeDelta = new Vector2(0.48f, 0.08f);
        _background.transform.localScale = Vector3.one;
    }

#endif
}