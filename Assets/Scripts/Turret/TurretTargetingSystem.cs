using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretTargetingSystem : MonoBehaviour
{

    public static event Action<GameObject> enemyEnteredTargetingRadius;
    public static event Action<GameObject> enemyLeftTargetingRadius;
    public static event Action<GameObject> setEnemyTarget;
    public static event Action<GameObject> clearEnemyTarget;
    
    public string enemyTag = "Enemy";
    public GameObject enemyTarget;
    public HashSet<GameObject> enemiesWithinTargetingRadius;

    private void OnEnable()
    {
        Enemy.enemyDestroyed += OnEnemyDestroyed;
    }

    private void OnDisable()
    {
        Enemy.enemyDestroyed -= OnEnemyDestroyed;
    }

    void Start()
    {
        enemiesWithinTargetingRadius = new HashSet<GameObject>();
    }

    void OnTriggerEnter(Collider collider)
    {
        // check if collider has "enemy" tag
        if(collider.gameObject.tag == enemyTag)
        {
            GameObject enemy = collider.gameObject;
            // add gameobject to hashset
            enemiesWithinTargetingRadius.Add(enemy);
            // notify about enemy entered the collider radius
            enemyEnteredTargetingRadius?.Invoke(enemy);
            // try locking on enemy, if not already locked on one
            TrySetTargetEnemy();
        }
    }

    void OnTriggerExit(Collider collider)
    {
        // check if collider has "enemy" tag
        if(collider.gameObject.tag == enemyTag)
        {   
            // remove gameobject from hashset
            enemiesWithinTargetingRadius.Remove(collider.gameObject);
            // notify about enemy leaving the collider radius
            enemyLeftTargetingRadius?.Invoke(collider.gameObject);
            // Try locking off enemy
            TryClearTargetEnemy(collider.gameObject);
        }
    }

    void TrySetTargetEnemy()
    {
        // try locking on enemy, if not already locked on one
        if(enemyTarget == null)
        {
            // return nearest enemy to turret
            GameObject nearestEnemy = ReturnNearestEnemy();
            if(nearestEnemy != null)
            {
                // lock on enemy
                SetTarget(nearestEnemy);
            }
        }
    }

    void TryClearTargetEnemy(GameObject enemy)
    {
        // check if there is a locked on enemy
        if(enemyTarget != null)
        {
            // check if given enemy is same as locked on enemy
            if(enemyTarget.Equals(enemy))
            {
                // finally remove enemy from lock
                ClearTarget();

                // immediatly search for the next possible enemy
                TrySetTargetEnemy();
            }
        }
    }

    void SetTarget(GameObject enemy)
    {
        // return if an enemy is already locked on
        if(enemyTarget != null) return;
        // lock on enemy
        enemyTarget = enemy;
        // notify about enemy locked in
        setEnemyTarget?.Invoke(enemy);
    }

    void ClearTarget()
    {
        // return if no enemy is locked in
        if(enemyTarget == null) return;
        // notify about enemy locked off
        clearEnemyTarget?.Invoke(enemyTarget);
        // lock off enemy
        enemyTarget = null;
        
    }

    GameObject ReturnNearestEnemy()
    {
        GameObject nearestEnemy = null;
        float shortestDistance = Mathf.Infinity;

        // iterates over all enemies inside collider and calculates shortest distance
        // to turrent position
        foreach (GameObject enemy in enemiesWithinTargetingRadius)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if(distance < shortestDistance)
            {
                shortestDistance = distance;
                nearestEnemy = enemy;
            }
        }

        return nearestEnemy;
    }

    void OnEnemyDestroyed(GameObject enemy)
    {
        // remove enemy from hashset
        enemiesWithinTargetingRadius.Remove(enemy);
        // remove current target
        ClearTarget();
        // immediatly try target the next enemy
        TrySetTargetEnemy();
    }
}
