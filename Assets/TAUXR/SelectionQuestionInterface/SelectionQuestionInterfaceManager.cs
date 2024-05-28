using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class SelectionQuestionInterfaceManager : MonoBehaviour
{
    private CancellationTokenSource _cts;

    public int NumberOfSelectionQuestions => _selectionQuestions.Length;

    [SerializeField] private SelectionQuestionData[] _selectionQuestions;
    [SerializeField] private float _timeBetweenQuestions = 5;
    [SerializeField] [HideInInspector] private int _startingQuestionIndex = 0;
    [SerializeField] [ReadOnly] private int _currentQuestionIndex;

    private void Start()
    {
        _currentQuestionIndex = _startingQuestionIndex;
        RunExamFromCurrentQuestion().Forget();
    }

    private async UniTask RunExamFromCurrentQuestion()
    {
        _cts = new CancellationTokenSource();
        while (_currentQuestionIndex < _selectionQuestions.Length)
        {
            await GetComponent<SelectionQuestionInterface>()
                .ShowQuestionAndWaitForFinalSubmission(_selectionQuestions[_currentQuestionIndex], _cts.Token);
            _currentQuestionIndex++;
            Debug.Log("Before delay");
            await UniTask.Delay(TimeSpan.FromSeconds(_timeBetweenQuestions), cancellationToken: _cts.Token);
            Debug.Log("after delay");
        }
    }

    public void NextQuestion()
    {
        _cts.Cancel();
        _currentQuestionIndex++;
        RunExamFromCurrentQuestion().Forget();
    }

    public void PreviousQuestion()
    {
        _cts.Cancel();
        _currentQuestionIndex--;
        RunExamFromCurrentQuestion().Forget();
    }
}