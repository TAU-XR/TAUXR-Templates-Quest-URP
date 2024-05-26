using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;

public class SelectionQuestionInterfaceManager : MonoBehaviour
{
    public SelectionQuestionData[] SelectionQuestions;
    [SerializeField] private float _timeBetweenQuestions = 5;
    private int _currentQuestionIndex = 0;

    private void Start()
    {
        RunExam().Forget();
    }

    private async UniTask RunExam()
    {
        foreach (SelectionQuestionData selectionQuestion in SelectionQuestions)
        {
            await GetComponent<SelectionQuestionInterface>().ShowQuestionAndWaitForFinalSubmission(selectionQuestion);
            await UniTask.Delay(TimeSpan.FromSeconds(_timeBetweenQuestions));
        }
    }

    public void NextQuestion()
    {
        
    }

    public void PreviousQuestion()
    {
    }
}