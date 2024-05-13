using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class SelectionQuestionInterface : MonoBehaviour
{
    public Action<SelectionAnswerData> AnswerSubmitted;

    [SerializeField] private SelectionQuestionInterfaceReferences _selectionQuestionInterfaceReferences;

    private SelectionAnswer _selectedAnswer;

    private bool _correctAnswerSubmitted = false;

    private SelectionQuestionData _selectionQuestionData;

    private void OnEnable()
    {
        _selectionQuestionInterfaceReferences.SubmitButton.AnswerSubmitted += OnAnswerSubmitted;
        foreach (SelectionAnswer selectionAnswer in _selectionQuestionInterfaceReferences.SelectionAnswers)
        {
            selectionAnswer.AnswerSelected += () => _selectedAnswer = selectionAnswer;
        }
    }

    private void OnDisable()
    {
        _selectionQuestionInterfaceReferences.SubmitButton.AnswerSubmitted -= OnAnswerSubmitted;
        foreach (SelectionAnswer selectionAnswer in _selectionQuestionInterfaceReferences.SelectionAnswers)
        {
            selectionAnswer.AnswerSelected -= () => _selectedAnswer = selectionAnswer;
        }
    }

    public async UniTask ShowQuestionAndWaitForFinalSubmission(SelectionQuestionData selectionQuestionData)
    {
        _selectionQuestionData = selectionQuestionData;
        _selectionQuestionInterfaceReferences.QuestionTextDisplay.SetText(_selectionQuestionData.Text);
        InitializeAnswerButtons();

        await UniTask.WaitUntil(() => _correctAnswerSubmitted);
    }

    private void InitializeAnswerButtons()
    {
        int numberOfAnswersInInterface = _selectionQuestionInterfaceReferences.SelectionAnswers.Length;
        for (int answerIndex = 0; answerIndex < numberOfAnswersInInterface; answerIndex++)
        {
            SelectionAnswer selectionAnswer = _selectionQuestionInterfaceReferences.SelectionAnswers[answerIndex];
            selectionAnswer.gameObject.SetActive(answerIndex < _selectionQuestionData.Answers.Length);
            selectionAnswer.Init(_selectionQuestionData.Answers[answerIndex]);
        }
    }

    private void OnAnswerSubmitted()
    {
        AnswerSubmitted?.Invoke(_selectedAnswer.SelectionAnswerData);
        _correctAnswerSubmitted = _selectedAnswer.SelectionAnswerData.IsCorrect;
        _selectedAnswer.OnAnswerSubmitted().Forget();
        _selectionQuestionData.NumberOfTries++;

        //If answer was wrong and we reached the max number of tries or there is only one more possible answer,
        //Select and submit the correct answer.
        if (_selectionQuestionData.NumberOfTries == _selectionQuestionData.Answers.Length)
        {
            _selectedAnswer = GetCorrectAnswer();
                OnAnswerSubmitted();
            return;
        }

        ShowAnswerInfo();
    }

    private void ShowAnswerInfo()
    {
        _selectionQuestionInterfaceReferences.AnswerInfo.Show();
        _selectionQuestionInterfaceReferences.AnswerInfo.SetText(_selectedAnswer.SelectionAnswerData.AnswerInfo);
    }

    public void Show()
    {
    }

    public void Hide()
    {
        _selectionQuestionInterfaceReferences.AnswerInfo.Hide();
    }

    private SelectionAnswer GetCorrectAnswer()
    {
        return _selectionQuestionInterfaceReferences.SelectionAnswers.ToList()
            .Find((selectionAnswer) => selectionAnswer.SelectionAnswerData.IsCorrect);
    }
}