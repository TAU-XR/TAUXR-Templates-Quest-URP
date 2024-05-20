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

    [SerializeField] private SelectionQuestionInterfaceReferences _selectionQuestionInterfaceReferences;
    [SerializeField] private SelectionAnswerButtonConfiguration _correctAnswerButtonConfiguration;
    [SerializeField] private SelectionAnswerButtonConfiguration _wrongAnswerButtonConfiguration;


    private bool _correctAnswerSubmitted = false;
    private SelectionQuestionData _questionData;

    private TXRRadioButtonGroup _selectionAnswersRadioButtonGroup;

    private int _numberOfTriesInCurrentQuestion;

    private void Awake()
    {
        _selectionAnswersRadioButtonGroup = GetComponent<TXRRadioButtonGroup>();
    }


    private void OnEnable()
    {
        _selectionQuestionInterfaceReferences.SubmitButton.AnswerSubmitted += OnAnswerSubmitted;
    }

    private void OnDisable()
    {
        _selectionQuestionInterfaceReferences.SubmitButton.AnswerSubmitted -= OnAnswerSubmitted;
    }

    public async UniTask ShowQuestionAndWaitForFinalSubmission(SelectionQuestionData selectionQuestionData)
    {
        InitializeWithNewQuestion(selectionQuestionData);

        await UniTask.WaitUntil(() => _correctAnswerSubmitted);
    }

    private void InitializeWithNewQuestion(SelectionQuestionData selectionQuestionData)
    {
        _numberOfTriesInCurrentQuestion = 0;
        _selectionQuestionInterfaceReferences.AnswerInfo.Hide();
        _questionData = selectionQuestionData;
        _correctAnswerSubmitted = false;
        _selectionQuestionInterfaceReferences.QuestionTextDisplay.SetText(_questionData.Text);
        InitializeNewAnswers(selectionQuestionData.Answers);
    }

    private void InitializeNewAnswers(SelectionAnswerData[] selectionAnswersData)
    {
        _selectionAnswersRadioButtonGroup.Reset();
        int numberOfAnswersInInterface = _selectionAnswersRadioButtonGroup.Buttons.Length;
        for (int answerIndex = 0; answerIndex < numberOfAnswersInInterface; answerIndex++)
        {
            SelectionAnswerButton selectionAnswerButton =
                _selectionAnswersRadioButtonGroup.Buttons[answerIndex].GetComponent<SelectionAnswerButton>();
            bool answerIsActive = answerIndex < selectionAnswersData.Length;
            selectionAnswerButton.gameObject.SetActive(answerIsActive);
            if (!answerIsActive) continue;
            bool isCorrectAnswer = selectionAnswersData[answerIndex].IsCorrect;
            SelectionAnswerButtonConfiguration buttonConfiguration =
                isCorrectAnswer ? _correctAnswerButtonConfiguration : _wrongAnswerButtonConfiguration;
            selectionAnswerButton.SetNewAnswer(selectionAnswersData[answerIndex], buttonConfiguration);
        }
    }

    private void OnAnswerSubmitted()
    {
        if (_selectionAnswersRadioButtonGroup.SelectedButton == null)
        {
            return;
        }

        SubmitSelectedAnswer();


        bool noAnswersLeft = _numberOfTriesInCurrentQuestion == _questionData.Answers.Length - 1;
        bool outOfTries = _numberOfTriesInCurrentQuestion == _questionData.MaxNumberOfTries;
        bool selectCorrectAnswer = outOfTries || noAnswersLeft;
        if (!selectCorrectAnswer || _correctAnswerSubmitted) return;

        _selectionAnswersRadioButtonGroup.SelectButton(GetCorrectAnswer().GetComponent<TXRButton_Toggle>());
        SubmitSelectedAnswer();
    }

    private void SubmitSelectedAnswer()
    {
        SelectionAnswerButton selectedAnswer = _selectionAnswersRadioButtonGroup.SelectedButton.GetComponent<SelectionAnswerButton>();
        AnswerSubmitted?.Invoke(selectedAnswer.SelectionAnswerData);
        _correctAnswerSubmitted = selectedAnswer.SelectionAnswerData.IsCorrect;
        selectedAnswer.OnAnswerSubmitted().Forget();
        _numberOfTriesInCurrentQuestion++;
        ShowAnswerInfo(selectedAnswer.SelectionAnswerData.AnswerInfo);
        _selectionAnswersRadioButtonGroup.Reset();

        if (_correctAnswerSubmitted)
        {
            OnCorrectAnswerSubmitted();
        }
    }

    private void OnCorrectAnswerSubmitted()
    {
        //TODO: disable all other buttons.
    }

    private void ShowAnswerInfo(string selectedAnswerInfo)
    {
        _selectionQuestionInterfaceReferences.AnswerInfo.Show();
        _selectionQuestionInterfaceReferences.AnswerInfo.SetText(selectedAnswerInfo);
    }

    private SelectionAnswerButton GetCorrectAnswer()
    {
        return _selectionQuestionInterfaceReferences.SelectionAnswers.ToList()
            .Find((selectionAnswer) => selectionAnswer.SelectionAnswerData.IsCorrect);
    }

    public void Show()
    {
    }

    public void Hide()
    {
        _selectionQuestionInterfaceReferences.AnswerInfo.Hide();
    }
}