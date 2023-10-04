using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

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