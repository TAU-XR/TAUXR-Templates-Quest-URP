using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

// You can only use this to stop the last sound played
// To keep track of multiple sounds - a more advanced system is needed
[CreateAssetMenu(menuName = "Audio/Audio Player", fileName = "Audio Player")]
[ExecuteInEditMode]
public class AudioPlayer : ScriptableObject
{
    private AudioSource _lastAudioSource;
    [SerializeField] private AudioClip[] _audioClips;
    [SerializeField] private EIterationOrder _playOrder = EIterationOrder.InOrder;
    [SerializeField] private bool _loop = false;
    [SerializeField] private bool _dynamicVolumeRange = false;
    [SerializeField] private float _volume = 1;
    [SerializeField] private Vector2 _volumeRange = new(1, 1);
    [SerializeField] private bool _dynamicPitchRange = false;
    [SerializeField] private float _pitch = 1;
    [SerializeField] private Vector2 _pitchRange = new(1, 1);
    [SerializeField] private EFadeType _fadeType = EFadeType.None;
    [SerializeField] private float _fadeDuration = 1;
    [SerializeField] private bool _useArpeggiator = false;
    [SerializeField] private Arpeggiator _arpeggiator;
    [SerializeField] private bool _playMultipleTimes = false;
    [SerializeField] private int _numberOfPlays = 3;
    [SerializeField] private float _delayBetweenPlays = 0.1f;

    private CancellationTokenSource _fadeCts;
    private ConfigurableIterator<AudioClip> _audioClipsIterator;
    private bool _isFirstTimePlayed = true;


    public void Play()
    {
        if (!ValidateAudioClipsAreAvailable()) return;

        _lastAudioSource = CreateAudioGameObject();
        PlayWithAudioSource(_lastAudioSource, true);
    }

    private bool ValidateAudioClipsAreAvailable()
    {
        bool audioClipsAreAvailable = _audioClips.Length > 0;
        if (audioClipsAreAvailable) Debug.LogWarning("No audio clips found on " + name + " audio player!");
        return audioClipsAreAvailable;
    }

    public void PlayWithAudioSource(AudioSource audioSource, bool destroyAudioWhenFinishedPlaying = false)
    {
        if (!ValidateAudioClipsAreAvailable()) return;

        if (_isFirstTimePlayed) Reset();

        if (_playMultipleTimes)
        {
            PlayMultipleTimes(_numberOfPlays, _delayBetweenPlays).Forget();
            return;
        }

        //TODO: Add option to use same game object and not create a new one(to stop audio when new one is played)
        _lastAudioSource = audioSource;
        PlayAudio(_lastAudioSource, destroyAudioWhenFinishedPlaying);
    }

    public void Reset()
    {
        if (_audioClips.Length > 1) _audioClipsIterator = new ConfigurableIterator<AudioClip>(_audioClips, _playOrder);
        _arpeggiator.Reset();
        _isFirstTimePlayed = false;
    }

    private async UniTask PlayMultipleTimes(int numberOfPlays, float delayBetweenPlays)
    {
        for (int i = 0; i < numberOfPlays; i++)
        {
            _lastAudioSource = CreateAudioGameObject();
            PlayAudio(_lastAudioSource, true);
            await UniTask.Delay(TimeSpan.FromSeconds(delayBetweenPlays));
        }
    }


    private AudioSource CreateAudioGameObject()
    {
        GameObject createdSound = new(name, typeof(AudioSource));
        createdSound.transform.parent = AudioManager.Instance == null ? null : AudioManager.Instance.transform;
        return createdSound.GetComponent<AudioSource>();
    }


    private void SetAudioSourceParameters(AudioSource audioSource)
    {
        audioSource.clip = GetNextAudioClip();
        audioSource.volume = GetVolume();
        audioSource.pitch = GetPitch();
        audioSource.loop = _loop;
    }

    private AudioClip GetNextAudioClip()
    {
        AudioClip nextAudioClip = _audioClips.Length > 1 ? _audioClipsIterator.Next() : _audioClips[0];
        return nextAudioClip;
    }

    private float GetVolume()
    {
        return _dynamicVolumeRange ? Random.Range(_volumeRange.x, _volumeRange.y) : _volume;
    }

    private float GetPitch()
    {
        float basePitch = _dynamicPitchRange ? Random.Range(_pitchRange.x, _pitchRange.y) : _pitch;

        if (!_useArpeggiator) return basePitch;

        return _arpeggiator.GetNextScaleNotePitch() * basePitch;
    }


    //Fade and play multiple times don't work together
    //Fix fade duration when starting from middle volume
    private async UniTask Fade(AudioSource audioSource, float startingVolume, float targetVolume)
    {
        _fadeCts?.Cancel();
        _fadeCts = new CancellationTokenSource();

        float passedTime = 0;
        float startTime = GetCurrentTime();

        while (passedTime < _fadeDuration)
        {
            passedTime = GetCurrentTime() - startTime;
            audioSource.volume = Mathf.Lerp(startingVolume, targetVolume, passedTime / _fadeDuration);

            await UniTask.Yield(cancellationToken: _fadeCts.Token);
        }

        audioSource.volume = targetVolume;

        if (targetVolume == 0)
        {
            audioSource.Stop();
        }
    }

    //TODO: rename
    private float GetCurrentTime()
    {
        float currentTime = Time.time;
#if UNITY_EDITOR
        currentTime = Application.isPlaying ? Time.time : (float)EditorApplication.timeSinceStartup;
#endif
        return currentTime;
    }

    //TODO: rename
    private void PlayAudio(AudioSource audioSource, bool destroyAudioWhenFinishedPlaying = true)
    {
        SetAudioSourceParameters(_lastAudioSource);
        audioSource.Play();
        if (_fadeType.ShouldFadeIn())
        {
            Fade(audioSource, 0, audioSource.volume).Forget();
        }

        if (destroyAudioWhenFinishedPlaying) DestroyWhenFinishedPlaying(audioSource).Forget();
    }

    private async UniTask DestroyWhenFinishedPlaying(AudioSource audioSource)
    {
        while (audioSource != null && audioSource.isPlaying)
        {
            await UniTask.Yield();
        }

        if (audioSource != null)
        {
            Action<GameObject> destroyAction = Application.isPlaying ? Destroy : DestroyImmediate;
            destroyAction(audioSource.gameObject);
        }
    }

    public void StopLastPlayedSound()
    {
        if (_lastAudioSource == null) return;
        if (_fadeType.ShouldFadeOut())
        {
            Fade(_lastAudioSource, _lastAudioSource.volume, 0).Forget();
            return;
        }

        _lastAudioSource.Stop();
    }
}