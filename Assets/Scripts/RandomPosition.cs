using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomPosition : MonoBehaviour
{
    void Start()
    {
        Vector3 pos = transform.position;
        pos.y = Random.Range(-1f, 1f);
        transform.position = pos;
    }
}
