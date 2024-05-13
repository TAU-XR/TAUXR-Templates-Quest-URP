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
    //It is only public the editor script
    public SelectionQuestionInterfaceReferences SelectionQuestionInterfaceReferences => _selectionQuestionInterfaceReferences;
    [SerializeField] private SelectionQuestionInterfaceReferences _selectionQuestionInterfaceReferences;
    [SerializeField] private SelectionAnswerButtonConfiguration _correctAnswerButtonConfiguration;
    [SerializeField] private SelectionAnswerButtonConfiguration _wrongAnswerButtonConfiguration;

    private SelectionAnswerButton _selectedAnswerButton;

    private bool _correctAnswerSubmitted = false;

    private SelectionQuestionData _selectionQuestionData;

    private SelectionAnswersButtonsManager _selectionAnswersButtonsManager;


    private void Awake()
    {
        _selectionAnswersButtonsManager = new SelectionAnswersButtonsManager(_selectionQuestionInterfaceReferences.SelectionAnswers,
            _correctAnswerButtonConfiguration, _wrongAnswerButtonConfiguration);
    }

    private void OnEnable()
    {
        _selectionQuestionInterfaceReferences.SubmitButton.AnswerSubmitted += OnAnswerSubmitted;
        _selectionAnswersButtonsManager.RegisterButtonEvents();
    }

    private void OnDisable()
    {
        _selectionQuestionInterfaceReferences.SubmitButton.AnswerSubmitted -= OnAnswerSubmitted;
        _selectionAnswersButtonsManager.UnregisterButtonEvents();
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
        _selectionAnswersButtonsManager.InitializeNewAnswers(selectionQuestionData.Answers);
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
            _selectionAnswersButtonsManager.SelectedAnswerButton = GetCorrectAnswer();
            SubmitSelectedAnswer();
        }
    }

    private void SubmitSelectedAnswer()
    {
        if (_selectedAnswerButton == null)
        {
            return;
        }

        AnswerSubmitted?.Invoke(_selectedAnswerButton.SelectionAnswerData);
        _correctAnswerSubmitted = _selectedAnswerButton.SelectionAnswerData.IsCorrect;
        _selectedAnswerButton.OnAnswerSubmitted().Forget();
        _selectionQuestionData.NumberOfTries++;
        ShowAnswerInfo();
    }

    private void ShowAnswerInfo()
    {
        _selectionQuestionInterfaceReferences.AnswerInfo.Show();
        _selectionQuestionInterfaceReferences.AnswerInfo.SetText(_selectedAnswerButton.SelectionAnswerData.AnswerInfo);
    }

    public void Show()
    {
    }

    public void Hide()
    {
        _selectionQuestionInterfaceReferences.AnswerInfo.Hide();
    }

    private SelectionAnswerButton GetCorrectAnswer()
    {
        return _selectionQuestionInterfaceReferences.SelectionAnswers.ToList()
            .Find((selectionAnswer) => selectionAnswer.SelectionAnswerData.IsCorrect);
    }
}