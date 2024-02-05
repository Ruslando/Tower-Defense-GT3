using System;
using System.Collections;
using System.Collections.Generic;
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
    public TurretType turretType = TurretType.GreenShell; // Default turret type in the Inspector.
    private int currentUpgradeLevel = 0; // Current upgrade level.

    [Header("User Control Settings")]
    public float userFireRate = 2f; // User-specified fire rate in shots per second.

    protected float fireCountdown = 0f; // Countdown to next shot.
    private bool isEditing;
    

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

                fireCountdown = 1f / userFireRate;
            }
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
            shell.SetFiringTurret(this); // Set the reference to the firing turret.
        }
    }

    // Method stub for firing a red shell.
    protected void FireRedShell()
    {
        GameObject shellInstance = Instantiate(redShellPrefab, firePoint.position, Quaternion.identity);
        Projectile2D shell = shellInstance.GetComponent<Projectile2D>();

        if (shell != null)
        {
            shell.SetFiringTurret(this); // Set the reference to the firing turret.
        }
    }

    // Method stub for firing a blue shell.
    protected void FireBlueShell()
    {
        GameObject shellInstance = Instantiate(blueShellPrefab, firePoint.position, Quaternion.identity);
        Projectile2D shell = shellInstance.GetComponent<Projectile2D>();

        if (shell != null)
        {
            shell.SetFiringTurret(this); // Set the reference to the firing turret.
        }
    }

    // Method to get the current fire rate based on the upgrade level.
    protected float GetCurrentFireRate()
    {
        // Implement logic to get fire rate based on the upgrade level.
        return userFireRate;
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
        isEditing = editing;
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
