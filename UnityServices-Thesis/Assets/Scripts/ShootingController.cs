using Cinemachine;
using StarterAssets;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Animations.Rigging;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.Windows;
#endif

public class ShootingController : NetworkBehaviour
{
    public static ShootingController Instance { get; private set; }

    private StarterAssetsInputs _input;
    private ThirdPersonController _controller;
    private Animator _animator;
    private CinemachineImpulseSource cinemachineImpulseSource;
    private int destroyedTargetsAmount;

    private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private float sensitivity;
    [SerializeField] private float aimDownSensitivity;
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    [SerializeField] private Transform debugGameObject;
    [SerializeField] private Transform target;
    [SerializeField] private Transform bulletProjectile;
    [SerializeField] private Transform bulletEntryPoint;
    [SerializeField] private Rig aimRig;
    [SerializeField] private Transform vfxHitGreen;
    [SerializeField] private Transform vfxHitRed;

    private void Awake()
    {
        Instance = this;
    }
    public override void OnNetworkSpawn()
    {
        if (IsLocalPlayer)
        {
            
            _controller = GetComponent<ThirdPersonController>();
            _input = GetComponent<StarterAssetsInputs>();
            debugGameObject = GameObject.Find("DebugGameObject").transform;
            GameObject aimFollow = GameObject.Find("PlayerAimCamera");
            virtualCamera = aimFollow.GetComponent<CinemachineVirtualCamera>();
            cinemachineImpulseSource = GetComponent<CinemachineImpulseSource>();
            virtualCamera.Follow = target;
            _animator = GetComponent<Animator>();

            virtualCamera.gameObject.SetActive(false);

        }

        else
        {
            return;
        }
    }
    
    private void Update()
    {
        HandleMouseWorldPosition();
        HandleShooting();
        HandleAiming();
    }

    private void HandleMouseWorldPosition()
    {
        debugGameObject.position = Vector3.Lerp(debugGameObject.position, GetMouseWorldPosition(), Time.deltaTime * 20f);
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, aimColliderLayerMask))
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
        cinemachineImpulseSource.GenerateImpulse();
        Vector3 targetPosition = GetMouseWorldPosition();
        Vector3 aimDir = (targetPosition - bulletEntryPoint.position).normalized;
        Transform bulletProjectileTransform = Instantiate(bulletProjectile, bulletEntryPoint.position, Quaternion.LookRotation(aimDir, Vector3.up));

        bulletProjectileTransform.GetComponent<BulletProjectileRaycast>().Setup(targetPosition);

        Vector2 screenCenterPoint = new Vector2(Screen.width / 2, Screen.height / 2);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f))
        {
            if (raycastHit.collider.gameObject.TryGetComponent<Rigidbody>(out Rigidbody rigidbody))
            {
                rigidbody.AddExplosionForce(1000f, targetPosition, 5f);
            }

            if (raycastHit.transform.GetComponent<BulletTarget>() != null)
            {
                if (raycastHit.collider.gameObject.TryGetComponent<IDamageable>(out IDamageable damageable))
                {
                    damageable.Damage(100);
                    destroyedTargetsAmount++;
                    Debug.Log(destroyedTargetsAmount);
                }
            }
        }
    }

    private void HandleShooting()
    {
        if (_input.fire)
        {
            if (_input.aim)
            {
                Shoot();
                _input.fire = false;
            }
        }
    }

    private void HandleAiming()
    {
        if (_input.aim)
        {
            virtualCamera.gameObject.SetActive(true);
            _controller.SetSensitivity(sensitivity);
            _controller.SetRotateOnMove(false);
            aimRig.weight = 1f;
            _animator.SetLayerWeight(1, Mathf.Lerp(_animator.GetLayerWeight(1), 1f, Time.deltaTime * 10f));

            Vector3 worldAimTarget = GetMouseWorldPosition();
            worldAimTarget.y = transform.position.y;
            Vector3 aimDirection = (worldAimTarget - transform.position).normalized;

            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);
        }
        else
        {
            virtualCamera.gameObject.SetActive(false);
            _controller.SetSensitivity(sensitivity);
            _controller.SetRotateOnMove(true);
            aimRig.weight = 0f;
            _animator.SetLayerWeight(1, Mathf.Lerp(_animator.GetLayerWeight(1), 0f, Time.deltaTime * 10f));
        }
    }

    public int GetDestroyedTargetsAmount()
    {
        return destroyedTargetsAmount;
    }
}
