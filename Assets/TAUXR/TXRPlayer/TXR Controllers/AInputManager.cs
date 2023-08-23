using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;
using Cysharp.Threading.Tasks;

public abstract class AInputManager
{
    public CancellationTokenSource WaitForHoldCancellationTokenSource;

    public bool IsPressedThisFrame(HandType handType)
    {
        bool isLeftHeld = IsLeftHeld();
        bool isRightHeld = IsRightHeld();

        switch (handType)
        {
            case HandType.Left:
                return isLeftHeld;
            case HandType.Right:
                return isRightHeld;
            case HandType.Any:
                return isLeftHeld || isRightHeld;
            case HandType.None:
                return false;
            default: return false;
        }
    }

    protected abstract bool IsLeftHeld();
    protected abstract bool IsRightHeld();

    public async UniTask WaitForPress(HandType handType, Action callback = default)
    {
        await WaitForExitIfHolding(handType);
        UniTask.WaitUntil(() => IsPressedThisFrame(handType));
        callback?.Invoke();
    }

    private async UniTask WaitForExitIfHolding(HandType handType)
    {
        if (IsPressedThisFrame(handType))
        {
            await UniTask.WaitUntil(() => !IsPressedThisFrame(handType));
        }
    }

    public async UniTask WaitForHold(HandType handType, float duration,
        Action callback = default)
    {
        using (WaitForHoldCancellationTokenSource = new CancellationTokenSource())
        {
            float holdingDuration = 0;

            await WaitForExitIfHolding(handType);

            bool cancellationRequested = WaitForHoldCancellationTokenSource.IsCancellationRequested;
            while (holdingDuration < duration && !cancellationRequested)
            {
                holdingDuration = IsPressedThisFrame(handType)
                    ? holdingDuration + Time.deltaTime
                    : 0;

                DoWhileHolding(holdingDuration, duration);

                await UniTask.Yield();
            }

            if (!cancellationRequested)
            {
                callback?.Invoke();
            }
        }

        DoOnHoldFinished();
    }

    protected abstract void DoWhileHolding(float holdingDuration, float duration);
    protected abstract void DoOnHoldFinished();
}