using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;

public class SelectionQuestionInterface : MonoBehaviour, ITXRQuestionInterface
{
    public Action<SQIAnswerButton> AnswerSubmitted;

    public Color CorrectAnswerColor;
    public Color WrongAnswerColor;
    public int MaxNumberOfTries;
    private const float SUBMIT_TO_DISABLE_DURATION = 1;

    [SerializeField] private SQIReferences _references;

    private int _currentNumberOfTries;
    private bool _finishedAnswering = false;
    private bool _isDone = false;
    private bool _shouldSkip = false;

    private void OnEnable()
    {
        Init();
        ShowQuestionAndWaitForFinalSubmission().Forget();
    }


    [Button]
    public void Init()
    {
        foreach (SQIAnswerButton answerButton in _references.SelectionAnswers)
            answerButton.Init(CorrectAnswerColor, WrongAnswerColor, SUBMIT_TO_DISABLE_DURATION);
        _references.QuestionTextDisplay.Init();
        _references.AnswerInfo.Init();
        _references.SubmitButton.Init();
    }

    [Button]
    public void Show()
    {
        _references.SubmitButton.SetHidden(false);
        foreach (SQIAnswerButton answerButton in _references.SelectionAnswers)
            answerButton.SetHidden(false);

        _references.RadioButtonGroup.OnAnswerSelected += OnAnswerSelected;
        _references.RadioButtonGroup.OnAnswerDeselected += OnAnswerDeselected;
    }

    [Button]
    public void Hide()
    {
        _references.SubmitButton.SetHidden(true);

        foreach (SQIAnswerButton answerButton in _references.SelectionAnswers)
            answerButton.SetHidden(true);

        _references.AnswerInfo.Hide();

        _references.RadioButtonGroup.OnAnswerSelected -= OnAnswerSelected;
        _references.RadioButtonGroup.OnAnswerDeselected -= OnAnswerDeselected;
    }

    public bool IsDone() => _isDone;

    public async UniTask ShowQuestionAndWaitForFinalSubmission()
    {
        _references.SubmitButton.AnswerSubmitted += OnAnswerSubmitted;

        Show();
        ResetInterface();
        foreach (SQIAnswerButton answerButton in _references.SelectionAnswers) answerButton.ResetAnswer();
        _references.RadioButtonGroup.Reset();  // turn all answer buttons off
        _references.SubmitButton.Button.SetState(TXRButtonState.Disabled);
        await UniTask.WaitUntil(() => _finishedAnswering || _shouldSkip);

        _references.SubmitButton.AnswerSubmitted -= OnAnswerSubmitted;
    }

    private void ResetInterface()
    {
        _currentNumberOfTries = 0;
        _references.AnswerInfo.Hide();
        _finishedAnswering = false;
    }

    // called when submit button is pressed
    private void OnAnswerSubmitted()
    {
        if (_references.RadioButtonGroup.SelectedButton == null) return;   // do nothing if no answer was selected before submittion

        SQIAnswerButton selectedAnswer = _references.RadioButtonGroup.SelectedButton.GetComponent<SQIAnswerButton>();
        SubmitAnswer(selectedAnswer).Forget();
    }

    private async UniTask SubmitAnswer(SQIAnswerButton selectedAnswer)
    {
        _references.RadioButtonGroup.Reset();
        _currentNumberOfTries++;
        selectedAnswer.SubmitAnswer().Forget();
        AnswerSubmitted?.Invoke(selectedAnswer);

        if (selectedAnswer.IsCorrect)
        {
            OnCorrectAnswerSubmitted(selectedAnswer);
            await UniTask.Delay(2000);
            _finishedAnswering = true;
        }
        else
        {
            await OnWrongAnswerSubmitted(selectedAnswer);
        }

    }

    private void OnCorrectAnswerSubmitted(SQIAnswerButton selectedAnswer)
    {
        ShowAnswerInfo(selectedAnswer.AnswerInfo);
        DisableWrongAnswers();
        _references.CorrectAnswerAudio.Play();
    }

    private void DisableWrongAnswers()
    {
        foreach (TXRButton_Toggle button in _references.RadioButtonGroup.Buttons)
        {
            if (!button.gameObject.activeSelf) continue;
            if (button.GetComponent<SQIAnswerButton>().IsCorrect)
            {
                continue;
            }

            button.SetState(TXRButtonState.Disabled);
        }
    }

    private async UniTask OnWrongAnswerSubmitted(SQIAnswerButton selectedAnswer)
    {
        _references.WrongAnswerAudio.Play();

        bool noAnswersLeft = _currentNumberOfTries == _references.RadioButtonGroup.NumberOfButtons - 1;
        bool outOfTries = _currentNumberOfTries == MaxNumberOfTries;

        if (outOfTries || noAnswersLeft)
        {
            await ShowCorrectAnswer();
        }
        else
        {
            ShowAnswerInfo(selectedAnswer.AnswerInfo);
        }
    }

    private async UniTask ShowCorrectAnswer()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(SUBMIT_TO_DISABLE_DURATION));
        DisableWrongAnswers();
        SQIAnswerButton correctAnswer = GetCorrectAnswer();
        correctAnswer.SubmitAnswer().Forget();
        ShowAnswerInfo(correctAnswer.AnswerInfo);
        await UniTask.Delay(2000);
        _finishedAnswering = true;
    }


    private void ShowAnswerInfo(string selectedAnswerInfo)
    {
        if (selectedAnswerInfo == "") return;
        _references.AnswerInfo.Show();
        _references.AnswerInfo.SetText(selectedAnswerInfo);
    }

    private SQIAnswerButton GetCorrectAnswer()
    {
        return _references.SelectionAnswers.ToList()
            .Find((selectionAnswer) => selectionAnswer.IsCorrect);
    }

    private void OnAnswerSelected()
    {
        _references.SubmitButton.Button.SetState(TXRButtonState.Active);
    }

    private void OnAnswerDeselected()
    {
        _references.SubmitButton.Button.SetState(TXRButtonState.Disabled);
    }
}