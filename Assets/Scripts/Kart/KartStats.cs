using UnityEngine;

[CreateAssetMenu(fileName = "KartStats", menuName = "Kart Stats", order = 1)]
public class KartStats : ScriptableObject
{
    public string kartName;
    public float topSpeed;
    public float accelerationRate;
    public float decelerationRate;
}