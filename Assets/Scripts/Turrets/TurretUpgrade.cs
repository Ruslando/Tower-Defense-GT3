using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TurretType
{
    GreenShellTurret,
    RedShellTurret,
    BlueShellTurret
}

[CreateAssetMenu(fileName = "New Turret Upgrade", menuName = "Turret Upgrade")]
public class TurretUpgrade : ScriptableObject
{
    [Serializable]
    public struct UpgradeLevel
    {
        public int cost;
        public TurretType turretType;
        public bool isEditable;
    }
    
    public List<UpgradeLevel> upgradeLevels = new List<UpgradeLevel>();
    public int currentLevel = 0; // Current upgrade level
    public UpgradeLevel CurrentUpgrade => upgradeLevels[currentLevel];

    public void ApplyUpgrade()
    {
        if(currentLevel < upgradeLevels.Count - 1)
        {
            currentLevel++;
        }   
    }

    public int GetCurrentLevelCost()
    {
        return CurrentUpgrade.cost;
    }

    public TurretType GetCurrentTurretType()
    {
        return CurrentUpgrade.turretType;
    }

    public bool IsEditable()
    {
        return CurrentUpgrade.isEditable;
    }

    public int GetNextLevelCost()
    {
        if(currentLevel + 1 < upgradeLevels.Count)
        {
            return upgradeLevels[currentLevel + 1].cost;
        }
        return -1;
    }

    public bool IsMaxLevel()
    {
        return currentLevel == upgradeLevels.Count - 1;
    }

    public bool CanBuyNextLevel()
    {
        return CurrencySystem.Instance.TotalCoins >= GetNextLevelCost();
    }
    
}
