using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextPopUp : MonoBehaviour
{
    private const float ScaleMultiplierScalar = 0.5f;
    [SerializeField] private GameObject _backFace;
    [SerializeField] private GameObject _pointer;
    [SerializeField] private TextMeshPro _text;

    [SerializeField] private int _numberOfWordsWhenScaleIsOne = 41;

    public void SetNewScale()
    {
        float scaleMultiplier = (float)_text.text.Length / _numberOfWordsWhenScaleIsOne;
        scaleMultiplier = Mathf.Lerp(1, scaleMultiplier, ScaleMultiplierScalar);

        Debug.Log(scaleMultiplier);
        _backFace.transform.localScale = new Vector3(scaleMultiplier, scaleMultiplier, 1);
        _text.rectTransform.sizeDelta = new Vector2(50 * scaleMultiplier,
            7 * scaleMultiplier);
        //TODO: set pointer position
        _pointer.transform.localPosition = new Vector3(-0.0034f, 0, 0.31f * scaleMultiplier);
        _pointer.transform.localScale = new Vector3(scaleMultiplier, scaleMultiplier, 1);
    }

#if UNITY_EDITOR
    public void ResetScale()
    {
        _text.rectTransform.sizeDelta = new Vector2(50, 7);
        _pointer.transform.localPosition = new Vector3(-0.0034f, 0, 0.31f);
        _pointer.transform.localScale = Vector3.one;
        _backFace.transform.localScale = Vector3.one;
    }

    public void DebugNumberOfWords()
    {
        Debug.Log(_text.text.Length);
    }
#endif
}