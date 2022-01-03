using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoSinglestion<T> : MonoBehaviour where T : MonoBehaviour
{
    private static bool shuttingDown = false;
    private static object locker = new object();

    private static T instance = null;

    public static T Instance
    {
        get
        {
            if (shuttingDown)
            {
                Debug.LogWarning("Instance" + typeof(T) + "is destroyed. returning null");
                return null;
            }

            lock (locker)
            {
                if (instance == null)
                {
                    instance = (T)FindObjectOfType<T>();
                    if (instance == null)
                    {
                        instance = new GameObject(typeof(T).ToString()).AddComponent<T>();
                    }
                }
            }
            return instance;
        }
    }

    private void OnApplicationQuit()
    {
        shuttingDown = true;
    }

    private void OnDestroy()
    {
        shuttingDown = true;
    }
}
