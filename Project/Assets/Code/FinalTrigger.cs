using System;
using UnityEngine;

public class FinalTrigger : MonoBehaviour
{
    public static Action OnGameCompleted;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            OnGameCompleted?.Invoke();
    }
}