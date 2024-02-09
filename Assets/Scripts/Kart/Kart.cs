using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum KartBuffType
{
    Invincibility,
    Untargetable
}

public enum KartDebuffType
{
    LightStun,
    HeavyStun
}


public class Kart : MonoBehaviour
{
    private KartStats kartStats;
    private float currentSpeed;
    private bool isAccelerating;
    private float lightStunRecoveryDuration = 0.5f;
    private float heavyStunRecoveryDuration = 2f;

    public List<KartBuffType> buffs = new List<KartBuffType>();
    public List<KartDebuffType> debuffs = new List<KartDebuffType>();

    public static event Action<Kart> OnLapCompleted;
    public static event Action<Kart> OnLastLapCompleted;
    public static event Action<Kart> OnLapCompletedFirst;
    public static event Action<Kart> OnAllLapsCompleted;
    public static event Action<Kart> OnOvertake;
    public static event Action<Kart, KartBuffType> OnBuffApplied;
    public static event Action<Kart, KartBuffType> OnBuffRemoved;
    public static event Action<Kart, KartDebuffType> OnDebuffApplied;
    public static event Action<Kart, KartDebuffType> OnDebuffRemoved;

    // Movement
    public Waypoint Waypoint { get; set; }
    public Vector3 TargetPointPosition => Waypoint.GetWaypointPosition(TargetWaypointIndex);
    public Vector3 CurrentPointPosition => Waypoint.GetWaypointPosition(CurrentWaypointIndex);
    
    private int _targetWaypointIndex;
    public int TargetWaypointIndex { get {
            return _targetWaypointIndex;
        }
        private set {
            if(Waypoint != null)
            {
                _targetWaypointIndex = value % Waypoint.Points.Length;
                CurrentWaypointIndex = (value - 1) % Waypoint.Points.Length;
            } else {
                _targetWaypointIndex = value;
                CurrentWaypointIndex = value - 1;
            }
            
        }
    }
    public int CurrentWaypointIndex { get; private set; }
    public int Lap { get; private set; }
    public int LapPosition { get; private set; }

    //Animation
    private Animator _animator;

    // Visuals
    private SpriteRenderer _spriteRenderer;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        Drive();
        Move();
        RotateSprite();
        
