using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Turret2D : MonoBehaviour
{
    [Header("Turret Settings")]
    [SerializeField] private Transform firePoint; // Point where projectiles are spawned.
    [SerializeField] private Transform _rotatePoint;

    [Header("Projectile Prefabs")]
    public GameObject greenShellPrefab; // Prefab of the green shell.
    public GameObject redShellPrefab; // Prefab of the red shell.
    public GameObject blueShellPrefab; // Prefab of the blue shell.

    [Header("Upgrade Levels")]
    public TurretImprovement[] turretImprovements;
    public TurretUpgrade turretUpgrade;

    protected float fireCountdown = 0f; // Countdown to next shot.
    private bool isEditing;

    private void Awake()
    {
        LoadTurretUpgradeData();
        LoadImprovementsData(turretUpgrade.GetCurrentTurretType());
    }

    private void Start()
    {
        
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

        if (fireCountdown > 0)
        {
            fireCountdown -= Time.deltaTime;
        }

        if(!isEditing) {
            // Fire when the countdown reaches zero.
            if (fireCountdown <= 0)
            {
                switch (turretUpgrade.currentLevel)
                {
                    case 0:
                        FireGreenShell(); // Implement this method.
                        break;
                    case 1:
                        FireRedShell(); // Implement this method.
                        break;
                    case 2:
                        FireBlueShell(); // Implement this method.
                        break;
                }

                fireCountdown = 1f / GetUpgradeValue(TurretImprovementType.FireRate);
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

    // Method to upgrade the turret to the next type
    public void UpgradeTurret()
    {
        turretUpgrade.ApplyUpgrade();
        LoadImprovementsData(turretUpgrade.GetCurrentTurretType());
    }

    // Method stub for firing a green shell.
    protected void FireGreenShell()
    {
        GameObject shellInstance = Instantiate(greenShellPrefab, firePoint.position, Quaternion.identity);
        Projectile2D shell = shellInstance.GetComponent<Projectile2D>();

        if (shell != null)
        {
            shell.Initialize(this);
        }
    }

    // Method stub for firing a red shell.
    protected void FireRedShell()
    {
        GameObject shellInstance = Instantiate(redShellPrefab, firePoint.position, Quaternion.identity);
        Projectile2D shell = shellInstance.GetComponent<Projectile2D>();

        if (shell != null)
        {
            shell.Initialize(this); // Set the reference to the firing turret.
        }
    }

    // Method stub for firing a blue shell.
    protected void FireBlueShell()
    {
        GameObject shellInstance = Instantiate(blueShellPrefab, firePoint.position, Quaternion.identity);
        Projectile2D shell = shellInstance.GetComponent<Projectile2D>();

        if (shell != null)
        {
            shell.Initialize(this); // Set the reference to the firing turret.
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
        // Check for left mouse button click and isEditing is true
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
