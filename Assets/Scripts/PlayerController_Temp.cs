using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerController_Temp : MonoBehaviour
{
    public float MaxSpeed;
    private Rigidbody2D rb;
    private Animator _anim;
    private Vector2 _mousePos;
    private bool isFacingRight;
    private GameManager_ gameManager;
    private Vector2 _mousePosOnScreen;
    public GameObject cursor;

    public Vector2 StartPosition;
    public ParticleSystem GrowParticle;
    public GameObject Point;
    public Texture2D CursorSprite;

    public Joystick joystick;

    private bool isInvincible = false;
    public int DashMultiplier = 2;

    [SerializeField] private float moveSpeed = 5;
    [SerializeField] private float boostFactor = 2;
    [SerializeField] private float boostUnit = 25;

    private float moveX = 0, moveY = 0;
    private bool boosting = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        gameManager = GameManager_.Instance;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.SetCursor(CursorSprite, new Vector2(0, 0), CursorMode.Auto);
        //Cursor.visible = false;
        StartPosition = transform.position;
    }

    private void Update()
    {
        if (!GameManager_.Instance.IsRunningGame) return;
        // Keyboard
        if (!Application.isMobilePlatform && _mousePosOnScreen == (Vector2) Input.mousePosition)
        {
            if (false)
            {
                moveX = Input.GetAxisRaw("Horizontal");
                moveY = Input.GetAxisRaw("Vertical");
                boosting = Input.GetKey(KeyCode.Space);
            }
            
            if ((moveX < -0.01f && isFacingRight) || (moveX > 0.01f && !isFacingRight))
            {
                FlipSprite();
            }
        }
        else if (Application.isMobilePlatform)
        {
            moveX = joystick.Horizontal;
            moveY = joystick.Vertical;
            return;
        }
        // Mouse
        _mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = _mousePos - (Vector2)transform.position;
        if (dir.magnitude >= 0.5f)
        {
            float speed = Mathf.Clamp(dir.magnitude * 5, 0, MaxSpeed);
            float factor = 1;
            if (Input.GetMouseButton(0))
            {
                gameManager.StopChargingBoost();
                factor = gameManager.UseBoost(boostUnit * Time.fixedDeltaTime) ? boostFactor : factor;
                rb.velocity = dir.normalized * speed * factor;
            }
            else {
                gameManager.ContinueChargingBoost();
                rb.velocity = dir.normalized * speed;
            } 
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
        _anim.SetFloat("Speed", dir.magnitude);
        if (rb.velocity.x > 0 && !isFacingRight)
        {
            FlipSprite();
        }
        else if (rb.velocity.x < 0 && isFacingRight)
        {
            FlipSprite();
        }
        _mousePosOnScreen = Input.mousePosition;
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
        if (moveX != 0 || moveY != 0)
        rb.velocity = new Vector2(moveX * moveSpeed * factor, moveY * moveSpeed * factor);
    }

    private void FlipSprite()
    {
        Vector3 currentLocalScale = transform.localScale;
        transform.localScale = Vector3.Scale(currentLocalScale, new Vector3(-1, 1, 1));
        isFacingRight = !isFacingRight;
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
                GameManager_.Instance.ParticlePlayer.PlayEffect("BloodSplatter", transform.position);
                if (gameManager.Stage == eb.Stage) CinemachineShake.Instance.ShakeCamera(3f, .2f);
                SpawnFloatingPoint(eb.scoreValue);
                gameManager.AddScore(eb.scoreValue);
            } else if (!isInvincible)
            {
                // TODO: reset stage
                transform.position = StartPosition;
                gameManager.AddLive(-1);
                gameManager.ResetProgress();
                GrowParticle.Play();
                GameManager_.Instance.ParticlePlayer.PlayEffect("BloodSplatter", collision.transform.position);
                StartCoroutine(DamagedCooldown());
            }
            
            GameManager_.Instance.SoundPlayer.PlayClip("Eat", 0.5f);
        }
        if (collision.gameObject.CompareTag("Live"))
        {
            Destroy(collision.gameObject);
            gameManager.AddLive(1);
        }
    }

    private void SpawnFloatingPoint(int score)
    {
        GameObject point = Instantiate(Point, transform.position, Quaternion.identity);
        point.GetComponentInChildren<TextMesh>().text = "+" + score.ToString();
        point.GetComponentInChildren<MeshRenderer>().sortingOrder = 10;
        Destroy(point, 2f);
    }

    IEnumerator DamagedCooldown()
    {
        isInvincible = true;
        yield return new WaitForSeconds(3f);
        isInvincible = false;
    }

    public void SetBoosting(bool state)
    {
        if (Application.isMobilePlatform)
        boosting = state;
    }
}
