using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voice
{
    public Synthesizer.WaveType waveType = Synthesizer.WaveType.Sine;

    public int noteNumber;
    public float velocity;
    float frequency;

    float sampleRate;
    double increment;
    double phase;

    System.Random random;

    public Voice()
    {
        noteNumber = -1;
        velocity = 0;
        random = new System.Random();
    }

    public void NoteOn(int noteNumber, float velocity)
    {
        this.noteNumber = noteNumber;
        this.velocity = velocity;

        frequency = Synthesizer.NoteToFrequency(noteNumber);

        phase = 0;
        sampleRate = (float)AudioSettings.outputSampleRate;
    }

    public void NoteOff(int noteNumber)
    {
        if (noteNumber != this.noteNumber) return;

        // TODO ADSR placeholder
    }

    public void WriteAudioBuffer(ref float[] data, int channels)
    {
        increment = frequency * 2 * Mathf.PI / sampleRate;

        for (int i = 0; i < data.Length; i += channels)
        {
            phase += increment;

            if (phase > Mathf.PI * 2)
            {
                phase -= Mathf.PI * 2;
            }

            switch (waveType)
            {
                case Synthesizer.WaveType.Noise:
                    {
                        data[i] += (float)(random.NextDouble() * 2 - 1);
                    }
                    break;
                case Synthesizer.WaveType.Sine:
                    {
                        data[i] += Mathf.Sin((float)phase);
                    }
                    break;
                case Synthesizer.WaveType.Square:
                    {
                        if (Mathf.Sin((float)phase) >= 0)
                        {
                            data[i] += 1f;
                        } else
                        {
                            data[i] -= 1f;
                        }
                    }
                    break;
                case Synthesizer.WaveType.Triangle:
                    {
                        data[i] += Mathf.PingPong((float)phase, 1f) * 2 - 1f;
                    }
                    break;
                case Synthesizer.WaveType.Saw:
                    {
                        data[i] += Mathf.InverseLerp(Mathf.PI * 2, 0, (float)phase) * 2 - 1f;
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
