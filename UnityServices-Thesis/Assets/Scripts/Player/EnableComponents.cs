using Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;

namespace StarterAssets
{
    public class EnableComponents : NetworkBehaviour
    {
        private GameObject follow;
        private CinemachineVirtualCamera cinemachineVirtualCamera;
        [SerializeField] private Transform target;

        
        void Start()
        {
            if (IsLocalPlayer)
            {
                CharacterController characterController = GetComponent<CharacterController>();
                RigBuilder aimRig = GetComponent<RigBuilder>();
                BoneRenderer boneRenderer = GetComponent<BoneRenderer>();
                aimRig.enabled = true;
                characterController.enabled = true;
                ThirdPersonController tpc = GetComponent<ThirdPersonController>();
                ShootingController tps = GetComponent<ShootingController>();
                tpc.enabled = true;
                tps.enabled = true;
                PlayerInput _input = GetComponent<PlayerInput>();
                _input.enabled = true;
                follow = GameObject.Find("PlayerFollowCamera");
                cinemachineVirtualCamera = follow.GetComponent<CinemachineVirtualCamera>();
                cinemachineVirtualCamera.Follow = target;               
            }
        }
    }  
}
