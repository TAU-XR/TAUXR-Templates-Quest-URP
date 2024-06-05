using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;

public class SelectionQuestionInterface : MonoBehaviour
{
    public Action<SelectionAnswerData> AnswerSubmitted;

    [SerializeField] private SelectionQuestionInterfaceReferences _selectionQuestionInterfaceReferences;    // cr: long name, can be simple _references.
    [SerializeField] private SelectionAnswerButtonConfiguration _correctAnswerButtonConfiguration;
    [SerializeField] private SelectionAnswerButtonConfiguration _wrongAnswerButtonConfiguration;
    [SerializeField] private bool _startHidden;


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
        // cr: move to InitializeWithNewQuestion?
    }

    private void OnDisable()
    {
        _selectionQuestionInterfaceReferences.SubmitButton.AnswerSubmitted -= OnAnswerSubmitted;
        // move to ShowQuestionAndWaitForFinalSubmission end?
    }

    private void Start()
    {
        if (_startHidden)
        {
            Hide();
        }
    }

    public async UniTask ShowQuestionAndWaitForFinalSubmission(SelectionQuestionData selectionQuestionData,
        CancellationToken cancellationToken = default)
    {
        Show();
        InitializeWithNewQuestion(selectionQuestionData);

        await UniTask.WaitUntil(() => _correctAnswerSubmitted, cancellationToken: cancellationToken);
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

        SelectionAnswerButton selectedAnswer = _selectionAnswersRadioButtonGroup.SelectedButton.GetComponent<SelectionAnswerButton>();
        _correctAnswerSubmitted = selectedAnswer.SelectionAnswerData.IsCorrect;

        bool noAnswersLeft = _numberOfTriesInCurrentQuestion == _questionData.Answers.Length - 2;
        bool outOfTries = _numberOfTriesInCurrentQuestion - 1 == _questionData.MaxNumberOfTries;
        bool shouldSubmitCorrectAnswer = (outOfTries || noAnswersLeft) && !_correctAnswerSubmitted;

        SubmitSelectedAnswer(showAnswerInfo: !shouldSubmitCorrectAnswer);

        if (!shouldSubmitCorrectAnswer) return;
        _correctAnswerSubmitted = true;
        SubmitCorrectAnswer().Forget(); // cr: confusing name, would implement this inside this method.
    }

    private async UniTask SubmitCorrectAnswer()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(_wrongAnswerButtonConfiguration.TimeFromSubmitToDisable));
        _selectionAnswersRadioButtonGroup.SelectButton(GetCorrectAnswer().GetComponent<TXRButton_Toggle>());
        SubmitSelectedAnswer();
    }

    private void SubmitSelectedAnswer(bool showAnswerInfo = true)
    {
        SelectionAnswerButton selectedAnswer = _selectionAnswersRadioButtonGroup.SelectedButton.GetComponent<SelectionAnswerButton>();
        AnswerSubmitted?.Invoke(selectedAnswer.SelectionAnswerData);
        _numberOfTriesInCurrentQuestion++;
        if (showAnswerInfo)
        {
            ShowAnswerInfo(selectedAnswer.SelectionAnswerData.AnswerInfo);
        }

        _selectionAnswersRadioButtonGroup.Reset();

        selectedAnswer.OnAnswerSubmitted().Forget();

        if (_correctAnswerSubmitted)
        {
            OnCorrectAnswerSubmitted();
        }
    }

    private void OnCorrectAnswerSubmitted()
    {
        foreach (TXRButton_Toggle button in _selectionAnswersRadioButtonGroup.Buttons)
        {
            if (button.GetComponent<SelectionAnswerButton>().SelectionAnswerData.IsCorrect)
            {
                continue;
            }

            button.SetState(ButtonState.Disabled);
        }
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
        _selectionQuestionInterfaceReferences.Graphics.SetActive(true);
    }

    public void Hide()
    {
        if (_selectionQuestionInterfaceReferences.AnswerInfo != null)
        {
            _selectionQuestionInterfaceReferences.AnswerInfo.Hide();
        }

        _selectionQuestionInterfaceReferences.Graphics.SetActive(false);
    }
}