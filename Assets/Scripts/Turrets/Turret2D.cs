using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Turret2D : MonoBehaviour
{
    public enum TurretType
    {
        GreenShell,
        RedShell,
        BlueShell
    }
    
    [Header("Turret Settings")]

    [SerializeField] private Transform firePoint; // Point where projectiles are spawned.
    [SerializeField] private Transform _rotatePoint;

    [Header("Projectile Prefabs")]
    public GameObject greenShellPrefab; // Prefab of the green shell.
    public GameObject redShellPrefab; // Prefab of the red shell.
    public GameObject blueShellPrefab; // Prefab of the blue shell.

    [Header("Upgrade Levels")]
    public List<TurretUpgradeData> turretUpgradeData = new List<TurretUpgradeData>();
    public TurretType turretType = TurretType.GreenShell; // Default turret type in the Inspector.
    private int currentUpgradeLevel = 0; // Current upgrade level.

    protected float fireCountdown = 0f; // Countdown to next shot.
    private bool isEditing;

    private void Start()
    {
        ChangeTurretUpgradeLevel();
    }

    private void LoadUpgradeData(TurretType turretType)
    {
        turretUpgradeData = new List<TurretUpgradeData>();
        // Load all TurretUpgradeData Scriptable Objects from the "UpgradeData" folder in Resources
        TurretUpgradeData[] upgrades = Resources.LoadAll<TurretUpgradeData>($"ScriptableObjects/TurretUpgrades/{turretType}Turret");
        // Add loaded upgrades to the list
        turretUpgradeData.AddRange(upgrades);
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
                switch (currentUpgradeLevel)
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

                fireCountdown = 1f / GetUpgradeValue(TurretUpgradeType.FireRate);
            }
        }
    }

    public void ApplyUpgrade(TurretUpgradeType turretUpgradeType)
    {
        TurretUpgradeData upgradeData = turretUpgradeData.FirstOrDefault(upgrade => upgrade.upgradeType == turretUpgradeType);
        if(upgradeData.currentLevel < upgradeData.upgradeLevels.Count - 1) 
        {
            upgradeData.currentLevel++;
        }
    }

    // Automatically called in the editor when a serialized field is modified.
    private void OnValidate()
    {
        ChangeTurretUpgradeLevel();
    }

    // Method to upgrade the turret based on the selected turret type.
    public void ChangeTurretUpgradeLevel()
    {
        switch (turretType)
        {
            case TurretType.GreenShell:
                currentUpgradeLevel = 0;
                // Additional logic for green shell upgrade.
                break;

            case TurretType.RedShell:
                currentUpgradeLevel = 1;
                // Additional logic for red shell upgrade.
                break;

            case TurretType.BlueShell:
                currentUpgradeLevel = 2;
                // Additional logic for blue shell upgrade.
                break;

            default:
                Debug.LogWarning("Invalid turret type selected.");
                break;
        }

        LoadUpgradeData(turretType);
    }

    // Method to upgrade the turret to the next type
    public void UpgradeTurret()
    {
        int maxTurretTypeIndex = System.Enum.GetValues(typeof(TurretType)).Length - 1;
        int currentIndex = (int)turretType;

        // Increment the current turret type index
        currentIndex++;

        // Check if the current index exceeds the maximum index value
        if (currentIndex <= maxTurretTypeIndex)
        {
            // Update the turret type
            turretType = (TurretType)currentIndex;

            ChangeTurretUpgradeLevel();
        }
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

    public float GetUpgradeValue(TurretUpgradeType turretUpgradeType)
    {
        TurretUpgradeData upgradeData = turretUpgradeData.FirstOrDefault(upgrade => upgrade.upgradeType == turretUpgradeType);
        if(upgradeData != null)
        {
            return upgradeData.CurrentUpgradeLevel.value;
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
        if(turretType != TurretType.BlueShell)
        {
            isEditing = editing;
        }
    }

    public bool GetIsEditing()
    {
        return isEditing;
    }

    public Transform GetRotatePoint()
    {
        return _rotatePoint;
    }
}
