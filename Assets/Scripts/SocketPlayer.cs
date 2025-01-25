using System;
using UnityEngine;

public class SocketPlayer : MonoBehaviour
{
    public static SocketPlayer LocalPlayer;

    public event Action<bool> OnLocalPlayerChanged;

    [field: SerializeField] public bool IsLocalPlayer { get; private set; }
    public int PlayerId = -1;

    private void Start()
    {
        if (LocalPlayer == null)
        {
            SetIsLocalPlayer(true);
        }
    }

    public void SetIsLocalPlayer(bool local)
    {
        if (local)
        {
            if (LocalPlayer != null)
            {
                LocalPlayer.IsLocalPlayer = false;
            }

            LocalPlayer = this;
        }

        IsLocalPlayer = local;

        OnLocalPlayerChanged?.Invoke(IsLocalPlayer);
    }

    private void OnApplicationQuit()
    {
        LocalPlayer = null;
    }
}
