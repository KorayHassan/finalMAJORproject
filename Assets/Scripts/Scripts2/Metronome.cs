using UnityEngine;
using System;

public class Metronome : MonoBehaviour
{
    [Header("Timing")]
    public float bpm = 120f;
    public float margin = 0.05f; // seconds window around beat

    [Header("Mapping")]
    public Transform targetTransform;
    public Vector3 startPosition;
    public Vector3 endPosition;

    [Header("Debug")]
    public float currentBeatFloat;
    public int lastBeat = 0;

    private float beatDuration;
    private float nextBeatTime;
    private float songStartTime;

    public event Action<int> OnBeat;

    void Start()
    {
        beatDuration = 60f / bpm; // seconds per beat
        songStartTime = Time.time;

        lastBeat = 0;
        nextBeatTime = songStartTime + beatDuration;
    }

    void Update()
    {
        float position = GetMusicPosition();

        // Calculate floating beat position (e.g. 3.25 = 25% into 4th beat)
        currentBeatFloat = (position - songStartTime) / beatDuration;

        // Beat trigger
        if (position >= nextBeatTime)
        {
            lastBeat++;
            OnBeat?.Invoke(lastBeat);

            nextBeatTime += beatDuration;
        }

        // Active beat window
        float activeBeatStart = nextBeatTime - margin;
        float activeBeatEnd = nextBeatTime + margin;

        // Map beat progress to transform
        float beatProgress = Mathf.Repeat(currentBeatFloat, 1f);
        UpdateTransform(beatProgress);
    }

    float GetMusicPosition()
    {
        // Replace with your audio system if needed
        return Time.time;
    }

    void UpdateTransform(float t)
    {
        if (targetTransform == null) return;

        targetTransform.position = Vector3.Lerp(startPosition, endPosition, t);
    }

    // Optional helper
    public bool IsInsideActiveBeatWindow()
    {
        float position = GetMusicPosition();
        return position >= (nextBeatTime - margin) &&
               position <= (nextBeatTime + margin);
    }
}