using System;
using UnityEngine;

public class TempSocketPlayer : MonoBehaviour
{
    public static TempSocketPlayer LocalPlayer;
    
    public bool IsLocalPlayer;

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
