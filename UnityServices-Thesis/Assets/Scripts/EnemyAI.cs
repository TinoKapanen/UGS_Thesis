using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EnemyAI : NetworkBehaviour, IDamageable
{
    [SerializeField] private Transform wallBroken;
    private HealthSystem healthSystem;
    private Vector3 lastDamagePosition;

    private void Awake()
    {
        healthSystem = new HealthSystem(100);
        healthSystem.OnDead += HealthSystem_OnDead;
    }

    private void HealthSystem_OnDead(object sender, System.EventArgs e)
    {
        Transform wallBrokenTransform = Instantiate(wallBroken, transform.position, transform.rotation);
        foreach (Transform child in wallBrokenTransform)
        {
            if (child.TryGetComponent<Rigidbody>(out Rigidbody childRigibody))
            {
                childRigibody.AddExplosionForce(100f, lastDamagePosition, 5f);
                Destroy(wallBrokenTransform.gameObject, 2f);

            }
        }
        
        Destroy(gameObject);
    }
    

    public void Damage(int damageValue)
    {
        healthSystem.Damage(damageValue);
    }
}
