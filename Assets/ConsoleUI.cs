using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsoleUI : MonoBehaviour
{
    [SerializeField] private TextDisplay _textDisplay;
    private float _timeSinceStartup = 0;
    public bool DebugConsoleUi;

    void OnEnable()
    {
        Application.logMessageReceived += LogCallback;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= LogCallback;
    }

    private void Update()
    {
        _timeSinceStartup += Time.deltaTime;
        if (DebugConsoleUi)
        {
            _textDisplay.Show();
            _textDisplay.transform.position = TXRPlayer.Instance.PlayerHead.position + TXRPlayer.Instance.PlayerHead.transform.forward * 2;
        }
    }

    void LogCallback(string logString, string stackTrace, LogType type)
    {
        if (!(_timeSinceStartup > 1)) return;
        _textDisplay.SetText(logString);
    }

    public void ExitDebugMode()
    {
        _textDisplay.Hide();
    }
}