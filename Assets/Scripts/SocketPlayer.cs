using System;
using UnityEngine;

public class SocketPlayer : MonoBehaviour
{
    public static SocketPlayer LocalPlayer;

    public event Action<bool> OnLocalPlayerChanged;

    [field: SerializeField] public bool IsLocalPlayer { get; private set; }
    public int PlayerId = -1;

    public string PlayerName;

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
            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        }
        else
        {
            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        }

        IsLocalPlayer = local;

        OnLocalPlayerChanged?.Invoke(IsLocalPlayer);
    }

    private void OnApplicationQuit()
    {
        LocalPlayer = null;
    }
}
