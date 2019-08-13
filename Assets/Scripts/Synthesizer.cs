using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Synthesizer : MonoBehaviour
{
    private System.Random random = new System.Random();

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
        Sine
    }

    [SerializeField] private WaveType waveType;


    #region Unity Messages

    private void OnEnable()
    {
        sampleRate = (float)AudioSettings.outputSampleRate;
        Debug.LogFormat("Output Sample Rate = {0}", sampleRate);

        NoteInput.NoteOn += NoteInput_NoteOn;
        NoteInput.NoteOff += NoteInput_NoteOff;
    }

    private void OnDisable()
    {
        NoteInput.NoteOn -= NoteInput_NoteOn;
        NoteInput.NoteOff -= NoteInput_NoteOff;
    }

    private void OnAudioFilterRead(float[] data, int channels)
    {
        increment = frequency * 2 * Mathf.PI / sampleRate;

        // clear audio buffer
        for (int i = 0; i < data.Length; i++)
        {
            data[i] = 0;
        }

        for (int i = 0; i < data.Length; i++)
        {
            phase += increment;

            switch (waveType)
            {
                case WaveType.Noise:
                    {
                        // generate white noise
                        data[i] = (float)(random.NextDouble() * 2 - 1) * volume;
                    }
                    break;
                case WaveType.Sine:
                    data[i] = Mathf.Sin((float)phase) * volume;
                    break;
                default:
                    break;
            }
        }
    }

    #endregion


    #region Event Handler

    private void NoteInput_NoteOn(int noteNumber, float velocity)
    {
        Debug.LogFormat("NoteOn: {0}", noteNumber);
        frequency = NoteToFrequency(noteNumber);
    }

    private void NoteInput_NoteOff(int noteNumber)
    {
        Debug.LogFormat("NoteOff: {0}", noteNumber);
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
