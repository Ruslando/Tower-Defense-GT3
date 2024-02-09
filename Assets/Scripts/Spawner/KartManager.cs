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
    public int CurrentLap { get; set; }

    [Header("Spawn Settings")]
    [SerializeField] private Vector3 startingPosition;
    [SerializeField] private int rows = 3;
    [SerializeField] private int cols = 3;
    [SerializeField] private Vector3 gridSize = new Vector3(3f, 0f, 3f);

    [Header("Poolers")]
    [SerializeField] private ObjectPooler kartPooler;
    public List<Kart> karts = new List<Kart>();
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
    }

    private void Update()
    {
        UpdateKartPositions();
    }

    private void SetupKarts()
    {
        CurrentLap = 0;
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

    private void IncrementLap(Kart kart)
    {
        CurrentLap++;
    }

    public void PlaceAndSpawnKarts()
    {
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                Vector3 staggerOffset = new Vector3(col * gridSize.x, row * gridSize.y, 0f);
                Vector3 finalPosition = startingPosition + staggerOffset;

                SpawnKart(finalPosition);
            }
        }
    }

    private void SpawnKart(Vector3 position)
    {
        GameObject kartObject = kartPooler.GetInstanceFromPool();
        Kart kart = kartObject.GetComponent<Kart>();

        KartStats kartStat = GetRandomKartStat();
        kart.SetStartValues(position, _waypoint, kartStat);

        kartObject.SetActive(true);
        karts.Add(kart);
    }

    private KartStats GetRandomKartStat()
    {
        KartStats[] kartStats = Resources.LoadAll<KartStats>("ScriptableObjects/KartStats");
        return kartStats[Random.Range(0, kartStats.Length)];
    }

    private void ResetAndRemoveAllKarts()
    {
        foreach (var kart in karts)
        {
            kart.Reset();
            ObjectPooler.ReturnToPool(kart.gameObject);
        }

        karts.Clear();
    }

    private void UpdateKartPositions()
    {
        karts.Sort((kartA, kartB) => CompareKartPositions(kartA, kartB));

        for (int i = 0; i < karts.Count; i++)
        {
            karts[i].SetLapPosition(i);
        }
    }

    private int CompareKartPositions(Kart kartA, Kart kartB)
    {
        if (kartA.Lap != kartB.Lap)
        {
            return kartB.Lap.CompareTo(kartA.Lap);
        }

        if (kartA.CurrentWaypointIndex != kartB.CurrentWaypointIndex)
        {
            return kartB.CurrentWaypointIndex.CompareTo(kartA.CurrentWaypointIndex);
        }

        Vector3 waypointPosition = _waypoint.GetWaypointPosition(kartA.CurrentWaypointIndex);

        return (kartB.transform.position - waypointPosition).magnitude.CompareTo((kartA.transform.position - waypointPosition).magnitude);
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
