using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Synthesizer : MonoBehaviour
{
    const int polyphony = 16;
    Voice[] voicesPool;
    List<Voice> activeVoices;
    Stack<Voice> freeVoices;
    Dictionary<int, Voice> noteDict;

    private float tick = 0;

    float sampleRate;
    double increment;
    double phase;

    [Header("Audio Controls")]
    [SerializeField, Range(0, 1)] float volume = 0.5f;
    [SerializeField] float frequency = 440f;

    public enum WaveType
    {
        Noise,
        Sine,
        Square,
        Triangle,
        Saw
    }

    [SerializeField] private WaveType waveType;


    #region Unity Messages

    private void OnEnable()
    {
        sampleRate = (float)AudioSettings.outputSampleRate;
        Debug.LogFormat("Output Sample Rate = {0}", sampleRate);

        NoteInput.NoteOn += NoteInput_NoteOn;
        NoteInput.NoteOff += NoteInput_NoteOff;

        voicesPool = new Voice[polyphony];
        freeVoices = new Stack<Voice>();
        for (int i = 0; i < voicesPool.Length; i++)
        {
            voicesPool[i] = new Voice();
            freeVoices.Push(voicesPool[i]);
        }

        activeVoices = new List<Voice>();
        noteDict = new Dictionary<int, Voice>();
    }

    private void OnDisable()
    {
        NoteInput.NoteOn -= NoteInput_NoteOn;
        NoteInput.NoteOff -= NoteInput_NoteOff;
    }

    private void OnAudioFilterRead(float[] data, int channels)
    {
        foreach (var voice in activeVoices)
        {
            voice.waveType = waveType;
            voice.WriteAudioBuffer(ref data, channels);
        }

        for (int i = 0; i < data.Length; i++)
        {
            data[i] *= volume;
        }
    }

    #endregion


    #region Event Handler

    private void NoteInput_NoteOn(int noteNumber, float velocity)
    {
        Debug.LogFormat("NoteOn: {0}", noteNumber);

        if (noteDict.ContainsKey(noteNumber)) return;
        if (freeVoices.Count <= 0) return;

        Voice voice = freeVoices.Pop();
        voice.NoteOn(noteNumber, velocity);
        activeVoices.Add(voice);
        noteDict.Add(noteNumber, voice);
    }

    private void NoteInput_NoteOff(int noteNumber)
    {
        Debug.LogFormat("NoteOff: {0}", noteNumber);

        if (!noteDict.ContainsKey(noteNumber)) return;

        Voice voice = noteDict[noteNumber];
        voice.NoteOff(noteNumber);
        activeVoices.Remove(voice);
        freeVoices.Push(voice);
        noteDict.Remove(noteNumber);
    }

    #endregion

    // A4 = 440Hz = noteNumber 60
    public static float NoteToFrequency(int noteNumber)
    {
        float twelfthRoot = Mathf.Pow(2f, (1f / 12f));
        int fixedNoteNumber = 60;
        return 440f * Mathf.Pow(twelfthRoot, (noteNumber - fixedNoteNumber));
    }
}
