using Melanchall.DryWetMidi.Interaction;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Lane : MonoBehaviour
{
    public Melanchall.DryWetMidi.MusicTheory.NoteName noteRestriction;
    public KeyCode input;
    public GameObject notePrefab;

    private List<Note> notes = new List<Note>();
    public List<double> timeStamps = new List<double>();

    private int spawnIndex = 0;
    private int inputIndex = 0;

    private bool isInitialized = false;

    public void SetTimeStamps(Melanchall.DryWetMidi.Interaction.Note[] array)
    {
        timeStamps.Clear();

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

        isInitialized = true;
    }

    void Update()
    {
        if (!isInitialized) return;
        if (SongManager.Instance == null) return;

        SpawnNotes();
        HandleInput();
    }

    void SpawnNotes()
    {
        if (spawnIndex >= timeStamps.Count) return;
        if (notePrefab == null)
        {
            Debug.LogError("Note Prefab is not assigned in Lane!");
            return;
        }

        double songTime = SongManager.GetAudioSourceTime();

        if (songTime >= timeStamps[spawnIndex] - SongManager.Instance.noteTime)
        {
            var spawned = Instantiate(notePrefab, transform);

            Note noteComponent = spawned.GetComponent<Note>();
            if (noteComponent == null)
            {
                Debug.LogError("Note prefab is missing Note script!");
                return;
            }

            noteComponent.assignedTime = (float)timeStamps[spawnIndex];

            notes.Add(noteComponent);
            spawnIndex++;
        }
    }

    void HandleInput()
    {
        if (inputIndex >= timeStamps.Count) return;
        if (SongManager.Instance == null) return;

        double timeStamp = timeStamps[inputIndex];
        double marginOfError = SongManager.Instance.marginOfError;

        double audioTime =
            SongManager.GetAudioSourceTime() -
            (SongManager.Instance.inputDelayInMilliseconds / 1000.0);

        // Strum detection
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (Input.GetKey(input))
            {
                if (Math.Abs(audioTime - timeStamp) < marginOfError)
                {
                    Hit();

                    if (inputIndex < notes.Count && notes[inputIndex] != null)
                    {
                        Destroy(notes[inputIndex].gameObject);
                    }

                    inputIndex++;
                }
                else
                {
                    Miss();
                }
            }
        }

        // Passive miss
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