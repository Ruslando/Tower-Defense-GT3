using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [Header("Attributes")]
    public float fireRate = 1f;
    public float fireCountdown;
    public float targetingRadius = 5f;

    private GameObject target;
    private Tween rotationTween;

    private SphereCollider sphereCollider;

    // Actions
    public static event Action<Turret, GameObject> turretFired;

    void Start()
    {
        sphereCollider = GetComponentInChildren<SphereCollider>();
    }

    void Update()
    {
        LookAtTarget();
        UpdateShoot();
        UpdateTargetingRadius();
    }
    
    private void OnEnable()
    {
        TurretTargetingSystem.enemyEnteredTargetingRadius += OnEnemyEnteredTargetingRadius;
        TurretTargetingSystem.enemyLeftTargetingRadius += OnEnemyLeftTargetingRadius;
        TurretTargetingSystem.setEnemyTarget += OnSetEnemyTarget;
        TurretTargetingSystem.clearEnemyTarget += OnClearEnemyTarget;
    }

    private void OnDisable()
    {
        TurretTargetingSystem.enemyEnteredTargetingRadius -= OnEnemyEnteredTargetingRadius;
        TurretTargetingSystem.enemyLeftTargetingRadius -= OnEnemyLeftTargetingRadius;
        TurretTargetingSystem.setEnemyTarget -= OnSetEnemyTarget;
        TurretTargetingSystem.clearEnemyTarget -= OnClearEnemyTarget;
    }

    private void OnEnemyEnteredTargetingRadius(Turret turret, GameObject enemy)
    {
        if(turret.Equals(this))
        {
            Debug.Log("Enemy Entered");
        }
        
    }

    private void OnEnemyLeftTargetingRadius(Turret turret, GameObject enemy)
    {
        if(turret.Equals(this))
        {
            Debug.Log("Enemy Left");
        }
    }

    private void OnSetEnemyTarget(Turret turret, GameObject enemy)
    {
        if(turret.Equals(this))
        {
            Debug.Log("Lock on enemy");
            target = enemy;
        }
        
    }

    private void OnClearEnemyTarget(Turret turret, GameObject enemy)
    {
        if(turret.Equals(this))
        {
            Debug.Log("Locked off enemy");
            target = null;
            ResetLook();
        }
        
    }

    private void ResetLook()
    {
        //rotationTween.Kill();
        transform.DORotate(Vector3.forward, 1).SetEase(Ease.OutCubic);
    }

    private void LookAtTarget()
    {
        if(target != null) transform.DOLookAt(target.transform.position, 0.5f);
        
    }

    private void UpdateShoot()
    {
        if (fireCountdown <= 0f && target != null)
        {
            Shoot();
            fireCountdown = 1f / fireRate;
        }

        if(fireCountdown > 0f)
        {
            fireCountdown -= Time.deltaTime;
        }
    }

    private void Shoot()
    {
        turretFired?.Invoke(this, target);
        if(target != null) target.GetComponent<Enemy>().DestroyEnemy();
        Debug.Log("Shoot");
    }

    private void UpdateTargetingRadius()
    {
        if(sphereCollider != null) sphereCollider.radius = targetingRadius;
    }
}