        if (CurrentPointPositionReached())
        {
            UpdateCurrentPointIndex();
        }
    }

    public void Reset()
    {
        currentSpeed = 0f;
        isAccelerating = false;
        buffs.Clear();
        debuffs.Clear();
        Waypoint = null;
        TargetWaypointIndex = 0;
        Lap = 0;
        LapPosition = 0;
    }

    public void SetStartValues(Vector3 position, Waypoint waypoint, KartStats kartStats)
    {
        transform.localPosition = position;
        Waypoint = waypoint;
        TargetWaypointIndex = 1;
        this.kartStats = kartStats;
        gameObject.name = kartStats.kartName;
    }

    public void StartEngine()
    {
        isAccelerating = true;
    }

    private void Move()
    {
        transform.position = Vector3.MoveTowards(transform.position, 
            TargetPointPosition, currentSpeed * Time.deltaTime);
    }

    private void RotateSprite()
    {
        Vector2 direction = TargetPointPosition - CurrentPointPosition;
        float xDifference = Mathf.Abs(direction.x);
        float yDifference = Mathf.Abs(direction.y);

        if (xDifference > yDifference)
        {
            _animator.SetBool("MoveUp", false);
            _animator.SetBool("MoveDown", false);
            _spriteRenderer.flipX = TargetPointPosition.x < CurrentPointPosition.x;
        }
        else
        {
            _animator.SetBool("MoveUp", TargetPointPosition.y > CurrentPointPosition.y);
            _animator.SetBool("MoveDown", TargetPointPosition.y < CurrentPointPosition.y);
        }
    }

    private void Drive()
    {
        if(debuffs.Count == 0)
        {
            if(isAccelerating)
            {
                Accelerate();
            } else {
                Decelerate();
            }
        }
    }

    private void Accelerate()
    {
        currentSpeed = Mathf.Clamp(currentSpeed + Time.deltaTime, 0f, kartStats.topSpeed);
    }

    private void Decelerate()
    {
        currentSpeed = Mathf.Max(currentSpeed - Time.deltaTime, 0f);
    }

    private bool CurrentPointPositionReached()
    {
        float distanceThreshold = 0.1f;
        return Vector3.Distance(transform.position, TargetPointPosition) < distanceThreshold;
    }

    private void UpdateCurrentPointIndex()
    {
        if (TargetWaypointIndex == 0)
        {
            EndPointReached();
        }
        TargetWaypointIndex++;
    }

    private void EndPointReached()
    {
        Lap++;
        if(Lap == KartManager.Instance.GetMaxLaps())
        {
            OnAllLapsCompleted?.Invoke(this);
        } else if (Lap == KartManager.Instance.GetMaxLaps() - 1 && LapPosition == 0) {
            OnLastLapCompleted?.Invoke(this);
        }
        else {
            if(LapPosition == 0) {
                OnLapCompletedFirst?.Invoke(this);
            }
            OnLapCompleted?.Invoke(this);
        }
    }

    public void SetLapPosition(int newLapPosition)
    {
        if(newLapPosition > LapPosition)
        {
            OnOvertake?.Invoke(this);
        }
        LapPosition = newLapPosition;
    }

    public void ApplyLightStunEffect(float durationInSeconds)
    {
        durationInSeconds *= kartStats.stunTimeMultiplier;
        if (debuffs.Count == 0 && !buffs.Contains(KartBuffType.Invincibility))
        {
            StartCoroutine(LightStunEffectCoroutine(durationInSeconds));
            StartCoroutine(InvincibilityCoroutine(durationInSeconds + lightStunRecoveryDuration));
            StartCoroutine(UntargetableCoroutine(durationInSeconds + lightStunRecoveryDuration));
        }
    }

    public void ApplyHeavyStunEffect(float durationInSeconds)
    {
        durationInSeconds *= kartStats.stunTimeMultiplier;
        if (debuffs.Count == 0 && !buffs.Contains(KartBuffType.Invincibility))
        {
            StartCoroutine(HeavyStunEffectCoroutine(durationInSeconds));
            StartCoroutine(InvincibilityCoroutine(durationInSeconds + heavyStunRecoveryDuration));
            StartCoroutine(UntargetableCoroutine(durationInSeconds + heavyStunRecoveryDuration));
        }
    }

    private IEnumerator LightStunEffectCoroutine(float durationInSeconds)
    {
        ApplyDebuff(KartDebuffType.LightStun);
        PlayHurtAnimation();
        yield return StartCoroutine(SetSpeedOverTimeCoroutine(0f, durationInSeconds));
        StopHurtAnimation();
        RemoveDebuff(KartDebuffType.LightStun);
    }

    private void PlayHurtAnimation()
    {
        _animator.SetBool("LightStun", true);
    }

    private void StopHurtAnimation()
    {
        _animator.SetBool("LightStun", false);
    }

    private IEnumerator HeavyStunEffectCoroutine(float stunTime)
    {
        ApplyDebuff(KartDebuffType.HeavyStun);
        yield return StartCoroutine(SetSpeedOverTimeCoroutine(0f, 0f)); // applies effect immediatly
        yield return new WaitForSeconds(stunTime);
        RemoveDebuff(KartDebuffType.HeavyStun);
    }

    public void ApplyInvincibility(float durationInSeconds)
    {
        if (!buffs.Contains(KartBuffType.Invincibility))
        {
            StartCoroutine(InvincibilityCoroutine(durationInSeconds));
        }
    }

    private IEnumerator InvincibilityCoroutine(float durationInSeconds)
    {
        ApplyBuff(KartBuffType.Invincibility);
        yield return new WaitForSeconds(durationInSeconds);
        RemoveBuff(KartBuffType.Invincibility);
    }

    public void ApplyUntargetable(float durationInSeconds)
    {
        if (!buffs.Contains(KartBuffType.Untargetable))
        {
            StartCoroutine(UntargetableCoroutine(durationInSeconds));
        }
    }

    private IEnumerator UntargetableCoroutine(float durationInSeconds)
    {
        ApplyBuff(KartBuffType.Untargetable);

        yield return new WaitForSeconds(durationInSeconds);

        RemoveBuff(KartBuffType.Untargetable);
    }

    public void SetSpeedOverTime(float targetSpeed, float duration)
    {
        StartCoroutine(SetSpeedOverTimeCoroutine(targetSpeed, duration));
    }

    private IEnumerator SetSpeedOverTimeCoroutine(float targetSpeed, float duration)
    {
        float initialSpeed = currentSpeed;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            currentSpeed = Mathf.Lerp(initialSpeed, targetSpeed, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        currentSpeed = targetSpeed;
    }

    private void ApplyBuff(KartBuffType buff)
    {
        buffs.Add(buff);
        OnBuffApplied?.Invoke(this, buff);
    }

    private void RemoveBuff(KartBuffType buff)
    {
        buffs.Remove(buff);
        OnBuffRemoved?.Invoke(this, buff);
    }

    private void ApplyDebuff(KartDebuffType debuff)
    {
        debuffs.Add(debuff);
        OnDebuffApplied?.Invoke(this, debuff);
    }

    private void RemoveDebuff(KartDebuffType debuff)
    {
        debuffs.Remove(debuff);
        OnDebuffRemoved?.Invoke(this, debuff);
    }
}
