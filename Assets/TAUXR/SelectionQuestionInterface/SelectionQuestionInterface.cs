using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;

public class SelectionQuestionInterface : MonoBehaviour
{
    public Action<SelectionAnswerData> AnswerSubmitted;

    //TODO: find a way to remove this public property 
    //It is only for the editor script
    public SelectionQuestionInterfaceReferences SelectionQuestionInterfaceReferences => _selectionQuestionInterfaceReferences;
    [SerializeField] private SelectionQuestionInterfaceReferences _selectionQuestionInterfaceReferences;

    private SelectionAnswer _selectedAnswer;

    private bool _correctAnswerSubmitted = false;

    private SelectionQuestionData _selectionQuestionData;

    private void OnEnable()
    {
        _selectionQuestionInterfaceReferences.SubmitButton.AnswerSubmitted += OnAnswerSubmitted;
        foreach (SelectionAnswer selectionAnswer in _selectionQuestionInterfaceReferences.SelectionAnswers)
        {
            selectionAnswer.AnswerSelected += () => OnAnswerSelected(selectionAnswer);
            selectionAnswer.AnswerDeselected += () => OnAnswerDeselected(selectionAnswer);
        }
    }

    private void OnDisable()
    {
        _selectionQuestionInterfaceReferences.SubmitButton.AnswerSubmitted -= OnAnswerSubmitted;
        foreach (SelectionAnswer selectionAnswer in _selectionQuestionInterfaceReferences.SelectionAnswers)
        {
            selectionAnswer.AnswerSelected -= () => OnAnswerSelected(selectionAnswer);
            selectionAnswer.AnswerDeselected -= () => OnAnswerDeselected(selectionAnswer);
        }
    }

    private void OnAnswerSelected(SelectionAnswer selectionAnswer)
    {
        if (selectionAnswer == _selectedAnswer)
        {
            return;
        }

        if (_selectedAnswer != null)
        {
            _selectedAnswer.ManuallyDeselectAnswer();
        }

        _selectedAnswer = selectionAnswer;
    }

    private void OnAnswerDeselected(SelectionAnswer selectionAnswer)
    {
        if (selectionAnswer == _selectedAnswer)
        {
            _selectedAnswer = null;
        }
    }

    public async UniTask ShowQuestionAndWaitForFinalSubmission(SelectionQuestionData selectionQuestionData)
    {
        InitializeWithNewQuestion(selectionQuestionData);

        await UniTask.WaitUntil(() => _correctAnswerSubmitted);
    }

    private void InitializeWithNewQuestion(SelectionQuestionData selectionQuestionData)
    {
        _selectionQuestionInterfaceReferences.AnswerInfo.Hide();
        _selectionQuestionData = selectionQuestionData;
        _selectionQuestionInterfaceReferences.QuestionTextDisplay.SetText(_selectionQuestionData.Text);
        InitializeAnswerButtons();
    }

    private void InitializeAnswerButtons()
    {
        int numberOfAnswersInInterface = _selectionQuestionInterfaceReferences.SelectionAnswers.Length;
        for (int answerIndex = 0; answerIndex < numberOfAnswersInInterface; answerIndex++)
        {
            SelectionAnswer selectionAnswer = _selectionQuestionInterfaceReferences.SelectionAnswers[answerIndex];
            bool answerIsActive = answerIndex < _selectionQuestionData.Answers.Length;
            selectionAnswer.gameObject.SetActive(answerIsActive);
            if (answerIsActive)
            {
                selectionAnswer.Init(_selectionQuestionData.Answers[answerIndex]);
            }
        }
    }

    private void OnAnswerSubmitted()
    {
        SubmitSelectedAnswer();

        //If answer was wrong and we reached the max number of tries or there is only one more possible answer,
        //Select and submit the correct answer as well.
        bool outOfTries = _selectionQuestionData.NumberOfTries == _selectionQuestionData.Answers.Length - 1 ||
                          _selectionQuestionData.NumberOfTries == _selectionQuestionData.MaxNumberOfTries;
        if (outOfTries && !_correctAnswerSubmitted)
        {
            _selectedAnswer = GetCorrectAnswer();
            SubmitSelectedAnswer();
        }
    }

    private void SubmitSelectedAnswer()
    {
        if (_selectedAnswer == null)
        {
            return;
        }

        AnswerSubmitted?.Invoke(_selectedAnswer.SelectionAnswerData);
        _correctAnswerSubmitted = _selectedAnswer.SelectionAnswerData.IsCorrect;
        _selectedAnswer.OnAnswerSubmitted().Forget();
        _selectionQuestionData.NumberOfTries++;
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

#if UNITY_EDITOR
    [Button]
    private void SelectButton(int buttonIndex)
    {
        _selectionQuestionInterfaceReferences.SelectionAnswers[buttonIndex].GetComponent<TXRButton_Toggle>()
            .TriggerToggleEvent(TXRButtonToggleState.On, ButtonColliderResponse.Both);
    }

    private void DeSelectButton(int buttonIndex)
    {
        _selectionQuestionInterfaceReferences.SelectionAnswers[buttonIndex].GetComponent<TXRButton_Toggle>()
            .TriggerToggleEvent(TXRButtonToggleState.Off, ButtonColliderResponse.Both);
    }

    [Button]
    private void SubmitAndRelease()
    {
    }
#endif
}