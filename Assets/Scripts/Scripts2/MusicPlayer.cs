using UnityEngine;
using System;

[RequireComponent(typeof(AudioSource))]
public class MusicPlayer : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource;

    [Header("Track")]
    public AudioClip currentTrack;

    [Header("Debug")]
    public double timePositionMs;

    public event Action OnTrackStarted;
    public event Action OnTrackFinished;

    private double dspStartTime;
    private bool isPlaying = false;

    void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!isPlaying) return;

        double dspNow = AudioSettings.dspTime;
        timePositionMs = (dspNow - dspStartTime) * 1000.0;

        // Track end detection
        if (!audioSource.isPlaying && isPlaying)
        {
            isPlaying = false;
            OnTrackFinished?.Invoke();
        }
    }

    // ▶ Play a track
    public void Play(AudioClip clip)
    {
        currentTrack = clip;

        audioSource.clip = clip;

        dspStartTime = AudioSettings.dspTime + 0.1; // small delay for accuracy
        audioSource.PlayScheduled(dspStartTime);

        isPlaying = true;

        OnTrackStarted?.Invoke();
    }

    // ⏹ Stop playback
    public void Stop()
    {
        audioSource.Stop();
        isPlaying = false;
    }

    // 🔁 Switch track
    public void SwitchTrack(AudioClip newTrack)
    {
        Stop();
        Play(newTrack);
    }

    // ⏱ Get time in seconds
    public double GetTimeSeconds()
    {
        return timePositionMs / 1000.0;
    }

    // ⏱ Get time in ms
    public double GetTimeMs()
    {
        return timePositionMs;
    }
}