using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController_Temp : MonoBehaviour
{
    public float MaxSpeed;
    private Rigidbody2D _rb;
    private Animator _anim;
    private Vector2 _mousePos;
    private bool isFacingRight;
    private GameManager_ gameManager;
    
    

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        gameManager = GameManager_.Instance;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (!GameManager_.Instance.IsRunningGame) return;
        _mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // Consider using add force
        Vector2 dir = _mousePos - (Vector2)transform.position;
        if (dir.magnitude >= 0.5f)
        {
            float speed = Mathf.Clamp(dir.magnitude * 5, 0, MaxSpeed);

            _rb.velocity = dir.normalized * speed;
        }
        else
        {
            _rb.velocity = Vector2.zero;
        }
        _anim.SetFloat("Speed", dir.magnitude);
        if (_rb.velocity.x > 0)
        {
            isFacingRight = true;
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (_rb.velocity.x < 0)
        {
            isFacingRight = false;
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }

    }

    

    private bool CheckMouseInBound()
    {
        return (Input.mousePosition.x <= Screen.width && Input.mousePosition.x >= 0
            && Input.mousePosition.y <= Screen.height && Input.mousePosition.y >= 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            EnemyBehavior eb = collision.gameObject.GetComponent<EnemyBehavior>();
            if (gameManager.Stage >= eb.Stage)
            {
                eb.gameObject.SetActive(false);
                gameManager._progress += 0.1f;
                if (gameManager._progress >= 1f)
                {
                    gameManager.ScaleUp();
                }
            }
        }
    }

}
