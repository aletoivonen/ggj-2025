using System;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    public static event Action OnDeathTriggered;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Kill player");
        OnDeathTriggered?.Invoke();
    }
}
