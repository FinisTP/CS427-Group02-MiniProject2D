using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float MaxSpeed;
    private Rigidbody2D rb;
    private Animator _anim;
    private Vector2 _mousePos;
    private bool isFacingRight;
    private GameManager_ gameManager;
    private Vector2 _mousePosOnScreen;
    public GameObject GFX;

    public Vector2 StartPosition;
    public ParticleSystem GrowParticle;
    public GameObject Point;
    public Texture2D CursorSprite;

    public ParticleSystem DustParticle;
    public TrailRenderer Trail;

    public Joystick joystick;

    private bool isInvincible = false;
    public int DashMultiplier = 2;
    private bool isDead = false;

    [SerializeField] private float moveSpeed = 5;
    [SerializeField] private float boostFactor = 2;
    [SerializeField] private float boostUnit = 25;

    private float moveX = 0, moveY = 0;
    private bool boosting = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        _anim = GFX.GetComponent<Animator>();
        gameManager = GameManager_.Instance;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.SetCursor(CursorSprite, new Vector2(0, 0), CursorMode.Auto);
        //Cursor.visible = false;
        StartPosition = transform.position;
        joystick = GameObject.FindObjectOfType<FixedJoystick>();
    }

    private void Update()
    {
        if (!GameManager_.Instance.IsRunningGame || isDead) return;
        // Keyboard
        if (!Application.isMobilePlatform && _mousePosOnScreen == (Vector2)Input.mousePosition)
        {
            moveX = Input.GetAxisRaw("Horizontal");
            moveY = Input.GetAxisRaw("Vertical");
            Vector2 log = new Vector2(moveX, moveY).normalized;
            moveX = log.x;
            moveY = log.y;
            boosting = Input.GetKey(KeyCode.Space);
            _anim.SetFloat("Speed", 1);
            if ((moveX < -0.01f && isFacingRight) || (moveX > 0.01f && !isFacingRight))
            {
                FlipSprite();
            }
            return;
        }
        else if (Application.isMobilePlatform)
        {
            moveX = joystick.Horizontal;
            moveY = joystick.Vertical;
            Vector2 log = new Vector2(moveX, moveY).normalized;
            moveX = log.x;
            moveY = log.y;
            _anim.SetFloat("Speed", 1);
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
            else
            {
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
        if (!gameManager.IsRunningGame) return;
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
            rb.velocity = new Vector2(moveX * MaxSpeed * factor, moveY * MaxSpeed * factor);

        if (rb.velocity.magnitude > 0) DustParticle.Play();
        if (rb.velocity.magnitude >= MaxSpeed * boostFactor) _anim.SetBool("Dash", true);
        else _anim.SetBool("Dash", false);
    }

    private void FlipSprite()
    {
        Vector3 currentLocalScale = transform.localScale;
        transform.localScale = Vector3.Scale(currentLocalScale, new Vector3(-1, 1, 1));
        isFacingRight = !isFacingRight;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDead || !gameManager.IsRunningGame) return;
        if (collision.gameObject.CompareTag("Enemy"))
        {
            EnemyBehavior eb = collision.gameObject.GetComponent<EnemyBehavior>();
            if (gameManager.Stage >= eb.Stage)
            {
                _anim.SetTrigger("Attack");
                eb.gameObject.SetActive(false);
                gameManager.AddProgress(eb.value);
                int score = gameManager.AddScore(eb.scoreValue);
                GameManager_.Instance.ParticlePlayer.PlayEffect("BloodSplatter", transform.position);
                if (gameManager.Stage == eb.Stage) CinemachineShake.Instance.ShakeCamera(3f, .2f);
                SpawnFloatingPoint(score, eb.scoreValue);
                
            }
            else if (!isInvincible)
            {
                // TODO: reset stage
                gameManager.AddLive(-1);
                gameManager.ResetProgress();
                rb.velocity = Vector2.zero;
                GameManager_.Instance.ParticlePlayer.PlayEffect("BloodSplatter", collision.transform.position);
                if (!gameManager.Lost)
                {
                    StartCoroutine(Respawn());
                    StartCoroutine(DamagedCooldown());
                }
            }

            GameManager_.Instance.SoundPlayer.PlayClip("Eat", 0.5f);
        }
        if (collision.gameObject.CompareTag("Live"))
        {
            collision.gameObject.SetActive(false);
            gameManager.AddLive(1);
            GameManager_.Instance.SoundPlayer.PlayClip("Mushroom", 0.5f);
            GameManager_.Instance.ParticlePlayer.PlayEffect("Mushroom", collision.transform.position);
        }
    }

    private void SpawnFloatingPoint(int score, int originalScore)
    {
        GameObject point = Instantiate(Point, transform.position, Quaternion.identity);
        TextMesh tm = point.GetComponentInChildren<TextMesh>();
        tm.text = "+" + score.ToString();
        if (score > originalScore)
        {
            tm.fontSize = 100 + (int)(originalScore / score) * 5;
        } else tm.fontSize = 100;
        point.GetComponentInChildren<MeshRenderer>().sortingOrder = 10;
        Destroy(point, 2f);
    }

    IEnumerator DamagedCooldown()
    {
        isInvincible = true;
        yield return new WaitForSeconds(4f);
        isInvincible = false;
    }

    public void SetBoosting(bool state)
    {
        if (Application.isMobilePlatform)
            boosting = state;
    }

    IEnumerator Respawn()
    {
        isDead = true;
        GFX.SetActive(false);
        yield return new WaitForSeconds(2f);
        GFX.SetActive(true);
        isDead = false;
        transform.position = StartPosition;
        GrowParticle.Play();
    }
}
