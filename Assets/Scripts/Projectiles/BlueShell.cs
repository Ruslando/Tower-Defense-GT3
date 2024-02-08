using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueShell : Projectile2D
{
    [Header("Blast Settings")]
    public float blastRadius = 2f;

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
            float deltaX = Mathf.Abs(transform.position.x - target.position.x);
            float deltaY = Mathf.Abs(transform.position.y - target.position.y);
            return deltaX <= tolerance && deltaY <= tolerance;
        }

        return false;
    }

    private void Detonate()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, firingTurret.GetUpgradeValue(TurretImprovementType.BlastRadius));
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Kart"))
            {
                Kart kart = collider.gameObject.GetComponent<Kart>();
                if (kart != null)
                {
                    kart.ApplyHeavyStunEffect(firingTurret.GetUpgradeValue(TurretImprovementType.StunTime));
                }
            }
        }

        Destroy(gameObject);
        return;
    }
}
