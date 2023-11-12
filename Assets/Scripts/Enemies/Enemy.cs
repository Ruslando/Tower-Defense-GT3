using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    // Actions
    public static event Action<GameObject> enemyDestroyed;

    public void DestroyEnemy()
    {
        enemyDestroyed?.Invoke(this.gameObject);
        Destroy(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
