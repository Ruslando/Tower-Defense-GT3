using System.Collections;
using UnityEngine;

public class Kart : MonoBehaviour
{
    [Header("Kart Settings")]
    public float topSpeed = 2f;
    public float accelerationRate = 1f;
    public float decelerationRate = 1f;
    protected float currentSpeed;
    private bool isAccelerating;

    // state
    private bool invincibilityActive;
    private bool untargetableActive;
    private bool lightStunActive;
    private bool heavyStunActive;

    // movement
    public Waypoint Waypoint { get; set; }
    public Vector3 CurrentPointPosition => Waypoint.GetWaypointPosition(CurrentWaypointIndex);
    
    //animation
    private Animator _animator;
    
    public int CurrentWaypointIndex { get; private set; }
    private Vector3 _lastPointPosition;
    public uint Lap { get; private set; }
    public uint LapPosition { get; private set; }

    // visuals
    private SpriteRenderer _spriteRenderer;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        isAccelerating = true;
        CurrentWaypointIndex = 0;
        _lastPointPosition = transform.position;
    }

    private void Update()
    {
        Drive();
        Move();
        Rotate();
        
        if (CurrentPointPositionReached())
        {
            UpdateCurrentPointIndex();
        }
    }

    private void Move()
    {
        transform.position = Vector3.MoveTowards(transform.position, 
            CurrentPointPosition, currentSpeed * Time.deltaTime);
    }

    private void Rotate()
    {
        if (CurrentPointPosition.x > _lastPointPosition.x)
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
        if(!IsLightStunActive() && !IsHeavyStunActive())
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
        if (currentSpeed < topSpeed)
        {
            currentSpeed += accelerationRate * Time.deltaTime;
        }
        else if (currentSpeed > topSpeed)
        {
            // If current speed is higher than top speed, gradually decrease to top speed
            currentSpeed -= decelerationRate * Time.deltaTime;
        }

        // Ensure speed is not negative
        currentSpeed = Mathf.Max(currentSpeed, 0f);
    }

    private void Decelerate()
    {
        // Gradually decrease speed until it reaches 0
        currentSpeed -= decelerationRate * Time.deltaTime;
        // Ensure speed does not go below 0
        currentSpeed = Mathf.Max(currentSpeed, 0);
    }

    private bool CurrentPointPositionReached()
    {
        float distanceToNextPointPosition = (transform.position - CurrentPointPosition).magnitude;
        if (distanceToNextPointPosition < 0.1f)
        {
            _lastPointPosition = transform.position;
            return true;
        }

        return false;
    }

    private void UpdateCurrentPointIndex()
    {
        int lastWaypointIndex = Waypoint.Points.Length - 1;
        if (CurrentWaypointIndex < lastWaypointIndex)
        {
            CurrentWaypointIndex++;
        }
        else
        {
            EndPointReached();
        }
    }

    private void EndPointReached()
    {
        Lap++;
        CurrentWaypointIndex = 0;
        // OnEndReached?.Invoke(this);
        // _enemyHealth.ResetHealth();
        // ObjectPooler.ReturnToPool(gameObject);
    }

    public void SetLapPosition(uint lapPosition)
    {
        LapPosition = lapPosition;
    }

    public void ApplyLightStunEffect(float durationInSeconds)
    {
        if (lightStunActive == false && invincibilityActive == false)
        {
            StartCoroutine(LightStunEffectCoroutine(durationInSeconds));
            StartCoroutine(InvincibilityCoroutine(durationInSeconds));
            StartCoroutine(UntargetableCoroutine(durationInSeconds));
        }
    }

    public void ApplyHeavyStunEffect(float stunTime, float recoveryTimeInSeconds)
    {
        if (!lightStunActive && !invincibilityActive)
        {
            StartCoroutine(HeavyStunEffectCoroutine(stunTime, recoveryTimeInSeconds));
            StartCoroutine(InvincibilityCoroutine(stunTime + recoveryTimeInSeconds));
            StartCoroutine(UntargetableCoroutine(stunTime + recoveryTimeInSeconds));
        }
    }

    private IEnumerator HeavyStunEffectCoroutine(float stunTime, float recoveryTimeInSeconds)
    {
        heavyStunActive = true;
        
        yield return StartCoroutine(SetSpeedOverTimeCoroutine(0f, 0f));

        // Wait for the specified recovery time with speed at zero
        yield return new WaitForSeconds(stunTime);

        // yield return StartCoroutine(SetSpeedOverTimeCoroutine(topSpeed, recoveryTimeInSeconds));

        // Reset the active heavy stun effect flag
        heavyStunActive = false;
    }

    public void ApplyInvincibility(float durationInSeconds)
    {
        if (invincibilityActive == false)
        {
            StartCoroutine(InvincibilityCoroutine(durationInSeconds));
        }
    }
    
    private void PlayHurtAnimation()
    {
        _animator.SetTrigger("Hurt");
    }
    
    private float GetCurrentAnimationLenght()
    {
        float animationLenght = _animator.GetCurrentAnimatorStateInfo(0).length;
        return animationLenght;
    }

    private IEnumerator InvincibilityCoroutine(float durationInSeconds)
    {
        invincibilityActive = true;
        yield return new WaitForSeconds(durationInSeconds);
        invincibilityActive = false;
    }

    public void ApplyUntargetable(float durationInSeconds)
    {
        if (!untargetableActive)
        {
            StartCoroutine(UntargetableCoroutine(durationInSeconds));
        }
    }

    private IEnumerator UntargetableCoroutine(float durationInSeconds)
    {
        untargetableActive = true;
        yield return new WaitForSeconds(durationInSeconds);
        untargetableActive = false;
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

        // Ensure the speed is set to regular speed at the end of the reset
        currentSpeed = targetSpeed;
    }

    private IEnumerator LightStunEffectCoroutine(float durationInSeconds)
    {
        lightStunActive = true;
        PlayHurtAnimation();
        yield return new WaitForSeconds(GetCurrentAnimationLenght() + 0.3f);
        yield return StartCoroutine(SetSpeedOverTimeCoroutine(0f, durationInSeconds));

        // yield return StartCoroutine(SetSpeedOverTimeCoroutine(topSpeed, durationInSeconds));

        lightStunActive = false;
    }

    public bool IsLightStunActive()
    {
        return lightStunActive;
    }

    private bool IsHeavyStunActive()
    {
        return heavyStunActive;
    }

    public bool IsInvincible()
    {
        return invincibilityActive;
    }

    public bool IsUntargetable()
    {
        return untargetableActive;
    }
}
