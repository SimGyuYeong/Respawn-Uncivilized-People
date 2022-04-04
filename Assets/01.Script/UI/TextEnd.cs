using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextEnd : MonoBehaviour
{
    void Start()
    {
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
    }

    void Update()
    {
        transform.Rotate(0, 250 * Time.deltaTime, 0);
    }
}
