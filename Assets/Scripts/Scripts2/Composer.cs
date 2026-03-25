using UnityEngine;
using System;
using System.Collections.Generic;

public class Composer : MonoBehaviour
{
    [Header("Chart")]
    public List<ChartNote> chart = new List<ChartNote>();

    [Header("References")]
    public Metronome metronome;
    public Judge judge;

    private int currentIndex = 0;

    public event Action<ChartNote> OnNextExpected;
    public event Action<ChartNote> OnCorrect;
    public event Action<ChartNote> OnWrong;

    void Start()
    {
        if (judge != null)
        {
            judge.OnSuccess += HandleSuccess;
            judge.OnFailure += HandleFailure;
        }

        EmitNext();
    }

    void OnDestroy()
    {
        if (judge != null)
        {
            judge.OnSuccess -= HandleSuccess;
            judge.OnFailure -= HandleFailure;
        }
    }

    void EmitNext()
    {
        if (currentIndex >= chart.Count) return;

        OnNextExpected?.Invoke(chart[currentIndex]);
    }

    void HandleSuccess(int beat, float offset)
    {
        if (currentIndex >= chart.Count) return;

        ChartNote expected = chart[currentIndex];

        // Check beat match
        if (beat == expected.beat)
        {
            // NOTE: Button check handled separately (see below)
            OnCorrect?.Invoke(expected);
            currentIndex++;
            EmitNext();
        }
        else
        {
            OnWrong?.Invoke(expected);
        }
    }

    void HandleFailure(int beat, float offset)
    {
        if (currentIndex >= chart.Count) return;

        ChartNote expected = chart[currentIndex];

        if (beat == expected.beat)
        {
            OnWrong?.Invoke(expected);
            currentIndex++;
            EmitNext();
        }
    }

    // Called externally by your input system
    public void RegisterPlayerInput(string button, int beat)
    {
        if (currentIndex >= chart.Count) return;

        ChartNote expected = chart[currentIndex];

        if (expected.beat == beat && expected.button == button)
        {
            OnCorrect?.Invoke(expected);
            currentIndex++;
            EmitNext();
        }
        else
        {
            OnWrong?.Invoke(expected);
        }
    }
}