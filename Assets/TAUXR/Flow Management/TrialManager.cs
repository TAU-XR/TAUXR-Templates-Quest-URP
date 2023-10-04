using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

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