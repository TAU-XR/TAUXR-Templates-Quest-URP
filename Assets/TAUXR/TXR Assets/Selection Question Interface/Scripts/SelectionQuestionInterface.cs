using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

/*
 * Current issues:
 * - When submitting without submit button - if answer button is pressed and then "hover" - the color won't change to green/red because the hover state will get after it and override.
 * - No option to Show/Hide or Reset without animation
 * 
 */

public class SelectionQuestionInterface : MonoBehaviour, ITXRQuestionInterface
{
    public Action<SQIAnswerButton> AnswerSubmitted;

    public bool UseSubmitButton = true;
    public bool ShouldResetAfterFirstUse = false;
    public Color CorrectAnswerColor;
    public Color WrongAnswerColor;
    public int MaxNumberOfTries;

    public UnityEvent OnSubmittionEnd;
    private const float SUBMIT_TO_DISABLE_DURATION = 1;

    [SerializeField] private SQIReferences _references;

    private int _currentNumberOfTries;
    private bool _finishedAnswering = false;
    private bool _isDone = false;
    private bool _shouldSkip = false;
    private bool _isFirstUse = true;
    private bool _isOnQuestionProcess = true;
    private void OnEnable()
    {
        _references.SubmitButton.AnswerSubmitted += OnAnswerSubmitted;
        _references.RadioButtonGroup.OnButtonSelected += OnAnswerSelected;
        _references.RadioButtonGroup.OnButtonDeselected += OnAnswerDeselected;

        if (!ShouldResetAfterFirstUse && !_isFirstUse) return;

        Init();
        ShowQuestionAndWaitForFinalSubmission().Forget();

    }

    private void OnDisable()
    {
        _references.SubmitButton.AnswerSubmitted -= OnAnswerSubmitted;
        _references.RadioButtonGroup.OnButtonSelected -= OnAnswerSelected;
        _references.RadioButtonGroup.OnButtonDeselected -= OnAnswerDeselected;
    }

    [Button]
    public void Init()
    {
        _references.InitAnswerArray(GetComponentsInChildren<SQIAnswerButton>());

        foreach (SQIAnswerButton answerButton in _references.SelectionAnswers)
            answerButton.Init(CorrectAnswerColor, WrongAnswerColor);
        //_references.QuestionTextDisplay.Init();
        _references.AnswerInfo.Init();
        _references.SubmitButton.Init();

        if (!UseSubmitButton)
            _references.SubmitButton.gameObject.SetActive(false);

        MaxNumberOfTries = Mathf.Min(MaxNumberOfTries, _references.SelectionAnswers.Count() - 1);
        _currentNumberOfTries = 0;
    }

    [Button]
    public void Show()
    {
        if (UseSubmitButton)
            _references.SubmitButton.SetHidden(false);

        foreach (SQIAnswerButton answerButton in _references.SelectionAnswers)
            answerButton.SetHidden(false);
    }

    [Button]
    public void Hide()
    {
        if (UseSubmitButton)
            _references.SubmitButton.SetHidden(true);

        foreach (SQIAnswerButton answerButton in _references.SelectionAnswers)
            answerButton.SetHidden(true);

        _references.AnswerInfo.Hide();
    }

    public bool IsDone() => _isDone;

    public async UniTask ShowQuestionAndWaitForFinalSubmission()
    {
        Show();
        ResetInterface();
        foreach (SQIAnswerButton answerButton in _references.SelectionAnswers) answerButton.ResetAnswer();
        _references.RadioButtonGroup.Reset();  // diselects selected answer

        if (UseSubmitButton)
            _references.SubmitButton.Button.SetState(TXRButtonState.Disabled);

        await UniTask.WaitUntil(() => _finishedAnswering || _shouldSkip);

        _isFirstUse = false;
        OnSubmittionEnd.Invoke();

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
        await UniTask.Delay(100);   // wait for button to play release visuals and only then change color to submitted answer color
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
                button.SetInteractable(false);
                continue;
            }

            button.SetInteractable(false);
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
        if (UseSubmitButton)
        {
            _references.SubmitButton.Button.SetState(TXRButtonState.Active);
        }
        else
        {
            if (_references.RadioButtonGroup.SelectedButton == null) return;   // do nothing if no answer was selected before submittion
            SQIAnswerButton selectedAnswer = _references.RadioButtonGroup.SelectedButton.GetComponent<SQIAnswerButton>();
            SubmitAnswer(selectedAnswer).Forget();
        }
    }

    private void OnAnswerDeselected()
    {
        if (!UseSubmitButton) return;
        _references.SubmitButton.Button.SetState(TXRButtonState.Disabled);
    }
}