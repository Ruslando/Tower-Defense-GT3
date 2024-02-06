using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueShell : Projectile2D
{
    [Header("Blast Settings")]
    public float blastRadius = 2f; // Radius for homing in on a target.

    protected override void Start()
    {
        base.Start();
        SetTarget(GetFirstKartInPosition());
    }

    // Update is called once per frame
    void Update()
    {
        MoveProjectile();
    }

    private Transform GetFirstKartInPosition()
    {
        return KartManager.Instance.GetKartInFirstPosition().transform;
    }

    protected override void MoveProjectile()
    {
        if(target != null)
        {
            // Move towards the target position without changing the rotation.
            rb.velocity = (target.position - transform.position).normalized * firingTurret.GetUpgradeValue(TurretImprovementType.ProjectileSpeed);

            if(IsAtTargetPosition())
            {
                Detonate();
            }
        }
        else 
        {
            Destroy(gameObject);
        }
    }

    private bool IsAtTargetPosition(float tolerance = 0.1f)
    {
        if (target != null)
        {
            // Check if the projectile's position is close to the target position in the x and z axes.
            float deltaX = Mathf.Abs(transform.position.x - target.position.x);
            float deltaY = Mathf.Abs(transform.position.y - target.position.y);

            return deltaX <= tolerance && deltaY <= tolerance;
        }

        return false;
    }

    private void Detonate()
    {
        // Get the colliders within the specified sphere.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, firingTurret.GetUpgradeValue(TurretImprovementType.BlastRadius));

        // Check if there are karts among the colliders.
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Kart"))
            {
                Kart kart = collider.gameObject.GetComponent<Kart>();

                // Check if the entity component is not null.
                if (kart != null)
                {
                    // Call the TakeDamage method on the entity.
                    kart.ApplyHeavyStunEffect(firingTurret.GetUpgradeValue(TurretImprovementType.StunTime), 2);
                }
            }
        }

        // Destroy the shell upon hitting an enemy.
        Destroy(gameObject);
        return; // Exit the method to prevent additional logic after destroying the shell.
    }
}
