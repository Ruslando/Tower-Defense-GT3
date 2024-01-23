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
        SetTarget(GetFirstUntargetableKart());
    }

    // Update is called once per frame
    void Update()
    {
        MoveProjectile();
    }

    private Transform GetFirstUntargetableKart()
    {
        GameObject[] kartObjects = GameObject.FindGameObjectsWithTag("Kart");

        foreach (GameObject kartObject in kartObjects)
        {
            // Assuming each Kart object has a Kart component
            Kart kartComponent = kartObject.GetComponent<Kart>();

            if (kartComponent != null && !kartComponent.IsUntargetable())
            {
                // Return the transform of the first untargetable kart
                return kartObject.transform;
            }
        }

        // Return null if no untargetable kart is found
        return null;
    }

    protected override void MoveProjectile()
    {
        if(target != null)
        {
            // Move towards the target position without changing the rotation.
            rb.velocity = (target.position - transform.position).normalized * speed;

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
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, blastRadius);

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
                    kart.ApplyHeavyStunEffect(1, 2);
                }
            }
        }

        // Destroy the shell upon hitting an enemy.
        Destroy(gameObject);
        return; // Exit the method to prevent additional logic after destroying the shell.
    }
}
