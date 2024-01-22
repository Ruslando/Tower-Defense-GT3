using System.Collections;
using UnityEngine;

public class Kart : MonoBehaviour
{
    [Header("Kart Settings")]
    public int maxHealth = 1; // Maximum health of the kart.
    protected int currentHealth; // Current health of the kart.
    public float regularSpeed = 5f;
    protected float currentSpeed;

    private Coroutine activeLightStunCoroutine;

    // Start is called before the first frame update.
    protected virtual void Start()
    {
        currentHealth = maxHealth; // Initialize health to the maximum when the kart is spawned.
        currentSpeed = regularSpeed;
    }

    // Method to apply a gradual light stun effect
    public void ApplyLightStunEffect(float durationInSeconds)
    {
        // If a light stun effect is already active, stop it before applying a new one
        if (activeLightStunCoroutine != null)
        {
            StopCoroutine(activeLightStunCoroutine);
        }

        // Start a new gradual light stun effect coroutine
        activeLightStunCoroutine = StartCoroutine(LightStunEffectCoroutine(durationInSeconds));
    }

    // Coroutine to handle the gradual light stun effect
    private IEnumerator LightStunEffectCoroutine(float durationInSeconds)
    {
        float elapsedTime = 0f;

        // Gradually decrease the speed to zero over the specified duration
        while (elapsedTime < durationInSeconds)
        {
            float t = elapsedTime / durationInSeconds;
            currentSpeed = Mathf.Lerp(regularSpeed, 0f, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the speed is set to zero at the end of the duration
        currentSpeed = 0f;

        // Reset the active gradual light stun effect coroutine
        activeLightStunCoroutine = null;

        // Wait for a frame to ensure that the speed is updated before the next actions
        yield return null;

        // Restore the regular speed
        currentSpeed = regularSpeed;
    }

    // Method to deal damage to the kart.
    public void TakeDamage(int damage)
    {
        // Implement logic to decrease the kart's health based on the damage received.
        currentHealth -= damage;

        // Check if the kart's health has reached zero or below.
        if (currentHealth <= 0)
        {
            DestroyKart(); // Call the method to destroy the kart.
        }
    }

    // Method to handle the destruction of the kart.
    private void DestroyKart()
    {
        // Implement logic to handle the destruction of the kart.
        // For example, play destruction effects, remove the kart from the scene, etc.
        Destroy(gameObject);
    }
}
