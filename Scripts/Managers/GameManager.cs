using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private GameStates gameState;
    public GameStates currentState { get { return gameState; } }

    public static event Action<GameStates> OnGameStateChanged;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
        DontDestroyOnLoad(Instance);
    }

    public enum GameStates
    {
        StartMenu,
        Game,
        End_Score,
    }

    public void UpdateGameState(GameStates newState)
    {
        gameState = newState;

        switch (gameState)
        {
            case GameStates.StartMenu:
                break;
            case GameStates.Game:
                break;
            case GameStates.End_Score:
                break;
            default:
                break;
        }

        OnGameStateChanged?.Invoke(gameState);
    }

    public void StartGame()
    {
        UpdateGameState(GameStates.Game);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
