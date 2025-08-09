using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Arpeggiator
{
    [SerializeField] private MusicScale _musicScale;

    [SerializeField] private bool _isSingleOctave;

    [SerializeField] private float _timeBetweenResets = 0.5f;

    private int _noteIndex;
    private float _lastPitchIncreaseTime = 0;

    //Currently only works with 0
    private int _startingNoteIndex = 0;

    public void Reset()
    {
        _noteIndex = _startingNoteIndex;
    }

    public float GetNextScaleNotePitch()
    {
        if (_lastPitchIncreaseTime < Time.time - _timeBetweenResets)
        {
            _noteIndex = _startingNoteIndex;
        }

        float nextScaleNotePitch = _musicScale.GetPitch(_noteIndex, _isSingleOctave);

        _noteIndex++;
        _lastPitchIncreaseTime = Time.time;

        return nextScaleNotePitch;
    }
}