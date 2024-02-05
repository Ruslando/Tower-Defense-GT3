using UnityEngine;
using System;
using System.Collections.Generic;


public enum TurretUpgradeType
{
    ProjectileSpeed,
    FireRate,
    StunTime,
    TargetingRadius,
    BlastRadius
}

[CreateAssetMenu(fileName = "New Turret Upgrade", menuName = "Turret Upgrade")]
public class TurretUpgradeData : ScriptableObject
{
    [Serializable]
    public struct UpgradeLevel
    {
        public int cost;
        public float value;
    }

    public TurretUpgradeType upgradeType;
    public int currentLevel = 0; // Current upgrade level
    public UpgradeLevel CurrentUpgradeLevel => upgradeLevels[currentLevel];
    public List<UpgradeLevel> upgradeLevels = new List<UpgradeLevel>();
}
