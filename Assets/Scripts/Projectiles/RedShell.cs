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
        if (kart != null)
        {
            kart.ApplyLightStunEffect(firingTurret.GetUpgradeValue(TurretImprovementType.StunTime));
        }
        Destroy(gameObject);
        return;
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
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, firingTurret.GetUpgradeValue(TurretImprovementType.TargetingRadius));
            foreach (Collider2D collider in colliders)
            {
                if (collider.CompareTag("Kart"))
                {
                    Kart kart = collider.gameObject.GetComponent<Kart>();
                    if (kart != null)
                    {
                        if (!kart.buffs.Contains(KartBuffType.Untargetable))
                        {
                            isHoming = true;
                            target = collider.transform;
                        }
                    }
                }
            }
        }
    }
}
