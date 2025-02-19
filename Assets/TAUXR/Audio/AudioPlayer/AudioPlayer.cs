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

    //Consider arpeggiator structure - maybe both the order of notes and how many times to play
    [SerializeField] private Arpeggiator _arpeggiator;
    [SerializeField] private bool _playMultipleTimes = false;
    [SerializeField] private int _numberOfPlays = 3;
    [SerializeField] private float _delayBetweenPlays = 0.1f;

    [SerializeField] private AudioSource _lastAudioSource;
    [SerializeField] private bool _inPreviewMode;
    private CancellationTokenSource _fadeCts;
    private ConfigurableIterator<AudioClip> _audioClipsIterator;
    private bool _isFirstTimePlayed = true;
    private AudioSource _previewer;

    public void Play()
    {
        if (!ValidateAudioClipsAreAvailable()) return;

        if (!_inPreviewMode) _lastAudioSource = CreateAudioGameObject();
        if (_isFirstTimePlayed) Reset();

        if (_playMultipleTimes)
        {
            PlayMultipleTimes(_numberOfPlays, _delayBetweenPlays).Forget();
            return;
        }

        PlayAudio(_lastAudioSource);
    }

    private bool ValidateAudioClipsAreAvailable()
    {
        bool audioClipsAreAvailable = _audioClips.Length > 0;
        if (!audioClipsAreAvailable) Debug.LogWarning("No audio clips found on " + name + " audio player!");
        return audioClipsAreAvailable;
    }

    public void Reset()
    {
        if (_audioClips != null && _audioClips.Length > 1)
            _audioClipsIterator = new ConfigurableIterator<AudioClip>(_audioClips, _playOrder);
        if (_useArpeggiator) _arpeggiator.Reset();
        _isFirstTimePlayed = false;
    }

    private async UniTask PlayMultipleTimes(int numberOfPlays, float delayBetweenPlays)
    {
        PlayAudio(_lastAudioSource);
        for (int i = 1; i < numberOfPlays; i++)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(delayBetweenPlays));
            _lastAudioSource = CreateAudioGameObject();
            PlayAudio(_lastAudioSource);
        }
    }


    private AudioSource CreateAudioGameObject()
    {
        GameObject createdSound = new(name, typeof(AudioSource));
        // createdSound.transform.parent = AudioManager.Instance == null ? null : AudioManager.Instance.transform;
        createdSound.transform.parent = null;
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
    private void PlayAudio(AudioSource audioSource)
    {
        SetAudioSourceParameters(_lastAudioSource);
        audioSource.Play();
        if (_fadeType.ShouldFadeIn())
        {
            Fade(audioSource, 0, audioSource.volume).Forget();
        }

        if (!_inPreviewMode) DestroyWhenFinishedPlaying(audioSource).Forget();
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

#if UNITY_EDITOR
    public void PlayPreview(AudioSource previewer)
    {
        _lastAudioSource = previewer;
        _inPreviewMode = true;
        Play();
        _inPreviewMode = false;
    }
#endif
}