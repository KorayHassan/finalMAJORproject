using Melanchall.DryWetMidi.Interaction;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Lane : MonoBehaviour
{
    public Melanchall.DryWetMidi.MusicTheory.NoteName noteRestriction;
    public KeyCode input; // Numpad key for this lane
    public GameObject notePrefab;

    private List<Note> notes = new List<Note>();
    public List<double> timeStamps = new List<double>();

    private int spawnIndex = 0;
    private int inputIndex = 0;

    public void SetTimeStamps(Melanchall.DryWetMidi.Interaction.Note[] array)
    {
        foreach (var note in array)
        {
            if (note.NoteName == noteRestriction)
            {
                var metricTimeSpan = TimeConverter.ConvertTo<MetricTimeSpan>(
                    note.Time,
                    SongManager.midiFile.GetTempoMap()
                );

                double time =
                    (double)metricTimeSpan.Minutes * 60f +
                    metricTimeSpan.Seconds +
                    (double)metricTimeSpan.Milliseconds / 1000f;

                timeStamps.Add(time);
            }
        }
    }

    void Update()
    {
        SpawnNotes();
        HandleInput();
    }

    void SpawnNotes()
    {
        if (spawnIndex >= timeStamps.Count) return;

        if (SongManager.GetAudioSourceTime() >= 
            timeStamps[spawnIndex] - SongManager.Instance.noteTime)
        {
            var spawned = Instantiate(notePrefab, transform);
            Note noteComponent = spawned.GetComponent<Note>();

            noteComponent.assignedTime = (float)timeStamps[spawnIndex];

            notes.Add(noteComponent);
            spawnIndex++;
        }
    }

    void HandleInput()
    {
        if (inputIndex >= timeStamps.Count) return;

        double timeStamp = timeStamps[inputIndex];
        double marginOfError = SongManager.Instance.marginOfError;
        double audioTime =
            SongManager.GetAudioSourceTime() -
            (SongManager.Instance.inputDelayInMilliseconds / 1000.0);

        // ✅ Strum detection
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Only hit if fret is being HELD
            if (Input.GetKey(input))
            {
                if (Math.Abs(audioTime - timeStamp) < marginOfError)
                {
                    Hit();

                    if (inputIndex < notes.Count)
                        Destroy(notes[inputIndex].gameObject);

                    inputIndex++;
                }
                else
                {
                    // Strummed at wrong time
                    Miss();
                }
            }
        }

        // Passive miss (note passed completely)
        if (audioTime > timeStamp + marginOfError)
        {
            Miss();
            inputIndex++;
        }
    }

    void Hit()
    {
        ScoreManager.Hit();
        Debug.Log($"Hit note {inputIndex}");
    }

    void Miss()
    {
        ScoreManager.Miss();
        Debug.Log($"Missed note {inputIndex}");
    }
}