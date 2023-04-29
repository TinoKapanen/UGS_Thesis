using Cinemachine;
using StarterAssets;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class TPSController : MonoBehaviour
{
    [SerializeField] private Rig aimRig;
    [SerializeField] private float normalSensitivity;
    [SerializeField] private float aimDownSensitivity;
    [SerializeField] private CinemachineVirtualCamera aimDownCamera;
    //[SerializeField] private GameObject cameraTarget;
    [SerializeField] private Transform bulletProjectile;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private Transform sound;
    [SerializeField] private List<AudioClip> shootAudioList;
    [SerializeField] private List<AudioClip> rifleShootAudioList;
    [SerializeField] private Transform mouseWorldPosition;
    [SerializeField] private LayerMask aimCollider;
    [SerializeField] private Transform target;

    private ThirdPersonController thirdPersonController;
    private StarterAssetsInputs input;
    private Animator animator;
    private CinemachineImpulseSource cinemachineImpulseSource;
    private bool isShooting;
    private float shootTimer;
    private float shootTimerMax = 0.1f;
    private float aimRigWeight;

    private void Awake()
    {
        thirdPersonController = GetComponent<ThirdPersonController>();
        input = GetComponent<StarterAssetsInputs>();
        animator = GetComponent<Animator>();
        cinemachineImpulseSource = GetComponent<CinemachineImpulseSource>();
        GameObject aimFollow = GameObject.Find("PlayerAimCamera");
        aimDownCamera = aimFollow.GetComponent<CinemachineVirtualCamera>();
        aimDownCamera.Follow = target;

        aimDownCamera.gameObject.SetActive(false);
        
    }

    private void Update()
    {
        HandleMouseWorldPosition();
        HandleShooting();
        HandleAiming();

        aimRig.weight = Mathf.Lerp(aimRig.weight, aimRigWeight, Time.deltaTime * 20f);
    }

    private void HandleMouseWorldPosition()
    {
        mouseWorldPosition.position = Vector3.Lerp(mouseWorldPosition.position, GetMouseWorldPosition(), Time.deltaTime);
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2, Screen.height / 2);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, aimCollider))
        {
            return raycastHit.point;
        }
        else
        {
            return Vector3.zero;
        }
    }

    private void Shoot()
    {
        animator.SetTrigger("ShootSingle");

        cinemachineImpulseSource.GenerateImpulse();

        Vector3 targetPosition = GetMouseWorldPosition();
        Vector3 aimDir  = (targetPosition - shootPoint.position).normalized;
        Transform bulletProjectileTransform = Instantiate(bulletProjectile, shootPoint.position, Quaternion.LookRotation(aimDir, Vector3.up));

        bulletProjectile.GetComponent<BulletProjectileRaycast>().Setup(targetPosition);

        Transform soundTransform = Instantiate(sound, transform.position, Quaternion.identity);
        soundTransform.GetComponent<AudioSource>().clip = rifleShootAudioList[UnityEngine.Random.Range(0, rifleShootAudioList.Count)];
        soundTransform.GetComponent<AudioSource>().Play();
        Destroy(soundTransform.gameObject, 1f);

        Vector2 screenCenterPoint = new Vector2(Screen.width / 2, Screen.height / 2);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f))
        {
            if (raycastHit.collider.gameObject.TryGetComponent<Rigidbody>(out Rigidbody rigidbody))
            {
                rigidbody.AddExplosionForce(1000f, targetPosition, 5f);
            }

            if (raycastHit.collider.gameObject.TryGetComponent<IDamageable>(out IDamageable damageable))
            {
                damageable.Damage(33);
            }
        }
    }

    private void HandleShooting()
    {
        if (isShooting)
        {
            shootTimer -= Time.deltaTime;
            if (shootTimer <= 0f)
            {
                shootTimer += shootTimerMax + UnityEngine.Random.Range(0f, shootTimerMax * 0.25f);

                if (input.fire)
                {
                    Shoot();
                }
            }
        }
    }

    private void OnShootStopped(object sender, EventArgs e)
    {
        isShooting = false;
    }

    private void OnShootStarted(object sender, System.EventArgs e)
    {
        isShooting = true;
        shootTimer = shootTimerMax;

        if (input.aim)
        {
            Shoot();
        }
    }

    private void OnAimStopped(object sender, System.EventArgs e)
    {
        aimDownCamera.gameObject.SetActive(false);
        thirdPersonController.SetSensitivity(normalSensitivity);
        thirdPersonController.SetRotateOnMove(false);
        aimRigWeight = 0f;
    }

    private void OnAimStarted(object sender, System.EventArgs e)
    {
        aimDownCamera.gameObject.SetActive(true);
        thirdPersonController.SetSensitivity(normalSensitivity);
        thirdPersonController.SetRotateOnMove(false);
        aimRigWeight = 1f;
    }

    private void HandleAiming()
    {
        if (input.aim)
        {
            animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 1f, Time.deltaTime * 10f));
            animator.SetLayerWeight(2, Mathf.Lerp(animator.GetLayerWeight(2), 0.5f, Time.deltaTime * 10f));

            Vector3 worldAimTarget = GetMouseWorldPosition();
            worldAimTarget.y = transform.position.y;
            Vector3 aimDirection = (worldAimTarget - transform.position).normalized;

            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);
        }
        else
        {
            animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 0f, Time.deltaTime * 10f));
        }
    }
}
