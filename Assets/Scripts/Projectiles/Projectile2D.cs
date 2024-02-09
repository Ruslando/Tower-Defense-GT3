using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile2D : MonoBehaviour
{
    protected bool isHoming = false;
    protected Transform target;

    protected int maxBounces = 3;
    protected int bounceCount = 0;

    protected Turret2D firingTurret;
    protected Vector2 firingDirection;
    protected Rigidbody2D rb;
    protected ProjectileSound projectileSound;

    // Awake is called when the script instance is being loaded.
    public virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        projectileSound = GetComponent<ProjectileSound>();
    }

    protected virtual void Start()
    {
        projectileSound.Play();
        firingDirection = firingTurret.GetRotatePoint().up.normalized;
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
            default:
                break;
        }
    }

    protected virtual void HandleCollisionKart(Collision2D collision){}
    protected virtual void HandleCollisionProjectile(Collision2D collision){}
    protected virtual void HandleTriggerEnterWall(Collider2D collider){}
}
