using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShaking : MonoBehaviour
{
    public float shaking;
    [SerializeField]
    float shakeTime;
    Vector3 startPosition;
    private void Start()
    {
        startPosition = new Vector3(0f,0f,-6f);
    }

    public void ShakeForTime(float time)
    {
        shakeTime = time;
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
