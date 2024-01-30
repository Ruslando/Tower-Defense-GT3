﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum SpawnModes
{
    Fixed,
    Random
}

public class KartManager : Singleton<KartManager>
{
    [Header("Lap Settings")]
    [SerializeField] private int maxLaps;

    [Header("Spawn Settings")]
    [SerializeField] private Vector3 startingPosition;
    [SerializeField] private int rows = 3; // Number of rows
    [SerializeField] private int cols = 3; // Number of columns
    public Vector3 gridSize = new Vector3(3f, 0f, 3f); // Size of the grid (spacing between karts)

    [Header("Poolers")] 
    [SerializeField] private ObjectPooler kartPooler;
    List<Kart> karts = new List<Kart>();
    
    private Waypoint _waypoint;

    private void Start()
    {
        _waypoint = GetComponent<Waypoint>();
        SpawnKart(startingPosition);
        // PlaceAndSpawnKarts();
    }

    private void Update()
    {
        // UpdateKartPositions();
    }

    // Method to spawn karts diagonally
    public void PlaceAndSpawnKarts()
    {
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                // Calculate staggered position
                Vector3 staggerOffset = new Vector3(col * gridSize.x, 0f, row * gridSize.z);
                Vector3 finalPosition = startingPosition + staggerOffset;

                SpawnKart(finalPosition);
            }
        }
    }

    private void SpawnKart(Vector3 position)
    {
        GameObject newInstance = kartPooler.GetInstanceFromPool();
        Kart enemy = newInstance.GetComponent<Kart>();
        enemy.Waypoint = _waypoint;

        enemy.transform.localPosition = position;
        newInstance.SetActive(true);
    }

    private void UpdateKartPositions()
    {
        // Sort karts by position
        karts.Sort((kart1, kart2) => {
            if (IsKartInFrontOf(kart1, kart2))
                return -1;
            else if (IsKartInFrontOf(kart2, kart1))
                return 1;
            else
                return 0;
        });

        // Set lap positions for each kart
        for (int i = 0; i < karts.Count; i++)
        {
            karts[i].SetLapPosition((uint)(i + 1));
        }
    }

    private bool IsKartInFrontOf(Kart kart1, Kart kart2)
    {
        if (kart1.Lap != kart2.Lap)
        {
            return kart1.Lap > kart2.Lap; // Compare lap counts directly
        }
        
        if (kart1.CurrentWaypointIndex != kart2.CurrentWaypointIndex)
        {
            return kart1.CurrentWaypointIndex > kart2.CurrentWaypointIndex; // Compare waypoint indices
        }

        // if lap and waypoint index are equal

        // Get positions of waypoints
        Vector3 waypoint1 = _waypoint.GetWaypointPosition(kart1.CurrentWaypointIndex);
        Vector3 waypoint2 = _waypoint.GetWaypointPosition((kart1.CurrentWaypointIndex + 1) % _waypoint.Points.Length);

        // Calculate direction vector between waypoints
        Vector3 direction = (waypoint2 - waypoint1).normalized;

        // Calculate vectors from waypoints to karts' positions
        Vector3 vectorToKart1 = kart1.transform.position - waypoint1;
        Vector3 vectorToKart2 = kart2.transform.position - waypoint1;

        // Project vectors onto direction vector
        float projection1 = Vector3.Dot(vectorToKart1, direction);
        float projection2 = Vector3.Dot(vectorToKart2, direction);

        // Compare distances of projections to determine if kart2 is in front of kart1
        return projection2 > projection1;
    }

    public Kart GetKartInFirstPosition()
    {
        return karts[0];
    }
    
    private void OnEnable()
    {
        // Enemy.OnEndReached += RecordEnemy;
        // EnemyHealth.OnEnemyKilled += RecordEnemy;
    }

    private void OnDisable()
    {
        // Enemy.OnEndReached -= RecordEnemy;
        // EnemyHealth.OnEnemyKilled -= RecordEnemy;
    }
}
