using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public enum EInteraction
{
    OnInteraction,
    OnStart,
    OnDestroy,
}

public class AudioEvent : MonoBehaviour
{
    [HideInInspector] [SerializeField] private EInteraction _playOn;

    [HideInInspector] [SerializeField] private AInteractionDetector _interactionDetector;

    [HideInInspector] [SerializeField] private AudioPlayer _audioPlayer;

    private void OnEnable()
    {
        if (_playOn == EInteraction.OnInteraction) _interactionDetector.InteractionStarted += _audioPlayer.Play;
    }

    private void OnDisable()
    {
        if (_playOn == EInteraction.OnInteraction) _interactionDetector.InteractionEnded -= _audioPlayer.Play;
    }

    private void Start()
    {
        if (_playOn == EInteraction.OnStart)
        {
            _audioPlayer.Play();
        }
    }

    private void OnDestroy()
    {
        if (_playOn == EInteraction.OnDestroy)
        {
            _audioPlayer.Play();
        }
    }
}