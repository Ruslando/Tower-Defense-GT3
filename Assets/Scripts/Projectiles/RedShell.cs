using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedShell : Projectile2D
{
    [Header("Targeting Settings")]
    public float targetingRadius = 2f; // Radius for homing in on a target.

    // Update is called once per frame
    void Update()
    {
        MoveProjectile();
    }

    protected override void MoveProjectile()
    {
        base.MoveProjectile();

        if(isHoming && target != null)
        {
            // Move towards the target position without changing the rotation.
            rb.velocity = (target.position - transform.position).normalized * speed;
        }
    }

    protected override void HandleTriggerEnterKart(Collider2D collider)
    {
        base.HandleTriggerEnterKart(collider);

        // Get the Kart component from the collider
        Kart kartComponent = collider.GetComponent<Kart>();

        // Check if the Kart component is not null
        if (kartComponent != null)
        {
            // Check if Kart is untargetable
            if (!kartComponent.IsUntargetable())
            {
                // Invincibility is not active, continue with normal logic
                isHoming = true;
                target = collider.transform;
            }
        }
    }

    protected override void HandleCollisionWall(Collision2D collision)
    {
        // Destroy the shell if the maximum bounces are reached
        Destroy(gameObject);
        return;
    }

    protected override void HandleCollisionKart(Collision2D collision)
    {
        Kart kart = collision.gameObject.GetComponent<Kart>();

        // Check if the entity component is not null.
        if (kart != null)
        {
            // Call the TakeDamage method on the entity.
            kart.ApplyLightStunEffect(1);
        }

        // Destroy the green shell upon hitting an enemy.
        Destroy(gameObject);
        return; // Exit the method to prevent additional logic after destroying the shell.
    }

    protected override void HandleCollisionProjectile(Collision2D collision)
    {
        Destroy(gameObject);
        return;
    }
}
