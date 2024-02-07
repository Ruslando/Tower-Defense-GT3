﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    [SerializeField] private int lives = 10;

    private bool _gameOver = true;
    public int TotalLives { get; set; }
    public int CurrentWave { get; set; }
    public float currentTime = 0f;

    public static event Action OnRestartGame;
    public static event Action OnStartGame;
    
    private void OnEnable()
    {
        Kart.OnAllLapsCompleted += GameOver;
    }

    private void Update()
    {
        UpdateTime();
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
        ResetTime();
        StartCoroutine(WaitAndInvokeStartGame());
    }

    IEnumerator WaitAndInvokeStartGame()
    {
        // Wait for 5 seconds
        yield return new WaitForSeconds(5f);

        _gameOver = false;

        // Invoke OnStartGame event
        OnStartGame?.Invoke();
    }

    private void UpdateTime()
    {
        if(!_gameOver) {
            currentTime += Time.deltaTime;
        }
    }

    private void ResetTime()
    {
        currentTime = 0f;
    }

    public float GetCurrentTime()
    {
        return currentTime;
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
