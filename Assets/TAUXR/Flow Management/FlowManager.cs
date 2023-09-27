using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor;


//Important features
//1. There is a place where you can see the entire flow
//2. Functionality has no dependency in flow.
public class TrialManager : TXRSingleton<TrialManager>
{
    public async UniTask RunTrialFlow(Trial trial)
    {
        StartTrial(trial);

        // all trial flow. Activating and waiting for project specific functionalities.

        EndTrial(trial);
    }

    private void StartTrial(Trial trial)
    {
        // setup trial initial conditions.
        RunTrialFlow(trial).Forget();
    }


    private void EndTrial(Trial trial)
    {
        // setup trial end conditions.
    }
}

public class Trial
{
}

public class RoundManager : TXRSingleton<RoundManager>
{
    [SerializeField] private Trial[] _trials;
    private int _currentTrial = 0;

    public async UniTask RunRoundFlow(Round round)
    {
        StartRound(round);

        while (_currentTrial < _trials.Length)
        {
            await TrialManager.Instance.RunTrialFlow(_trials[_currentTrial]);
            await BetweenTrialsFlow(round);
            _currentTrial++;
        }

        EndRound(round);
    }

    private void StartRound(Round round)
    {
        // setup round initial conditions.
        RunRoundFlow(round).Forget();
    }


    private void EndRound(Round round)
    {
        // setup end round conditions
        SessionManager.Instance.OnRoundEnd();
    }

    private async UniTask BetweenTrialsFlow(Round round)
    {
    }
}

public class Round
{
    // holds all round configurations
}

public class SessionManager : TXRSingleton<SessionManager>
{
    [SerializeField] private Round[] _rounds;
    private int _currentRound;

    public async UniTask RunSessionFlow(Round round)
    {
        StartSession(round);

        while (_currentRound < _rounds.Length)
        {
            await RoundManager.Instance.RunRoundFlow(_rounds[_currentRound]);
            await BetweenRoundsFlow(round);
            _currentRound++;
        }

        EndSession(round);
    }


    private void StartSession(Round round)
    {
        // setup session initial conditions.
        RunSessionFlow(round).Forget();
    }


    private void EndSession(Round round)
    {
        // setup end session conditions
        // If there is a higher flow unit manager, tell him the session finished.
    }

    public void OnRoundEnd()
    {
    }

    private async UniTask BetweenRoundsFlow(Round round)
    {
        throw new NotImplementedException();
    }
}