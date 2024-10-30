using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;

public class SelectionQuestionInterface : MonoBehaviour, ITXRQuestionInterface
{
    public Action<SelectionAnswerData> AnswerSubmitted;

    [SerializeField] private SelectionQuestionInterfaceReferences _references;

    // remove scriptableObject. use time as const and color as serialized fields.
    [SerializeField] private SelectionAnswerButtonConfiguration _correctAnswerButtonConfiguration;
    [SerializeField] private SelectionAnswerButtonConfiguration _wrongAnswerButtonConfiguration;
    [SerializeField] private bool _startHidden;


    private bool _finishedAnswering = false;
    private bool _isDone = false;
    private SelectionQuestionData _questionData;

    private TXRRadioButtonGroup _selectionAnswersRadioButtonGroup;

    private int _numberOfTriesInCurrentQuestion;

    private bool _shouldDisplayText = true;
    public void SetWithOrWithoutText(bool shouldDisplayText)
    {
        _shouldDisplayText = shouldDisplayText;
    }

    private void Awake()
    {
        _selectionAnswersRadioButtonGroup = GetComponent<TXRRadioButtonGroup>();
    }

    private void OnEnable()
    {
        _references.SubmitButton.AnswerSubmitted += OnAnswerSubmitted;
    }

    private void OnDisable()
    {
        _references.SubmitButton.AnswerSubmitted -= OnAnswerSubmitted;

    }

    [Button]
    public void Init()
    {
        _selectionAnswersRadioButtonGroup = GetComponent<TXRRadioButtonGroup>();
        foreach (SelectionAnswerButton answerButton in _references.SelectionAnswers) answerButton.Init();
        _references.QuestionTextDisplay.Init();
        _references.AnswerInfo.Init();
        _references.SubmitButton.Init();
    }

    [Button]
    public void Show()
    {
        _references.SubmitButton.SetHidden(false);
        foreach (SelectionAnswerButton answerButton in _references.SelectionAnswers) answerButton.SetHidden(false);

        if (_shouldDisplayText)
            _references.QuestionTextDisplay.Show();

        _selectionAnswersRadioButtonGroup.OnAnswerSelected += OnAnswerSelected;
        _selectionAnswersRadioButtonGroup.OnAnswerDeselected += OnAnswerDeselected;
    }

    [Button]
    public void Hide()
    {
        if (_references.AnswerInfo != null)
        {
            _references.AnswerInfo.Hide();
        }

        _references.SubmitButton.SetHidden(true);
        foreach (SelectionAnswerButton answerButton in _references.SelectionAnswers) answerButton.SetHidden(true);
        _references.AnswerInfo?.Hide();

        if (_shouldDisplayText)
            _references.QuestionTextDisplay.Hide();

        _selectionAnswersRadioButtonGroup.OnAnswerSelected -= OnAnswerSelected;
        _selectionAnswersRadioButtonGroup.OnAnswerDeselected -= OnAnswerDeselected;
    }

    public bool IsDone() => _isDone;

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
        _references.SubmitButton.Button.SetState(TXRButtonState.Disabled);
        await UniTask.WaitUntil(() => _finishedAnswering, cancellationToken: cancellationToken);
    }

    private void InitializeWithNewQuestion(SelectionQuestionData selectionQuestionData)
    {
        _numberOfTriesInCurrentQuestion = 0;
        _references.AnswerInfo.Hide();
        _questionData = selectionQuestionData;
        _finishedAnswering = false;
        _references.QuestionTextDisplay.SetText(_questionData.Text);
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
            //selectionAnswerButton.SetHidden(!answerIsActive);
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
        if (_selectionAnswersRadioButtonGroup.SelectedButton == null) return;

        SelectionAnswerButton selectedAnswer = _selectionAnswersRadioButtonGroup.SelectedButton.GetComponent<SelectionAnswerButton>();
        SubmitAnswer(selectedAnswer).Forget();
    }

    private async UniTask SubmitAnswer(SelectionAnswerButton selectedAnswer)
    {
        if (_selectionAnswersRadioButtonGroup.SelectedButton == null) return;

        _selectionAnswersRadioButtonGroup.Reset();
        _numberOfTriesInCurrentQuestion++;
        selectedAnswer.OnAnswerSubmitted().Forget();
        AnswerSubmitted?.Invoke(selectedAnswer.SelectionAnswerData);

        if (selectedAnswer.SelectionAnswerData.IsCorrect)
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

    private void OnCorrectAnswerSubmitted(SelectionAnswerButton selectedAnswer)
    {
        ShowAnswerInfo(selectedAnswer.SelectionAnswerData.AnswerInfo);
        DisableWrongAnswers();
        _references.CorrectAnswerAudio.Play();
    }

    private void DisableWrongAnswers()
    {
        foreach (TXRButton_Toggle button in _selectionAnswersRadioButtonGroup.Buttons)
        {
            if (!button.gameObject.activeSelf) continue;
            if (button.GetComponent<SelectionAnswerButton>().SelectionAnswerData.IsCorrect)
            {
                continue;
            }

            button.SetState(TXRButtonState.Disabled);
        }
    }

    private async UniTask OnWrongAnswerSubmitted(SelectionAnswerButton selectedAnswer)
    {
        bool noAnswersLeft = _numberOfTriesInCurrentQuestion == _questionData.Answers.Length - 1;
        bool outOfTries = _numberOfTriesInCurrentQuestion == _questionData.MaxNumberOfTries;
        _references.WrongAnswerAudio.Play();
        if (outOfTries || noAnswersLeft)
        {
            await ShowCorrectAnswer();
        }
        else
        {
            ShowAnswerInfo(selectedAnswer.SelectionAnswerData.AnswerInfo);
        }
    }

    private async UniTask ShowCorrectAnswer()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(_wrongAnswerButtonConfiguration.TimeFromSubmitToDisable));
        DisableWrongAnswers();
        SelectionAnswerButton correctAnswer = GetCorrectAnswer();
        correctAnswer.OnAnswerSubmitted().Forget();
        ShowAnswerInfo(correctAnswer.SelectionAnswerData.AnswerInfo);
        await UniTask.Delay(2000);
        _finishedAnswering = true;
    }


    private void ShowAnswerInfo(string selectedAnswerInfo)
    {
        if (selectedAnswerInfo == "") return;
        _references.AnswerInfo.Show();
        _references.AnswerInfo.SetText(selectedAnswerInfo);
    }

    private SelectionAnswerButton GetCorrectAnswer()
    {
        return _references.SelectionAnswers.ToList()
            .Find((selectionAnswer) => selectionAnswer.SelectionAnswerData.IsCorrect);
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