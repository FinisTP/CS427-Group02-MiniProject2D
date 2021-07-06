using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5;
    [SerializeField] private float boostFactor = 2;
    [SerializeField] private float boostUnit = 25;

    private GameManager_ gameManager;
    private Rigidbody2D rb;
    private float moveX = 0, moveY = 0;
    private bool isFacingRight = true;
    private bool boosting = false;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gameManager = GameManager_.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        moveX = Input.GetAxisRaw("Horizontal");
        moveY = Input.GetAxisRaw("Vertical");

        if ((moveX < 0 && isFacingRight) || (moveX > 0 && ! isFacingRight))
        {
            FlipSprite();
        }

        boosting = Input.GetKey(KeyCode.Space);
    }

    void FlipSprite()
    {
        Vector3 currentLocalScale = transform.localScale;
        transform.localScale = Vector3.Scale(currentLocalScale, new Vector3(-1, 1, 1));
        isFacingRight = !isFacingRight;
    }

    private void FixedUpdate()
    {
        float factor = 1;
        if (boosting)
        {
            gameManager.StopChargingBoost();
            factor = gameManager.UseBoost(boostUnit * Time.fixedDeltaTime) ? boostFactor : factor;
        }
        else
        {
            gameManager.ContinueChargingBoost();
        }
        rb.velocity = new Vector2(moveX * moveSpeed * factor, moveY * moveSpeed * factor);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            EnemyBehavior eb = collision.gameObject.GetComponent<EnemyBehavior>();
            if (gameManager.Stage >= eb.Stage)
            {
                eb.gameObject.SetActive(false);
                gameManager.AddProgress(eb.value);
            }
            else
            {
                // TODO : insert die here
            }
        }
    }
}
