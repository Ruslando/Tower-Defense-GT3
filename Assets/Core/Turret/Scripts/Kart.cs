using System.Collections;
using UnityEngine;

public class Kart : MonoBehaviour
{
    [Header("Kart Settings")]
    
    public float regularSpeed = 2f;
    protected float currentSpeed;

    // state

    private bool invincibilityActive;
    private bool untargetableActive;
    private bool lightStunActive;
    private bool heavyStunActive;

    // movement
    public Waypoint Waypoint { get; set; }
    public Vector3 CurrentPointPosition => Waypoint.GetWaypointPosition(_currentWaypointIndex);
    
    private int _currentWaypointIndex;
    private Vector3 _lastPointPosition;

    // visuals
    private SpriteRenderer _spriteRenderer;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        
        currentSpeed = regularSpeed;

        _currentWaypointIndex = 0;
        _lastPointPosition = transform.position;
    }

    private void Update()
    {
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
        if (_currentWaypointIndex < lastWaypointIndex)
        {
            _currentWaypointIndex++;
        }
        else
        {
            EndPointReached();
        }
    }

    private void EndPointReached()
    {
        // OnEndReached?.Invoke(this);
        // _enemyHealth.ResetHealth();
        // ObjectPooler.ReturnToPool(gameObject);
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

        yield return StartCoroutine(SetSpeedOverTimeCoroutine(regularSpeed, recoveryTimeInSeconds));

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

        yield return StartCoroutine(SetSpeedOverTimeCoroutine(0f, durationInSeconds));

        yield return null;

        yield return StartCoroutine(SetSpeedOverTimeCoroutine(regularSpeed, durationInSeconds));

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
