using System;
using UnityEngine;

public class SocketPlayer : MonoBehaviour
{
    public static SocketPlayer LocalPlayer;
    
    public bool IsLocalPlayer;
    public int PlayerId;

    private void Start()
    {
        if (LocalPlayer == null)
        {
            MakeLocalPlayer();
        }
    }

    public void MakeLocalPlayer()
    {
        if (LocalPlayer != null)
        {
            LocalPlayer.IsLocalPlayer = false;
        }
        
        LocalPlayer = this;
        IsLocalPlayer = true;
    }

    private void OnApplicationQuit()
    {
        LocalPlayer = null;
    }
}
