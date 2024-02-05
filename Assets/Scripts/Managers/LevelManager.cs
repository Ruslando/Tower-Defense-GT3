using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    [SerializeField] private int lives = 10;

    private bool _gameOver;
    public int TotalLives { get; set; }
    public int CurrentWave { get; set; }

    public static event Action OnRestartGame;
    
    private void OnEnable()
    {
        Kart.OnAllLapsCompleted += GameOver;
    }

    private void Start()
    {
        TotalLives = lives;
        CurrentWave = 1;
    }

    private void GameOver(Kart kart)
    {
        if(!_gameOver)
        {
            _gameOver = true;
            UIManager.Instance.ShowGameOverPanel();
        }
    }

    public void RestartGame()
    {
        OnRestartGame?.Invoke();
    }
    
    // private void WaveCompleted()
    // {
    //     CurrentWave++;
    //     AchievementManager.Instance.AddProgress("Waves10", 1);
    //     AchievementManager.Instance.AddProgress("Waves20", 1);
    //     AchievementManager.Instance.AddProgress("Waves50", 1);
    //     AchievementManager.Instance.AddProgress("Waves100", 1);
    // }
    
    // private void OnEnable()
    // {
    //     Enemy.OnEndReached += ReduceLives;
    //     // KartManager.OnWaveCompleted += WaveCompleted;
    // }
    //
    // private void OnDisable()
    // {
    //     Enemy.OnEndReached -= ReduceLives;
    //     // KartManager.OnWaveCompleted -= WaveCompleted;
    // }
}
