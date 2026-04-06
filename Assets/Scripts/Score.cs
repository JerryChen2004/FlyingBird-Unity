using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Score : MonoBehaviour
{
    private UI ui;
    // Start is called before the first frame update
    void Start()
    {
        ui = FindObjectOfType<UI>();
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
