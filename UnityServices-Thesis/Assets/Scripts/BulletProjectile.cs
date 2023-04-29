using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BulletProjectile : NetworkBehaviour
{
    private Rigidbody bulletRigibody;

    private void Awake()
    {
        bulletRigibody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        float speed = 370f;
        bulletRigibody.velocity = transform.forward * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<BulletTarget>() != null)
        {
            // Hit target
            //Instantiate(vfxHitGreen, transform.position, Quaternion.identity);
        }
        else
        {
            // Hit something else
            //Instantiate(vfxHitRed, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}