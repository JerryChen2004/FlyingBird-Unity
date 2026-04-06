using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomCheckpoint : MonoBehaviour
{
    private UI ui;

    // Start is called before the first frame update
    void Start()
    {
        ui = FindObjectOfType<UI>();

        Vector3 pos = transform.position;
        pos.y = Random.Range(-0.5f, 0.5f);
        transform.position = pos;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            gameManager.instance.AddScore(1);
            ui.ShowScore();
            Destroy(gameObject);
        }
    }
}
