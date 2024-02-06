using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedShell : Projectile2D
{
    // Update is called once per frame
    void Update()
    {
        MoveProjectile();
        CheckKartInRadius();
    }

    protected override void MoveProjectile()
    {
        base.MoveProjectile();

        if(isHoming && target != null)
        {
            // Move towards the target position without changing the rotation.
            rb.velocity = (target.position - transform.position).normalized * firingTurret.GetUpgradeValue(TurretImprovementType.ProjectileSpeed);
        }
    }

    protected override void HandleTriggerEnterWall(Collider2D collider)
    {
        // ignore first wall collision
        if(bounceCount != 0)
        {
            Destroy(gameObject);
        }

        bounceCount++;
    }

    protected override void HandleCollisionKart(Collision2D collision)
    {
        Kart kart = collision.gameObject.GetComponent<Kart>();

        // Check if the entity component is not null.
        if (kart != null)
        {
            // Call the TakeDamage method on the entity.
            kart.ApplyLightStunEffect(firingTurret.GetUpgradeValue(TurretImprovementType.StunTime));
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

    private void CheckKartInRadius()
    {
        if(target == null)
        {
            // Get the colliders within the specified sphere.
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, firingTurret.GetUpgradeValue(TurretImprovementType.TargetingRadius));

            // Check if there are karts among the colliders.
            foreach (Collider2D collider in colliders)
            {
                if (collider.CompareTag("Kart"))
                {
                    Kart kart = collider.gameObject.GetComponent<Kart>();

                    // Check if the entity component is not null.
                    if (kart != null)
                    {
                        // Check if Kart is untargetable
                        if (!kart.buffs.Contains(KartBuffType.Untargetable))
                        {
                            // Invincibility is not active, continue with normal logic
                            isHoming = true;
                            target = collider.transform;
                        }
                    }
                }
            }
        }
    }
}
