using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(LineRenderer))]
public class WaveVisualizer : MonoBehaviour
{
    LineRenderer lineRenderer;
    List<Vector3> linePositions;

    float[] audioBuffer;
    int audioChannels;

    [Header("Settings")]
    [SerializeField, Range(0, 1)] float bufferScale = .5f;
    [SerializeField] float amplitudeMultiplier = 1f;
    [SerializeField] Bounds bounds;
    
    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        linePositions = new List<Vector3>();
    }

    // Update is called once per frame
    void Update()
    {
        if (audioBuffer == null) return;

        linePositions.Clear();
        for (int i = 0; i < audioBuffer.Length * bufferScale; i += audioChannels)
        {
            linePositions.Add(new Vector3(
                Mathf.Lerp(bounds.center.x - bounds.extents.x, bounds.center.x + bounds.extents.x, (float)i/(audioBuffer.Length * bufferScale )),
                Mathf.Lerp(bounds.center.y - bounds.extents.y, bounds.center.y + bounds.extents.y, ((audioBuffer[i]+1)/2f * amplitudeMultiplier) ),
                0
                ));
        }

        lineRenderer.positionCount = linePositions.Count;
        lineRenderer.SetPositions(linePositions.ToArray());
    }

    private void OnAudioFilterRead(float[] data, int channels)
    {
        audioBuffer = data;
        audioChannels = channels;
    }
}
