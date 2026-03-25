using UnityEngine;
using System;


[Serializable]
public class ChartNote
{
    public int beat;
    public string button;
}
public class Judge : MonoBehaviour
{
    [Header("References")]
    public Metronome metronome;

    [Header("Settings")]
    public float perfectWindow = 0.05f; // seconds
    public float goodWindow = 0.1f;

    public event Action<int, float> OnSuccess; // beat, accuracy offset
    public event Action<int, float> OnFailure; // beat, accuracy offset

    private bool inputReceivedThisBeat = false;
    private int lastJudgedBeat = -1;

    void Awake()
    {
        if (metronome != null)
            metronome.OnBeat += OnBeat;
    }

    void OnDestroy()
    {
        if (metronome != null)
            metronome.OnBeat -= OnBeat;
    }

    void Update()
    {
        // Example input (replace with your input system)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            HandleInput("space");
        }
    }

    void HandleInput(string button)
    {
        float position = GetMusicPosition();
        float beatTime = GetCurrentBeatTime();

        float offset = position - beatTime; // negative = early, positive = late
        float absOffset = Mathf.Abs(offset);

        int beatIndex = metronome.lastBeat;

        // Prevent double judging same beat
        if (beatIndex == lastJudgedBeat)
            return;

        if (absOffset <= goodWindow)
        {
            OnSuccess?.Invoke(beatIndex, offset);
            inputReceivedThisBeat = true;
            lastJudgedBeat = beatIndex;
        }
        else
        {
            OnFailure?.Invoke(beatIndex, offset);
            lastJudgedBeat = beatIndex;
        }
    }

    void OnBeat(int beat)
    {
        // If player didn't press anything during last beat → fail
        if (!inputReceivedThisBeat && beat > 0)
        {
            OnFailure?.Invoke(beat - 1, float.MaxValue);
        }

        inputReceivedThisBeat = false;
    }

    float GetMusicPosition()
    {
        return Time.time;
    }

    float GetCurrentBeatTime()
    {
        float beatDuration = 60f / metronome.bpm;
        float songStart = GetSongStartTime();

        int beatIndex = metronome.lastBeat;
        return songStart + (beatIndex * beatDuration);
    }

    float GetSongStartTime()
    {
        // Assumes metronome started at same time
        return Time.time - (metronome.currentBeatFloat * (60f / metronome.bpm));
    }
}