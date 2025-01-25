using System;
using UnityEngine;

public class SideTrigger : MonoBehaviour
{
    [SerializeField] private bool _isLeftSide;
    public static event Action<bool> OnSideTriggerEnter;
    private void OnTriggerEnter2D(Collider2D other)
    {
        OnSideTriggerEnter?.Invoke(_isLeftSide);
    }
}
