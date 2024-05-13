using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SelectionQuestionInterfaceReferences
{
    public TextDisplay QuestionTextDisplay => _questionTextDisplay;
    public TextDisplay AnswerInfo => _answerInfo;
    public SelectionQuestionSubmitButton SubmitButton => _submitButton;
    public SelectionAnswer[] SelectionAnswers => _selectionAnswers;


    [SerializeField] private TextDisplay _questionTextDisplay;
    [SerializeField] private TextDisplay _answerInfo;
    [SerializeField] private SelectionQuestionSubmitButton _submitButton;
    [SerializeField] private SelectionAnswer[] _selectionAnswers;
}