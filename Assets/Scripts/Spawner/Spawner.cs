using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum SpawnModes
{
    Fixed,
    Random
}

public class Spawner : MonoBehaviour
{
    public static Action OnWaveCompleted;
    
    [Header("Settings")]
    [SerializeField] private SpawnModes spawnMode = SpawnModes.Fixed;
    [SerializeField] private int enemyCount = 10;
    [SerializeField] private float delayBtwWaves = 1f;

    [Header("Fixed Delay")]
    [SerializeField] private float delayBtwSpawns;
    
    [Header("Random Delay")]
    [SerializeField] private float minRandomDelay;
    [SerializeField] private float maxRandomDelay;

    [Header("Poolers")] 
    [SerializeField] private ObjectPooler enemyWave10Pooler;
    [SerializeField] private ObjectPooler enemyWave11To20Pooler;
    [SerializeField] private ObjectPooler enemyWave21To30Pooler;
    [SerializeField] private ObjectPooler enemyWave31To40Pooler;
    [SerializeField] private ObjectPooler enemyWave41To50Pooler;
    
    private float _spawnTimer;
    private int _enemiesSpawned;
    private int _enemiesRamaining;
    
    private Waypoint _waypoint;

    private void Start()
    {
        _waypoint = GetComponent<Waypoint>();

        _enemiesRamaining = enemyCount;
    }

    private void Update()
    {
        _spawnTimer -= Time.deltaTime;
        if (_spawnTimer < 0)
        {
            _spawnTimer = GetSpawnDelay();
            if (_enemiesSpawned < enemyCount)
            {
                _enemiesSpawned++;
                //SpawnEnemy();
                SpawnKart();
            }
        }
    }

    // private void SpawnEnemy()
    // {
    //     GameObject newInstance = GetPooler().GetInstanceFromPool();
    //     Enemy enemy = newInstance.GetComponent<Enemy>();
    //     enemy.Waypoint = _waypoint;
    //     enemy.ResetEnemy();

    //     enemy.transform.localPosition = transform.position;
    //     newInstance.SetActive(true);
    // }

    private void SpawnKart()
    {
        GameObject newInstance = GetPooler().GetInstanceFromPool();
        Kart enemy = newInstance.GetComponent<Kart>();
        enemy.Waypoint = _waypoint;
        //enemy.ResetEnemy();

        enemy.transform.localPosition = transform.position;
        newInstance.SetActive(true);
    }

    private float GetSpawnDelay()
    {
        float delay = 0f;
        if (spawnMode == SpawnModes.Fixed)
        {
            delay = delayBtwSpawns;
        }
        else
        {
            delay = GetRandomDelay();
        }

        return delay;
    }
    
    private float GetRandomDelay()
    {
        float randomTimer = Random.Range(minRandomDelay, maxRandomDelay);
        return randomTimer;
    }

    private ObjectPooler GetPooler()
    {
        int currentWave = LevelManager.Instance.CurrentWave;
        if (currentWave <= 10) // 1- 10
        {
            return enemyWave10Pooler;
        }

        if (currentWave > 10 && currentWave <= 20) // 11- 20
        {
            return enemyWave11To20Pooler;
        }
        
        if (currentWave > 20 && currentWave <= 30) // 21- 30
        {
            return enemyWave21To30Pooler;
        }
        
        if (currentWave > 30 && currentWave <= 40) // 31- 40
        {
            return enemyWave31To40Pooler;
        }
        
        if (currentWave > 40 && currentWave <= 50) // 41- 50
        {
            return enemyWave41To50Pooler;
        }

        return null;
    }
    
    private IEnumerator NextWave()
    {
        yield return new WaitForSeconds(delayBtwWaves);
        _enemiesRamaining = enemyCount;
        _spawnTimer = 0f;
        _enemiesSpawned = 0;
    }
    
    private void RecordEnemy(Enemy enemy)
    {
        _enemiesRamaining--;
        if (_enemiesRamaining <= 0)
        {
            OnWaveCompleted?.Invoke();
            StartCoroutine(NextWave());
        }
    }
    
    private void OnEnable()
    {
        Enemy.OnEndReached += RecordEnemy;
        EnemyHealth.OnEnemyKilled += RecordEnemy;
    }

    private void OnDisable()
    {
        Enemy.OnEndReached -= RecordEnemy;
        EnemyHealth.OnEnemyKilled -= RecordEnemy;
    }
}
