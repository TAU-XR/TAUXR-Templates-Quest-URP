using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SQIAnswerData
{
    [TextArea]
    public string Text;
    public bool IsCorrect;
    public string AnswerInfo;
}