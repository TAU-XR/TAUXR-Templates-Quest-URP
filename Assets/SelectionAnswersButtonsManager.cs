using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionAnswersButtonsManager
{
    private SelectionAnswerButtonConfiguration _correctAnswerButtonConfiguration;
    private SelectionAnswerButtonConfiguration _wrongAnswerButtonConfiguration;
    [HideInInspector] public SelectionAnswerButton[] _answerButtons;

    [HideInInspector] public SelectionAnswerButton SelectedAnswerButton;

    public SelectionAnswersButtonsManager(SelectionAnswerButton[] answerButtons,
        SelectionAnswerButtonConfiguration correctAnswerButtonConfiguration,
        SelectionAnswerButtonConfiguration wrongAnswerButtonConfiguration)
    {
        _answerButtons = answerButtons;
        _correctAnswerButtonConfiguration = correctAnswerButtonConfiguration;
        _wrongAnswerButtonConfiguration = wrongAnswerButtonConfiguration;
    }


    public void InitializeNewAnswers(SelectionAnswerData[] selectionAnswersData)
    {
        SelectedAnswerButton = null;
        int numberOfAnswersInInterface = _answerButtons.Length;
        for (int answerIndex = 0; answerIndex < numberOfAnswersInInterface; answerIndex++)
        {
            SelectionAnswerButton selectionAnswerButton = _answerButtons[answerIndex];
            bool answerIsActive = answerIndex < selectionAnswersData.Length;
            selectionAnswerButton.gameObject.SetActive(answerIsActive);
            if (answerIsActive)
            {
                selectionAnswerButton.Init(selectionAnswersData[answerIndex]);
            }
        }
    }

    public void RegisterButtonEvents()
    {
        foreach (SelectionAnswerButton selectionAnswer in _answerButtons)
        {
            selectionAnswer.AnswerSelected += () => OnAnswerSelected(selectionAnswer);
            selectionAnswer.AnswerDeselected += () => OnAnswerDeselected(selectionAnswer);
        }
    }

    public void UnregisterButtonEvents()
    {
        foreach (SelectionAnswerButton selectionAnswer in _answerButtons)
        {
            selectionAnswer.AnswerSelected -= () => OnAnswerSelected(selectionAnswer);
            selectionAnswer.AnswerDeselected -= () => OnAnswerDeselected(selectionAnswer);
        }
    }


    private void OnAnswerSelected(SelectionAnswerButton selectionAnswerButton)
    {
        Debug.Log(SelectedAnswerButton.name);
        if (selectionAnswerButton == SelectedAnswerButton)
        {
            return;
        }

        if (SelectedAnswerButton != null)
        {
            SelectedAnswerButton.ManuallyDeselectAnswer();
        }

        SelectedAnswerButton = selectionAnswerButton;
        Debug.Log(SelectedAnswerButton.name);
    }

    private void OnAnswerDeselected(SelectionAnswerButton selectionAnswerButton)
    {
        if (selectionAnswerButton == SelectedAnswerButton)
        {
            SelectedAnswerButton = null;
        }
    }
}