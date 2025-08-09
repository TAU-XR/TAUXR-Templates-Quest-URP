using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Audio/Note Scale", fileName = "Major")]
public class MusicScale : ScriptableObject
{
    [FormerlySerializedAs("NotesPitch")] [FormerlySerializedAs("RatiosStartingFromDo")] [SerializeField]
    private float[] _notesPitch = { 1.0f, 1.122f, 1.26f, 1.335f, 1.498f, 1.682f, 1.888f };

    public float GetPitch(int noteIndex, bool singleOctave = false)
    {
        if (singleOctave) noteIndex %= _notesPitch.Length;
        int pitchMultiplication = noteIndex / _notesPitch.Length + 1;
        return _notesPitch[noteIndex % _notesPitch.Length] * Mathf.Pow(2, pitchMultiplication - 1);
    }
}