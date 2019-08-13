using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteInput : MonoBehaviour
{
    public delegate void NoteOnDelegate(int noteNumber, float velocity);
    public static event NoteOnDelegate NoteOn;

    public delegate void NoteOffDelegate(int noteNumber);
    public static event NoteOffDelegate NoteOff;

    Dictionary<KeyCode, int> virtualKeysDict;

    private void Awake()
    {
        virtualKeysDict = new Dictionary<KeyCode, int>();
        virtualKeysDict.Add(KeyCode.A, 51); // C
        virtualKeysDict.Add(KeyCode.W, 52); // C#
        virtualKeysDict.Add(KeyCode.S, 53); // D
        virtualKeysDict.Add(KeyCode.E, 54); // D#
        virtualKeysDict.Add(KeyCode.D, 55); // E
        virtualKeysDict.Add(KeyCode.F, 56); // F
        virtualKeysDict.Add(KeyCode.T, 57); // F#
        virtualKeysDict.Add(KeyCode.G, 58); // G
        virtualKeysDict.Add(KeyCode.Z, 59); // G#
        virtualKeysDict.Add(KeyCode.H, 60); // A
        virtualKeysDict.Add(KeyCode.U, 61); // A#
        virtualKeysDict.Add(KeyCode.J, 62); // B
        virtualKeysDict.Add(KeyCode.K, 63); // C

        // ...
    }

    private void Update()
    {
        foreach (var key in virtualKeysDict.Keys)
        {
            if (Input.GetKeyDown(key))
            {
                NoteOn?.Invoke(virtualKeysDict[key], 1f);
            }

            if (Input.GetKeyUp(key))
            {
                NoteOff?.Invoke(virtualKeysDict[key]);
            }
        }
    }
}
