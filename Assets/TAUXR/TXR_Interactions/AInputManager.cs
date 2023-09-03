using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;
using Cysharp.Threading.Tasks;

//TODO: create interface (to see the methods)
public abstract class AInputManager
{
    public CancellationTokenSource WaitForHoldCancellationTokenSource;
    public CancellationTokenSource WaitForPressCancellationTokenSource;

    public bool IsInputPressedThisFrame(HandType handType)
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

    public abstract bool IsLeftHeld();
    public abstract bool IsRightHeld();

    public async UniTask WaitForPress(HandType handType, Action callback = default)
    {
        using (WaitForPressCancellationTokenSource = new CancellationTokenSource())
        {
            try
            {
                await WaitForReleaseIfHolding(handType, WaitForPressCancellationTokenSource.Token);
                await UniTask.WaitUntil(() => IsInputPressedThisFrame(handType),
                    cancellationToken: WaitForPressCancellationTokenSource.Token);
                callback?.Invoke();
                DoOnPress();
            }
            catch (OperationCanceledException ex)
            {
                Debug.Log("Wait for press cancelled");
            }
        }
    }

    protected virtual void DoOnPress()
    {
    }

    private async UniTask WaitForReleaseIfHolding(HandType handType,
        CancellationToken cancellationToken = default)
    {
        if (IsInputPressedThisFrame(handType))
        {
            await UniTask.WaitUntil(() => !IsInputPressedThisFrame(handType),
                cancellationToken: cancellationToken);
        }
    }

    public async UniTask WaitForHoldAndRelease(HandType handType, float duration, Action callback = default)
    {
        try
        {
            await WaitForHold(handType, duration);
            await WaitForReleaseIfHolding(handType, WaitForHoldCancellationTokenSource.Token);
            callback?.Invoke();
        }
        catch (OperationCanceledException ex)
        {
            Debug.Log("Wait for hold and release cancelled");
        }
    }

    public async UniTask WaitForHold(HandType handType, float duration,
        Action callback = default)
    {
        using (WaitForHoldCancellationTokenSource = new CancellationTokenSource())
        {
            try
            {
                //If the player is already holding when the method starts, we wait for release and a new hold
                await WaitForReleaseIfHolding(handType, WaitForHoldCancellationTokenSource.Token);

                float holdingTime = 0;
                while (holdingTime < duration)
                {
                    holdingTime = IsInputPressedThisFrame(handType)
                        ? holdingTime + Time.deltaTime
                        : 0;

                    DoWhileHolding(handType, holdingTime, duration);

                    await UniTask.Yield(cancellationToken: WaitForHoldCancellationTokenSource.Token);
                }

                callback?.Invoke();
            }
            catch (OperationCanceledException ex)
            {
                Debug.Log("Hold cancelled");
            }
        }

        DoOnHoldFinished(handType);
    }

    protected virtual void DoWhileHolding(HandType handType, float holdingDuration, float duration)
    {
    }

    protected virtual void DoOnHoldFinished(HandType handType)
    {
    }

    public async UniTask WaitForInputsInARow(int numberOfInputsInARow, float maxTimeBetweenInputs,
        Action successCallback, Action timeOutCallback = default,
        bool alternateHands = false, HandType startingHand = HandType.Right)
    {
        int currentNumberOfInputs = 1;
        float currentTime = 0;
        HandType nextHandType = startingHand;
        while (currentNumberOfInputs < numberOfInputsInARow)
        {
            bool inputReceived = alternateHands
                ? IsInputPressedThisFrame(nextHandType)
                : IsInputPressedThisFrame(HandType.Any);
            if (inputReceived)
            {
                currentNumberOfInputs++;
                currentTime = 0;
                nextHandType = nextHandType == HandType.Left ? HandType.Right : HandType.Left;
            }

            await UniTask.Yield();
            currentTime += Time.deltaTime;

            if (currentTime > maxTimeBetweenInputs)
            {
                timeOutCallback?.Invoke();
                return;
            }
        }

        successCallback();
    }
}