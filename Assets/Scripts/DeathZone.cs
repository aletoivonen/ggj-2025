using System;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    public static event Action OnDeathTriggered;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent<SocketPlayer>(out var p) || !p.IsLocalPlayer)
        {
            return;
        }
        Debug.Log("Kill player");
        OnDeathTriggered?.Invoke();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!other.gameObject.TryGetComponent<SocketPlayer>(out var p) || !p.IsLocalPlayer)
        {
            return;
        }
        Debug.Log("Kill player");
        OnDeathTriggered?.Invoke();
    }
}
