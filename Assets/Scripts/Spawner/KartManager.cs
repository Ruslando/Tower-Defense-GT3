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
    private int currentLap;

    [Header("Spawn Settings")]
    [SerializeField] private Vector3 startingPosition;
    [SerializeField] private int rows = 3; // Number of rows
    [SerializeField] private int cols = 3; // Number of columns
    public Vector3 gridSize = new Vector3(3f, 0f, 3f); // Size of the grid (spacing between karts)

    [Header("Poolers")]
    [SerializeField] private ObjectPooler kartPooler;
    public List<Kart> karts = new List<Kart>();
    private KartStats[] kartStats;
    private Waypoint _waypoint;

    private void OnEnable()
    {
        LevelManager.OnRestartGame += SetupKarts;
        LevelManager.OnStartGame += StartGame;
        Kart.OnLapCompletedFirst += IncrementLap;
    }

    private void OnDisable()
    {
        LevelManager.OnRestartGame -= SetupKarts;
        LevelManager.OnStartGame -= StartGame;
        Kart.OnLapCompletedFirst -= IncrementLap;
    }

    private void Start()
    {
        _waypoint = GetComponent<Waypoint>();
        LoadKartStats();
    }

    private void LoadKartStats()
    {
        kartStats = Resources.LoadAll<KartStats>($"ScriptableObjects/KartStats");
    }

    private void Update()
    {
        UpdateKartPositions();
    }

    private void IncrementLap(Kart kart)
    {
        currentLap++;
    }

    public int GetCurrentLap()
    {
        return currentLap;
    }

    // Method to spawn karts diagonally
    public void PlaceAndSpawnKarts()
    {
        int index = 0;
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                // Calculate staggered position
                Vector3 staggerOffset = new Vector3(col * gridSize.x, row * gridSize.y, 0f);
                Vector3 finalPosition = startingPosition + staggerOffset;

                SpawnKart(index++, finalPosition);
            }
        }
    }

    private void SpawnKart(int index, Vector3 position)
    {
        GameObject gameObject = kartPooler.GetInstanceFromPool();
        Kart kart = gameObject.GetComponent<Kart>();

        // Retrieve the element at the random index
        KartStats kartStat = kartStats[Random.Range(0, kartStats.Length)];

        kart.SetStartValues(position, _waypoint, kartStat);

        gameObject.SetActive(true);
        karts.Add(kart);
    }

    private void SetupKarts()
    {
        ResetAndRemoveAllKarts();
        PlaceAndSpawnKarts();
    }

    private void StartGame()
    {
        for (int i = 0; i < karts.Count; i++)
        {
            karts[i].StartEngine();
        }
    }

    private void ResetAndRemoveAllKarts()
    {
        for (int i = 0; i < karts.Count; i++)
        {
            karts[i].Reset();
            ObjectPooler.ReturnToPool(karts[i].gameObject);
        }

        karts = new List<Kart>();
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
            karts[i].SetLapPosition((uint)i);
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
        // Vector3 waypoint2 = _waypoint.GetWaypointPosition((kart1.TargetWaypointIndex + 1) % _waypoint.Points.Length);

        // // Calculate direction vector between waypoints
        // Vector3 direction = waypoint2 - waypoint1;

        // // Project vectors onto direction vector
        // Vector3 projection1 = Vector3.Project(kart1.transform.position, direction);
        // Vector3 projection2 = Vector3.Project(kart2.transform.position, direction);

        // // Calculate vector from the first position to the third position
        // Vector3 projectionVector1 = projection1 - waypoint1;
        // Vector3 projectionVector2 = projection2 - waypoint1;



        // Compare distances of projections to determine if kart2 is in front of kart1
        return (kart1.transform.position - waypoint1).magnitude > (kart2.transform.position - waypoint1).magnitude;
    }

    public Kart GetKartInFirstPosition()
    {
        return karts[0];
    }

    public int GetMaxLaps()
    {
        return maxLaps;
    }
}
