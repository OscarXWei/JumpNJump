using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpin : MonoBehaviour
{
    public float rotateSpeed = 100f;  // Adjust in inspector or here

    void Update()
    {
        // Spin around Y axis
        transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);
    }
}
