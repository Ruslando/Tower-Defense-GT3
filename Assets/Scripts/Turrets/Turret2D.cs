using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Turret2D : MonoBehaviour
{
    [Header("Turret Settings")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private Transform _rotatePoint;

    [Header("Projectile Prefabs")]
    public GameObject greenShellPrefab;
    public GameObject redShellPrefab;
    public GameObject blueShellPrefab;

    [Header("Upgrade Levels")]
    public TurretImprovement[] turretImprovements;
    public TurretUpgrade turretUpgrade;

    protected float fireCountdown = 0f;
    private bool isEditing;

    private void Awake()
    {
        LoadTurretUpgradeData();
        LoadImprovementsData(turretUpgrade.GetCurrentTurretType());
    }

    private void LoadTurretUpgradeData()
    {
        turretUpgrade = Instantiate(Resources.LoadAll<TurretUpgrade>($"ScriptableObjects/TurretUpgrades")[0]);
        
    }

    private void LoadImprovementsData(TurretType turretType)
    {
        List<TurretImprovement> copiedScriptableObjects = new List<TurretImprovement>();
        TurretImprovement[] improvements = Resources.LoadAll<TurretImprovement>($"ScriptableObjects/TurretImprovements/{turretType}");

        foreach (TurretImprovement original in improvements)
        {
            TurretImprovement copiedScriptableObject = Instantiate(original);
            copiedScriptableObjects.Add(copiedScriptableObject);
        }

        turretImprovements = copiedScriptableObjects.ToArray();
    }

    protected virtual void Update()
    {
        CheckEditingCancel();
        RotateTowardsMouse();

        if(!LevelManager.Instance.IsGameOver())
        {
            if (fireCountdown > 0)
            {
                fireCountdown -= Time.deltaTime;
            }

            if(!isEditing) {
                if (fireCountdown <= 0)
                {
                    switch (turretUpgrade.currentLevel)
                    {
                        case 0:
                            FireGreenShell();
                            break;
                        case 1:
                            FireRedShell();
                            break;
                        case 2:
                            FireBlueShell();
                            break;
                    }

                    fireCountdown = 1f / GetUpgradeValue(TurretImprovementType.FireRate);
                }
            }
        }
        
    }

    public void ImproveTurret()
    {
        if(!turretImprovements[0].IsMaxLevel() && turretImprovements[0].CanBuyNextLevel())
        {
            CurrencySystem.Instance.RemoveCoins(turretImprovements[0].GetNextLevelCost());
            foreach (var improvementData in turretImprovements)
            {
                improvementData.ApplyImprovement();
            }
        }
    }

    public void UpgradeTurret()
    {
        turretUpgrade.ApplyUpgrade();
        LoadImprovementsData(turretUpgrade.GetCurrentTurretType());
    }

    protected void FireGreenShell()
    {
        GameObject shellInstance = Instantiate(greenShellPrefab, firePoint.position, Quaternion.identity);
        Projectile2D shell = shellInstance.GetComponent<Projectile2D>();

        if (shell != null)
        {
            shell.Initialize(this);
        }
    }

    protected void FireRedShell()
    {
        GameObject shellInstance = Instantiate(redShellPrefab, firePoint.position, Quaternion.identity);
        Projectile2D shell = shellInstance.GetComponent<Projectile2D>();

        if (shell != null)
        {
            shell.Initialize(this);
        }
    }

    protected void FireBlueShell()
    {
        GameObject shellInstance = Instantiate(blueShellPrefab, firePoint.position, Quaternion.identity);
        Projectile2D shell = shellInstance.GetComponent<Projectile2D>();

        if (shell != null)
        {
            shell.Initialize(this);
        }
    }

    public float GetUpgradeValue(TurretImprovementType turretUpgradeType)
    {
        TurretImprovement upgradeData = turretImprovements.FirstOrDefault(upgrade => upgrade.upgradeType == turretUpgradeType);
        if(upgradeData != null)
        {
            return upgradeData.GetCurrentLevelValue();
        }
        
        return 0f;
    }

    private void CheckEditingCancel()
    {
        if (isEditing && Input.GetMouseButtonDown(0))
        {
            SetIsEditing(false);
        }
    }

    private void RotateTowardsMouse()
    {
        if(isEditing)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = mousePosition - (Vector2)transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg -90f;
            _rotatePoint.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    public void SetIsEditing(bool editing)
    {
        if(turretUpgrade.IsEditable())
        {
            isEditing = editing;
        }
    }

    public Transform GetRotatePoint()
    {
        return _rotatePoint;
    }

    public int GetResellValue()
    {
        // resell value = half of upgrade cost + half of improvement cost
        return turretUpgrade.GetCurrentLevelCost() / 2 + turretImprovements[0].GetCurrentLevelCost() / 2;
    }
}
