using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public enum EEvent
{
    OnStart,

    //Maybe: Instead of on start - OnEnable, with checkbox for if to only play once (on start or on enable)
    OnSpecificDetection,
    OnDestroy,

    //OnDisable
    Custom,
}

public class AudioEvent : MonoBehaviour
{
    [HideInInspector] [SerializeField] private EEvent _playOn;

    [HideInInspector] [SerializeField] private ADetector _detector;

    [HideInInspector] [SerializeField] private AudioPlayer _audioPlayer;

    private void OnEnable()
    {
        if (_playOn == EEvent.OnSpecificDetection) _detector.InteractionStarted += _audioPlayer.Play;
    }

    private void OnDisable()
    {
        if (_playOn == EEvent.OnSpecificDetection) _detector.InteractionEnded -= _audioPlayer.Play;
    }

    private void Start()
    {
        if (_playOn == EEvent.OnStart)
        {
            _audioPlayer.Play();
        }
    }

    private void OnDestroy()
    {
        if (_playOn == EEvent.OnDestroy)
        {
            _audioPlayer.Play();
        }
    }
}