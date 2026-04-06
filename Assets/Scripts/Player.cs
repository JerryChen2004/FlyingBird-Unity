using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    [SerializeField] public float flapForce = 5f;
    [SerializeField] public float flapRotate = 10f;
    [SerializeField] public float forwardSpeed = 2f;

    private Rigidbody2D rb;
    private UI ui;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        ui = FindObjectOfType<UI>();

    }
    void Update()
    {
        Play();
    }

    private void FixedUpdate()
    {
        transform.rotation = Quaternion.Euler(0, 0, rb.velocity.y * flapRotate);
        rb.velocity = new Vector2(forwardSpeed, rb.velocity.y);
    }

    void Play()
    {
        if (gameManager.instance.hasHit != true && gameManager.instance.pause != true)
        {
            if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
            {
                rb.velocity = Vector2.up * flapForce;
                audioManager.instance.PlaySFX("Flap");
            }
        }
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        if (gameManager.instance.hasHit) return;

        if (collision.collider.CompareTag("Obstacle") || collision.collider.CompareTag("Floor"))
        {
            gameManager.instance.hasHit = true;
            gameManager.instance.HighScore();
            audioManager.instance.PlaySFX("Hit");
            ui.GameEnd();
            
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("GameEnd"))
        {
            gameManager.instance.gameEnd = true;
            gameManager.instance.HighScore();
            ui.GameEnd();
            
        }

    }

}
