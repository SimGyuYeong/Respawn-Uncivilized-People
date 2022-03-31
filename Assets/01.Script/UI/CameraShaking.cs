using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CameraShaking : MonoBehaviour
{
    public static CameraShaking Instance;
    [SerializeField]
    public UnityEvent<float, float> OnShakeCam;
    public float shaking;
    [SerializeField]
    float shakeTime;
    Vector3 startPosition;
    private void Start()
    {
        startPosition = new Vector3(0f,0f,-6f);
    }

    void Awake()
    {
        if(Instance != null)
        {
            Debug.LogError("¿À·ù");
        }
        Instance = this;
    }

    public void ShakeCam(float time = 0.13f, float str = 1000)
    {
        OnShakeCam?.Invoke(time, str);
    }

    private void Update()
    {
        if(shakeTime > 0)
        {
            transform.position = Random.insideUnitSphere * shaking + startPosition;
            shakeTime -= Time.deltaTime;
        }

        else
        {
            shakeTime = 0.0f;
            transform.position = startPosition;
        }
    }
}
