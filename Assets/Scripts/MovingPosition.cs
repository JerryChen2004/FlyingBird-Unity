using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPosition : MonoBehaviour
{
    private float speed = 0.5f;

    void Start()
    {
        Vector3 pos = transform.position;
        pos.y = Random.Range(-1f, 1f);
        transform.position = pos;

        bool startUp = Random.value > 0.5f;

        if (startUp)
            StartCoroutine(MoveY(1f, -1f));
        else
            StartCoroutine(MoveY(-1f, 1f));
    }
    IEnumerator MoveY(float firstY, float secondY)
    {
        Vector3 posA = new Vector3(transform.position.x, firstY, transform.position.z);
        Vector3 posB = new Vector3(transform.position.x, secondY, transform.position.z);

        while (true)
        {
            yield return StartCoroutine(MoveToPosition(posB));
            yield return StartCoroutine(MoveToPosition(posA));
        }
    }

    IEnumerator MoveToPosition(Vector3 target)
    {
        float time = 0;
        Vector3 start = transform.position;

        while (time < 1f)
        {
            time += Time.deltaTime * speed;
            transform.position = Vector3.Lerp(start, target, time);
            yield return null;
        }

        transform.position = target;
    }
}
