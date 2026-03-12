using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameState State;
    
    public static event Action<GameState> OnGameStateChanged;
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        UpdateGameState(GameState.GameStart);
    }
    
    public void UpdateGameState(GameState state)
    {
        State = state;

        switch (State)
        {
            case GameState.Defeat:
                break;
            case GameState.GameStart:
                break;
            case GameState.Victory:
                break;
            case GameState.Difficulty:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
            
        }
        
        OnGameStateChanged?.Invoke(state);
    }
    
    void HandleDifficulty(GameState state)
    {
        
    }
}

public enum GameState
{
    Victory,
    Defeat,
    GameStart,
    Difficulty,
    Tutorial,
    HoldUpright,
    NumpadPressed,
    SpacePressed,
    
}