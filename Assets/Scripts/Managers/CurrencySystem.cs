using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencySystem : Singleton<CurrencySystem>
{
    [SerializeField] private int startCoins;
    private string CURRENCY_SAVE_KEY = "MYGAME_CURRENCY";
    
    public int TotalCoins { get; set; }

    private void Start()
    {
        ResetCoins();
    }

    private void LoadCoins()
    {
        TotalCoins = PlayerPrefs.GetInt(CURRENCY_SAVE_KEY, startCoins);
    }

    private void ResetCoins()
    {
        PlayerPrefs.DeleteKey(CURRENCY_SAVE_KEY);
        LoadCoins();
    }
    
    public void AddCoins(int amount)
    {
        TotalCoins += amount;
        PlayerPrefs.SetInt(CURRENCY_SAVE_KEY, TotalCoins);
        PlayerPrefs.Save();
    }

    public void RemoveCoins(int amount)
    {
        if (TotalCoins >= amount)
        {
            TotalCoins -= amount;
            PlayerPrefs.SetInt(CURRENCY_SAVE_KEY, TotalCoins);
            PlayerPrefs.Save();
        }
    }

    // private void AddCoins(Enemy enemy)
    // {
    //     AddCoins(1);
    // }
    //

    private void HandleAddCoins(Kart kart, KartDebuffType kartDebuffType)
    {
        switch (kartDebuffType)
        {
            case KartDebuffType.LightStun:
                AddCoins(2);
                break;
            case KartDebuffType.HeavyStun:
                AddCoins(25);
                break;
        }
    }

    private void OnEnable()
    {
        LevelManager.OnRestartGame += ResetCoins;
        Kart.OnDebuffApplied += HandleAddCoins;
    }
    
    private void OnDisable()
    {
        LevelManager.OnRestartGame -= ResetCoins;
        Kart.OnDebuffApplied -= HandleAddCoins;
    }
}
