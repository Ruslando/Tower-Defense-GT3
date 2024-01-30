using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenShell : Projectile2D
{
    // Update is called once per frame
    void Update()
    {
        MoveProjectile();
    }

    protected override void HandleCollisionWall(Collision2D collision)
    {
        if (bounceCount < maxBounces)
        {
            // Get the contact point and calculate the reflection direction
            ContactPoint2D contact = collision.GetContact(0);
            firingDirection = Vector2.Reflect(firingDirection.normalized, contact.normal);

            // Apply the new velocity with a bounce force
            rb.velocity = firingDirection * speed;
        }
        else
        {
            // Destroy the shell if the maximum bounces are reached
            Destroy(gameObject);
        }
    }

    protected override void HandleCollisionKart(Collision2D collision)
    {
        Kart kart = collision.gameObject.GetComponent<Kart>();

        // Check if the entity component is not null.
        if (kart != null)
        {
            // Call the TakeDamage method on the entity.
            kart.ApplyLightStunEffect(1);

            // Destroy the green shell upon hitting an enemy.
            Destroy(gameObject);
            return; // Exit the method to prevent additional logic after destroying the shell.
        }
    }

    protected override void HandleCollisionProjectile(Collision2D collision)
    {
        Destroy(gameObject);
        return;
    }
}
