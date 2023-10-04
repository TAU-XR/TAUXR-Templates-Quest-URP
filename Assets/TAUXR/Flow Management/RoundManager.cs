using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

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