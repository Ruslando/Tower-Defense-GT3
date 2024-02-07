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

    public List<KartBuffType> buffs = new List<KartBuffType>();
    public List<KartDebuffType> debuffs = new List<KartDebuffType>();

    public static event Action<Kart> OnLapCompleted;
    public static event Action<Kart> OnLapCompletedFirst;
    public static event Action<Kart> OnAllLapsCompleted;
    public static event Action<Kart, int, int> OnLapPositionChanged;
    public static event Action<Kart, KartBuffType> OnBuffApplied;
    public static event Action<Kart, KartBuffType> OnBuffRemoved;
    public static event Action<Kart, KartDebuffType> OnDebuffApplied;
    public static event Action<Kart, KartDebuffType> OnDebuffRemoved;

    // Movement
    public Waypoint Waypoint { get; set; }
    public Vector3 CurrentPointPosition => Waypoint.GetWaypointPosition(TargetWaypointIndex);
    
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
    private Vector3 lastPointPosition;
    public int Lap { get; private set; }
    public int LapPosition { get; private set; }

    //Animation
    private Animator _animator;

    // visuals
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
        RotateSpriteHorizontally();
        
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
        lastPointPosition = Vector3.zero;
        Lap = 0;
        LapPosition = 0;
    }

    public void SetStartValues(Vector3 position, Waypoint waypoint, KartStats kartStats)
    {
        transform.localPosition = position;
        lastPointPosition = position;
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
            CurrentPointPosition, currentSpeed * Time.deltaTime);
    }

    private void RotateSpriteHorizontally()
    {
        if (CurrentPointPosition.x > lastPointPosition.x)
        {
            _spriteRenderer.flipX = false;
        }
        else
        {
            _spriteRenderer.flipX = true;
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
        currentSpeed = Mathf.Clamp(currentSpeed + kartStats.accelerationRate * Time.deltaTime, 0f, kartStats.topSpeed);
    }

    private void Decelerate()
    {
        currentSpeed = Mathf.Max(currentSpeed - kartStats.decelerationRate * Time.deltaTime, 0f);
    }

    private bool CurrentPointPositionReached()
    {
        float distanceThreshold = 0.1f;
        bool distanceReached = Vector3.Distance(transform.position, CurrentPointPosition) < distanceThreshold;
        if(distanceReached) {
            lastPointPosition = transform.position;
            return true;
        }
        
        return false;
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
        } else {
            if(LapPosition == 0) {
                OnLapCompletedFirst?.Invoke(this);
            }
            OnLapCompleted?.Invoke(this);
        }
    }

    public void SetLapPosition(int lapPosition)
    {
        OnLapPositionChanged?.Invoke(this, LapPosition, lapPosition);
        LapPosition = lapPosition;
    }

    public void ApplyLightStunEffect(float durationInSeconds)
    {
        if (debuffs.Count == 0 && !buffs.Contains(KartBuffType.Invincibility))
        {
            StartCoroutine(LightStunEffectCoroutine(durationInSeconds));
            StartCoroutine(InvincibilityCoroutine(durationInSeconds));
            StartCoroutine(UntargetableCoroutine(durationInSeconds));
        }
    }

    public void ApplyHeavyStunEffect(float stunTime, float recoveryTimeInSeconds)
    {
        if (debuffs.Count == 0 && !buffs.Contains(KartBuffType.Invincibility))
        {
            StartCoroutine(HeavyStunEffectCoroutine(stunTime, recoveryTimeInSeconds));
            StartCoroutine(InvincibilityCoroutine(stunTime + recoveryTimeInSeconds));
            StartCoroutine(UntargetableCoroutine(stunTime + recoveryTimeInSeconds));
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

    private void PlayUpMovementAnimation()
    {
        _animator.SetBool("MoveUp", true);
    }
    
    private void StopUpMovementAnimation()
    {
        _animator.SetBool("MoveUp", false);
    }
    
    private void PlayDownMovementAnimation()
    {
        _animator.SetBool("MoveDown", true);
    }
    
    private void StopDownMovementAnimation()
    {
        _animator.SetBool("MoveDown", false);
    }

    private IEnumerator HeavyStunEffectCoroutine(float stunTime, float recoveryTimeInSeconds)
    {
        ApplyDebuff(KartDebuffType.HeavyStun);
        yield return StartCoroutine(SetSpeedOverTimeCoroutine(0f, 0f));
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
