using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile2D : MonoBehaviour
{
    private float lifetime = 5f; // Lifetime of the shell before it disappears.
    protected bool isHoming = false; // Flag indicating whether the shell is homing.
    protected Transform target; // The target the shell is homing towards.

    protected int maxBounces = 3; // Set the maximum number of bounces
    protected int bounceCount = 0; // Counter for the number of bounces

    protected Turret2D firingTurret; // Reference to the turret that fired the shell.
    protected Vector2 firingDirection;
    protected Rigidbody2D rb;

    // Awake is called when the script instance is being loaded.
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void Start()
    {
        firingDirection = firingTurret.GetRotatePoint().up.normalized;
        // Set a lifetime for the projectile.
        if(lifetime > 0)
        {
            Destroy(gameObject, lifetime);
        }
    }

    protected virtual void MoveProjectile()
    {
        rb.velocity = firingDirection * firingTurret.GetUpgradeValue(TurretImprovementType.ProjectileSpeed);
    }

    public void SetTarget(Transform transform)
    {
        this.target = transform;
    }

    public void Initialize(Turret2D firingTurret)
    {
        this.firingTurret = firingTurret;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        switch (other.gameObject.tag)
        {
            case "Wall":
                HandleTriggerEnterWall(other);
                break;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Kart":
                HandleCollisionKart(collision);
                break;

            case "Projectile":
                HandleCollisionProjectile(collision);
                break;

            // Add more cases as needed

            default:
                // Handle other cases if necessary
                break;
        }
    }

    protected virtual void HandleCollisionKart(Collision2D collision){}
    protected virtual void HandleCollisionProjectile(Collision2D collision){}
    protected virtual void HandleTriggerEnterWall(Collider2D collider){}
}
