using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GreenShell : Projectile2D
{
    // Update is called once per frame
    void Update()
    {
        MoveProjectile();
    }

    protected override void HandleTriggerEnterWall(Collider2D collider)
    {
        // ignore first wall collision
        if(bounceCount != 0)
        {
            if (bounceCount < maxBounces)
            {
                Vector3 collisionPoint = collider.ClosestPoint(transform.position);
                Vector3 collisionNormal = transform.position - collisionPoint;

                firingDirection = Vector2.Reflect(firingDirection.normalized, collisionNormal.normalized);
                rb.velocity = firingDirection * firingTurret.GetUpgradeValue(TurretImprovementType.ProjectileSpeed);
            }
            else
            {
                projectileSound.Stop();
                Destroy(gameObject);
            }
        }

        bounceCount++;
    }

    protected override void HandleCollisionKart(Collision2D collision)
    {
        Kart kart = collision.gameObject.GetComponent<Kart>();
        if (kart != null)
        {
            kart.ApplyLightStunEffect(firingTurret.GetUpgradeValue(TurretImprovementType.StunTime));
            projectileSound.Stop();
            Destroy(gameObject);
            return;
        }
    }

    protected override void HandleCollisionProjectile(Collision2D collision)
    {
        projectileSound.Stop();
        Destroy(gameObject);
        return;
    }
}
