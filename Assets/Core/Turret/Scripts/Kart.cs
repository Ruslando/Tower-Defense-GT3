using System.Collections;
using UnityEngine;

public class Kart : MonoBehaviour
{
    [Header("Kart Settings")]
    
    public int maxHealth = 1;
    protected int currentHealth;
    public float regularSpeed = 2f;
    protected float currentSpeed;
    private bool invincibilityActive;
    private bool untargetableActive;
    private bool lightStunActive;
    private bool heavyStunActive;

    private void Start()
    {
        currentHealth = maxHealth;
        currentSpeed = regularSpeed;
    }

    private void Update()
    {
        MoveRight();
    }

    private void MoveRight ()
    {
        float movement = currentSpeed * Time.deltaTime;
        transform.Translate(Vector3.right * movement);
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

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            DestroyKart();
        }
    }

    private void DestroyKart()
    {
        Destroy(gameObject);
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
