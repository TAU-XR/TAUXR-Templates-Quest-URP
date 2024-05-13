using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;

public class SelectionQuestionInterfaceTester : MonoBehaviour
{
    public SelectionQuestionData[] SelectionQuestions;
    [SerializeField] private float _timeBetweenQuestions = 5;

    private async void Start()
    {
        foreach (SelectionQuestionData selectionQuestion in SelectionQuestions)
        {
            await GetComponent<SelectionQuestionInterface>().ShowQuestionAndWaitForFinalSubmission(selectionQuestion);
            await UniTask.Delay(TimeSpan.FromSeconds(_timeBetweenQuestions));
        }
    }
}