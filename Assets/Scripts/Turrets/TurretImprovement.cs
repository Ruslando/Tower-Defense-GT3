using UnityEngine;
using System;
using System.Collections.Generic;


public enum TurretImprovementType
{
    ProjectileSpeed,
    FireRate,
    StunTime,
    TargetingRadius,
    BlastRadius
}

[CreateAssetMenu(fileName = "New Turret Improvement", menuName = "Turret Improvement")]
public class TurretImprovement : ScriptableObject
{
    [Serializable]
    public struct ImprovementLevel
    {
        public int cost;
        public float value;
    }

    public TurretImprovementType upgradeType;
    public int currentLevel = 0;
    public ImprovementLevel CurrentImprovementLevel => improvementLevels[currentLevel];
    public List<ImprovementLevel> improvementLevels = new List<ImprovementLevel>();

    public void ApplyImprovement()
    {
        if(currentLevel < improvementLevels.Count - 1) 
        {
            currentLevel++;
        }
    }

    public int GetCurrentLevelCost()
    {
        return CurrentImprovementLevel.cost;
    }

    public int GetNextLevelCost()
    {
        if(currentLevel + 1 < improvementLevels.Count)
        {
            return improvementLevels[currentLevel + 1].cost;
        }
        return -1;
    }

    public float GetCurrentLevelValue()
    {
        return CurrentImprovementLevel.value;
    }

    public float GetNextLevelValue()
    {
        if(currentLevel + 1 < improvementLevels.Count)
        {
            return improvementLevels[currentLevel].value;
        }
        return -1;
    }

    public bool IsMaxLevel()
    {
        return currentLevel == improvementLevels.Count - 1;
    }

    public bool CanBuyNextLevel()
    {
        return CurrencySystem.Instance.TotalCoins >= GetNextLevelCost();
    }
}
