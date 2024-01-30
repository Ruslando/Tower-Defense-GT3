using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile2D : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float speed = 10f; // Speed of the shell.
    public float lifetime = 5f; // Lifetime of the shell before it disappears.

    protected bool isHoming = false; // Flag indicating whether the shell is homing.
    protected Transform target; // The target the shell is homing towards.

    [Header("Bounce Settings")]
    public int maxBounces = 3; // Set the maximum number of bounces
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
        firingDirection = firingTurret.transform.up.normalized;
        // Set a lifetime for the projectile.
        if(lifetime > 0)
        {
            Destroy(gameObject, lifetime);
        }
    }

    protected virtual void MoveProjectile()
    {
        rb.velocity = firingDirection * speed;
    }

    public void SetTarget(Transform transform)
    {
        this.target = transform;
    }

    public void SetFiringTurret(Turret2D turret)
    {
        this.firingTurret = turret;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Kart"))
        {
            HandleTriggerEnterKart(other);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Wall":
                HandleCollisionWall(collision);
                break;

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

    protected virtual void HandleCollisionWall(Collision2D collision){}
    protected virtual void HandleCollisionKart(Collision2D collision){}
    protected virtual void HandleCollisionProjectile(Collision2D collision){}
    protected virtual void HandleTriggerEnterKart(Collider2D collider) {}
}
