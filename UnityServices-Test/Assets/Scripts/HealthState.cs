using System;
using Unity.Netcode;
using UnityEngine;

public class HealthState : NetworkBehaviour
{
    [HideInInspector]
    public NetworkVariable<int> healthPoints = new NetworkVariable<int>();

    public event Action healthDepleted;

    public event Action HealthReplenished;

    void OnEnable()
    {
        healthPoints.OnValueChanged += HealthChanged;
    }

    void OnDisable()
    {
        healthPoints.OnValueChanged -= HealthChanged;
    }

    void HealthChanged(int previousValue, int newValue)
    {
        if (previousValue > 0 && newValue <= 0)
        {
            // 0 HP reached
            healthDepleted?.Invoke();
        }

        else if (previousValue <= 0 && newValue > 0)
        {
            // revived
            HealthReplenished?.Invoke();
        }
    }
}
