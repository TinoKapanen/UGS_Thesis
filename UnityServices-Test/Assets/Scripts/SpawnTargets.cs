using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SpawnTargets : NetworkBehaviour
{
    [SerializeField] private GameObject targetPrefab;
    private GameObject prefabInstance;
    private NetworkObject networkObject;

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            return;
        }

        prefabInstance = Instantiate(targetPrefab);
        
    }
}
