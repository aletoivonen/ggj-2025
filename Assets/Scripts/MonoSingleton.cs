using System;
using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour
    where T : MonoSingleton<T>
{
    public static T Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
            return;
        }

        Instance = (T)this;

        DontDestroyOnLoad(gameObject);

        OnAwake();
    }

    protected virtual void OnAwake() { }

    private void OnApplicationQuit()
    {
        Instance = null;
    }
}
